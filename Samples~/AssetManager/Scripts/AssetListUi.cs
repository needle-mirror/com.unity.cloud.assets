using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetManager
{
    public class AssetListUi : ListUi<AssetListUi.AssetListController, IAsset>
    {
        public class AssetListController : ListController<IAsset>
        {
            readonly Dictionary<Button, int> m_ExpandButtons = new();

            VisualElement m_MenuPopup;
            int m_CurrentMenuPopupOwner = -1;

            public event Action<IAsset> RemoveAsset;

            public override void Initialize(ListView listView, Func<VisualElement> makeItem, Action<IEnumerable<object>> onSelectionChange)
            {
                base.Initialize(listView, makeItem, onSelectionChange);

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

            protected override void OnBindItem(VisualElement element, int i)
            {
                _ = PopulateItemAsync(element, m_List[i]);

                var expandButton = element.Q<Button>("ExpandButton");
                m_ExpandButtons.Add(expandButton, i);
                expandButton.RegisterCallback<ClickEvent>(OnExpandButtonClick);
            }

            protected override void OnUnbindItem(VisualElement element, int i)
            {
                base.OnUnbindItem(element, i);

                var expandButton = element.Q<Button>("ExpandButton");
                expandButton.UnregisterCallback<ClickEvent>(OnExpandButtonClick);
                m_ExpandButtons.Remove(expandButton);
            }

            public void SetSelection(IAsset asset)
            {
                m_ListView.SetSelection(m_List.FindIndex(a => a.Descriptor.Equals(asset.Descriptor)));
            }

            public void UpdateItem(IAsset asset)
            {
                var index = m_List.FindIndex(a => a.Descriptor.AssetId.Equals(asset.Descriptor.AssetId));
                if (index < 0) return;

                m_List[index] = asset;
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

            static async Task PopulateItemAsync(VisualElement element, IAsset asset)
            {
                var properties = await asset.GetPropertiesAsync(CancellationToken.None);

                element.Q<Label>("TitleLabel").text = properties.Name;
                element.Q<Label>("IngestedDateLabel").text = properties.AuthoringInfo?.Updated.ToString("MMM dd, yyyy") ?? "unknown";
                element.Q<Label>("IngestedTimeLabel").text = properties.AuthoringInfo?.Updated.ToString("h:mm tt GMT") ?? "unknown";
                element.Q<Label>("DescriptionLabel").text = properties.Description;
                element.Q<Label>("TagsLabel").text = properties.Tags.FirstOrDefault();
                element.Q<Label>("TypeLabel").text = properties.Type.ToString();
                element.Q<Label>("Label").text = asset.Descriptor.AssetVersion.GetVersionText(properties);
                element.Q<Label>("StatusLabel").text = properties.StatusName;
            }
        }

        public event Action<IAsset> AssetSelected;

        protected override string VisualElementName => "AssetListContainer";
        protected override string EmptyListMessage => "Empty";

        public event Action<IAsset> RemoveAsset
        {
            add => m_ListController.RemoveAsset += value;
            remove => m_ListController.RemoveAsset -= value;
        }

        public override void Initialize(VisualElement uiDocumentRoot, Func<VisualElement> makeItem)
        {
            base.Initialize(uiDocumentRoot, makeItem);

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
