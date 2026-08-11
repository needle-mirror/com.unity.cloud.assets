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
    public class AssetListUi : ListUi<AssetListUi.AssetListController, object>
    {
        public class AssetListController : ListController<object>
        {
            readonly Dictionary<Button, int> m_ExpandButtons = new();

            VisualElement m_MenuPopup;
            int m_CurrentMenuPopupOwner = -1;
            bool m_IsViewingTrash = false;

            public event Action<IAsset> RemoveAsset;
            public event Action<AssetDescriptor> RestoreAsset;
            public event Action<AssetDescriptor> DeletePermanentlyAsset;

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
                var restoreButton = m_MenuPopup.Q<Button>("Restore");
                restoreButton.RegisterCallback<ClickEvent>(OnRestoreButtonClick);
                var deletePermanentlyButton = m_MenuPopup.Q<Button>("DeletePermanently");
                deletePermanentlyButton.RegisterCallback<ClickEvent>(OnDeletePermanentlyButtonClick);

                UpdateMenuButtons();
            }

            public void SetViewingTrash(bool isViewingTrash)
            {
                m_IsViewingTrash = isViewingTrash;
                UpdateMenuButtons();
            }

            void UpdateMenuButtons()
            {
                var openButton = m_MenuPopup.Q<Button>("Open");
                var removeButton = m_MenuPopup.Q<Button>("Remove");
                var restoreButton = m_MenuPopup.Q<Button>("Restore");
                var deletePermanentlyButton = m_MenuPopup.Q<Button>("DeletePermanently");

                if (m_IsViewingTrash)
                {
                    openButton.style.display = DisplayStyle.None;
                    removeButton.style.display = DisplayStyle.None;
                    restoreButton.style.display = DisplayStyle.Flex;
                    deletePermanentlyButton.style.display = DisplayStyle.Flex;
                }
                else
                {
                    openButton.style.display = DisplayStyle.Flex;
                    removeButton.style.display = DisplayStyle.Flex;
                    restoreButton.style.display = DisplayStyle.None;
                    deletePermanentlyButton.style.display = DisplayStyle.None;
                }
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
                var index = m_List.FindIndex(a => (a as IAsset)?.Descriptor.Equals(asset.Descriptor) == true);
                if (index >= 0)
                    m_ListView.SetSelection(index);
            }

            public void UpdateItem(IAsset asset)
            {
                var index = m_List.FindIndex(a => (a as IAsset)?.Descriptor.AssetId.Equals(asset.Descriptor.AssetId) == true);
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
                    UpdateMenuButtons();
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

                var asset = m_List[m_CurrentMenuPopupOwner] as IAsset;
                if (asset == null) return;

                DialogService.ShowMessage("Trash Asset Confirmation",
                    "This will move the asset to the project's trash.",
                    () =>
                    {
                        RemoveAsset?.Invoke(asset);
                        ClearMenuPopupOwner();
                    }, () => { });
            }

            void OnRestoreButtonClick(EventBase evt)
            {
                evt.StopImmediatePropagation();

                if (m_CurrentMenuPopupOwner < 0) return;

                var trashed = m_List[m_CurrentMenuPopupOwner] as ITrashedAsset;
                if (trashed == null) return;

                var descriptor = trashed.Descriptor;
                DialogService.ShowMessage("Restore Asset Confirmation",
                    "This will restore the asset from trash back to the project.",
                    () =>
                    {
                        RestoreAsset?.Invoke(descriptor);
                        ClearMenuPopupOwner();
                    }, () => { });
            }

            void OnDeletePermanentlyButtonClick(EventBase evt)
            {
                evt.StopImmediatePropagation();

                if (m_CurrentMenuPopupOwner < 0) return;

                var trashed = m_List[m_CurrentMenuPopupOwner] as ITrashedAsset;
                if (trashed == null) return;

                var descriptor = trashed.Descriptor;
                DialogService.ShowMessage("Delete Confirmation",
                    "This will permanently delete the asset from trash. This action cannot be undone.",
                    () =>
                    {
                        DeletePermanentlyAsset?.Invoke(descriptor);
                        ClearMenuPopupOwner();
                    }, () => { });
            }

            static async Task PopulateItemAsync(VisualElement element, object item)
            {
                AssetProperties properties;
                AssetDescriptor descriptor;

                if (item is IAsset asset)
                {
                    properties = await asset.GetPropertiesAsync(CancellationToken.None);
                    descriptor = asset.Descriptor;
                }
                else if (item is ITrashedAsset trashed)
                {
                    properties = await trashed.GetPropertiesAsync(CancellationToken.None);
                    descriptor = trashed.Descriptor;
                }
                else
                    return;

                element.Q<Label>("TitleLabel").text = properties.Name;
                element.Q<Label>("IngestedDateLabel").text = properties.AuthoringInfo?.Updated.ToString("MMM dd, yyyy") ?? "unknown";
                element.Q<Label>("IngestedTimeLabel").text = properties.AuthoringInfo?.Updated.ToString("h:mm tt GMT") ?? "unknown";
                element.Q<Label>("DescriptionLabel").text = properties.Description;
                element.Q<Label>("TagsLabel").text = properties.Tags.FirstOrDefault();
                element.Q<Label>("TypeLabel").text = properties.Type.ToString();
                element.Q<Label>("Label").text = descriptor.AssetVersion.GetVersionText(properties);
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

        public event Action<AssetDescriptor> RestoreAsset
        {
            add => m_ListController.RestoreAsset += value;
            remove => m_ListController.RestoreAsset -= value;
        }

        public event Action<AssetDescriptor> DeletePermanentlyAsset
        {
            add => m_ListController.DeletePermanentlyAsset += value;
            remove => m_ListController.DeletePermanentlyAsset -= value;
        }

        public void SetViewingTrash(bool isViewingTrash)
        {
            m_ListController.SetViewingTrash(isViewingTrash);
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
            UpdateList(assetsList.Cast<object>());
        }

        public void PopulateTrashedAssetsList(IEnumerable<ITrashedAsset> trashedAssetsList)
        {
            UpdateList(trashedAssetsList.Cast<object>());
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
            if (selection is IAsset asset)
                AssetSelected?.Invoke(asset);
        }
    }
}
