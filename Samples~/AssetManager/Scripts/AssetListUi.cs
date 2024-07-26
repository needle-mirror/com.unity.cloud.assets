using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetManager
{
    public class AssetListUi : ListUi<AssetListUi.AssetListController, IAsset>
    {
        public class AssetListController : ListController<IAsset>
        {
            readonly Dictionary<Button, int> m_ExpandButtons = new();
            readonly Dictionary<AssetId, (IStatus, Label)> m_AssetStatuses = new();

            VisualElement m_MenuPopup;
            int m_CurrentMenuPopupOwner = -1;

            public event Action<IAsset> RemoveAsset;

            public override void Initialize(ListView listView, VisualTreeAsset itemTemplate, Action<IEnumerable<object>> onSelectionChange)
            {
                base.Initialize(listView, itemTemplate, onSelectionChange);

                listView.RegisterCallback<ClickEvent>(HandleOutClickEvent);

                var scrollview = listView.Q<ScrollView>();
                scrollview.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
                scrollview.verticalScrollerVisibility = ScrollerVisibility.Hidden;

                var popupTemplate = listView.parent.Q<TemplateContainer>("AssetListItemMenuPopup");
                m_MenuPopup = popupTemplate.templateSource.Instantiate().Q("AssetMenu");
                m_MenuPopup.style.display = DisplayStyle.None;

                var openButton = m_MenuPopup.Q<Button>("Open");
                openButton.RegisterCallback<ClickEvent>(OnOpenButtonClick);
                var removeButton = m_MenuPopup.Q<Button>("Remove");
                removeButton.RegisterCallback<ClickEvent>(OnRemoveButtonClick);
            }

            protected override VisualElement OnMakeItem(VisualTreeAsset itemTemplate)
            {
                var item = base.OnMakeItem(itemTemplate);

                ManageAssetListItemStyling(item);

                return item;
            }

            protected override void OnBindItem(VisualElement element, int i)
            {
                var asset = m_List[i];

                element.Q<Label>("TitleLabel").text = asset.Name;
                element.Q<Label>("IngestedDateLabel").text = asset.AuthoringInfo?.Updated.ToString("MMM dd, yyyy") ?? "unknown";
                element.Q<Label>("IngestedTimeLabel").text = asset.AuthoringInfo?.Updated.ToString("h:mm tt GMT") ?? "unknown";
                element.Q<Label>("DescriptionLabel").text = asset.Description;
                element.Q<Label>("TagsLabel").text = asset.Tags.FirstOrDefault();
                element.Q<Label>("TypeLabel").text = asset.Type.ToString();
                element.Q<Label>("Label").text = asset.GetVersionText();

                var statusLabel = element.Q<Label>("StatusLabel");
                statusLabel.text = "Unknown";

                if (m_AssetStatuses.TryGetValue(asset.Descriptor.AssetId, out (IStatus status, Label _) value))
                {
                    // If the status has been fetched, update the label.
                    if (value.status != null)
                    {
                        statusLabel.text = value.status.Name;
                    }
                    // Otherwise, update the reference to the label.
                    else
                    {
                        m_AssetStatuses[asset.Descriptor.AssetId] = (null, statusLabel);
                    }
                }
                else
                {
                    // If the status has not been fetched, add the asset and its status label to the dictionary and fetch the status.
                    m_AssetStatuses[asset.Descriptor.AssetId] = (null, statusLabel);
                    _ = UpdateStatusAsync(asset);
                }

                var expandButton = element.Q<Button>("ExpandButton");
                m_ExpandButtons.Add(expandButton, i);
                expandButton.RegisterCallback<ClickEvent>(OnExpandButtonClick);
            }

            async Task UpdateStatusAsync(IAsset asset)
            {
                var status = await asset.GetStatusAsync(CancellationToken.None);

                // If the label is still assigned, update it.
                if (m_AssetStatuses.TryGetValue(asset.Descriptor.AssetId, out (IStatus _, Label label) value) && value.label != null)
                {
                    value.label.text = status.Name;
                    m_AssetStatuses[asset.Descriptor.AssetId] = (status, value.label);
                }
            }

            protected override void OnUnbindItem(VisualElement element, int i)
            {
                base.OnUnbindItem(element, i);

                var expandButton = element.Q<Button>("ExpandButton");
                expandButton.UnregisterCallback<ClickEvent>(OnExpandButtonClick);
                m_ExpandButtons.Remove(expandButton);

                // Clear the Label reference
                if (i < m_List.Count)
                {
                    var assetId = m_List[i].Descriptor.AssetId;
                    if (m_AssetStatuses.TryGetValue(assetId, out (IStatus status, Label _) value))
                    {
                        m_AssetStatuses[assetId] = (value.status, null);
                    }
                }
            }

            public override void ClearList()
            {
                base.ClearList();

                m_AssetStatuses.Clear();
            }

            public void SetSelection(IAsset asset)
            {
                m_ListView.SetSelection(m_List.FindIndex(a => a.Descriptor.Equals(asset.Descriptor)));
            }

            public void UpdateItem(IAsset asset)
            {
                var index = m_List.FindIndex(a => a.Descriptor.Equals(asset.Descriptor));
                if (index < 0) return;

                m_ListView.RefreshItem(index);
            }

            public void ClearMenuPopupOwner()
            {
                m_CurrentMenuPopupOwner = -1;
                m_MenuPopup.style.display = DisplayStyle.None;
                m_MenuPopup.RemoveFromHierarchy();
            }

            public void HandleOutClickEvent(ClickEvent evt)
            {
                ClearMenuPopupOwner();
            }

            static void ManageAssetListItemStyling(VisualElement element)
            {
                element.Q("LeftTopPanel").RegisterCallback<MouseOverEvent>(_ =>
                {
                    element.Q("LeftTopPanel").style.backgroundColor = new Color(0.14f, 0.14f, 0.14f, 1f);
                    element.Q("RightPanel").style.backgroundColor = new Color(0.19f, 0.19f, 0.19f, 1f);
                });

                element.Q("LeftTopPanel").RegisterCallback<MouseOutEvent>(_ =>
                {
                    element.Q("LeftTopPanel").style.backgroundColor = new Color(0.07f, 0.07f, 0.07f, 1f);
                    element.Q("RightPanel").style.backgroundColor = new Color(0.14f, 0.14f, 0.14f, 1f);
                });
            }

            void OnExpandButtonClick(EventBase evt)
            {
                evt.StopImmediatePropagation();

                if (evt.target is Button button && m_ExpandButtons.TryGetValue(button, out var index))
                {
                    m_CurrentMenuPopupOwner = index;
                    button.parent.Add(m_MenuPopup);
                    m_MenuPopup.style.display = DisplayStyle.Flex;
                }
            }

            void OnOpenButtonClick(EventBase evt)
            {
                evt.StopImmediatePropagation();

                if (m_CurrentMenuPopupOwner < 0) return;

                m_ListView.SetSelection(m_CurrentMenuPopupOwner);
                ClearMenuPopupOwner();
            }

            void OnRemoveButtonClick(EventBase evt)
            {
                evt.StopImmediatePropagation();

                if (m_CurrentMenuPopupOwner < 0) return;

                DialogService.ShowMessage("Remove Asset Confirmation",
                    "This will remove the asset from the current project. Note that once an asset is no longer linked to any projects it is effectively deleted.",
                    () =>
                    {
                        RemoveAsset?.Invoke(m_List[m_CurrentMenuPopupOwner]);
                        m_List.RemoveAt(m_CurrentMenuPopupOwner);
                        m_ListView.Rebuild();
                        ClearMenuPopupOwner();
                    }, () => { });
            }
        }

        public event Action<IAsset> AssetSelected;

        protected override string VisualElementName => "AssetListBox";
        protected override string EmptyListMessage => string.Empty;

        public event Action<IAsset> RemoveAsset
        {
            add => m_ListController.RemoveAsset += value;
            remove => m_ListController.RemoveAsset -= value;
        }

        public override void Initialize(VisualElement uiDocumentRoot, VisualTreeAsset listItemTemplate)
        {
            base.Initialize(uiDocumentRoot, listItemTemplate);

            uiDocumentRoot.RegisterCallback<ClickEvent>(m_ListController.HandleOutClickEvent);
        }

        public void SelectAsset(IAsset asset)
        {
            m_ListController.SetSelection(asset);
        }

        public void UpdateAsset(IAsset asset)
        {
            m_ListController.UpdateItem(asset);
        }

        public void PopulateAssetsList(IEnumerable<IAsset> assetsList)
        {
            UpdateList(assetsList);
        }

        public void ClearAssetList()
        {
            m_ListController.ClearMenuPopupOwner();
            m_ListController.ClearSelection();
            m_ListController.ClearList();
        }

        protected override void OnSelectionChange(IEnumerable<object> selectedItems)
        {
            var selection = selectedItems.FirstOrDefault();
            AssetSelected?.Invoke(selection as IAsset);
        }
    }
}
