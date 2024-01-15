using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetManager
{
    public class AssetListUi : ListUi<AssetListUi.AssetListController, IAsset>
    {
        public class AssetListController : ListController<IAsset>
        {
            readonly Dictionary<Button, int> m_OpenButtons = new();

            VisualElement m_CurrentMenuPopupOwner;

            public override void Initialize(ListView listView, VisualTreeAsset itemTemplate, Action<IEnumerable<object>> onSelectionChange)
            {
                base.Initialize(listView, itemTemplate, onSelectionChange);

                listView.RegisterCallback<ClickEvent>(HandleOutClickEvent);

                var scrollview = listView.Q<ScrollView>();
                scrollview.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
                scrollview.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            }

            protected override VisualElement OnMakeItem(VisualTreeAsset itemTemplate)
            {
                var item = base.OnMakeItem(itemTemplate);

                item.Q<Button>("ExpandButton").RegisterCallback<ClickEvent>(evt =>
                {
                    ChangeMenuPopupDisplay(item, DisplayStyle.Flex);
                    evt.StopImmediatePropagation();
                });

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
                element.Q<Label>("VersionLabel").text = asset.Descriptor.AssetVersion.ToString();
                element.Q<Label>("StatusLabel").text = asset.Status;

                m_OpenButtons.Add(element.Q<Button>("OpenButton"), i);
                element.Q<Button>("OpenButton").RegisterCallback<ClickEvent>(OnOpenButtonClick);
            }

            protected override void OnUnbindItem(VisualElement element, int i)
            {
                base.OnUnbindItem(element, i);

                m_OpenButtons.Remove(element.Q<Button>("OpenButton"));
                element.Q<Button>("OpenButton").UnregisterCallback<ClickEvent>(OnOpenButtonClick);
            }

            protected override bool AreEqual(IAsset item1, IAsset item2)
            {
                return item1.Descriptor.Equals(item2.Descriptor);
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
                m_CurrentMenuPopupOwner = null;
            }

            public void HandleOutClickEvent(ClickEvent evt)
            {
                if (m_CurrentMenuPopupOwner == null) return;

                ChangeMenuPopupDisplay(m_CurrentMenuPopupOwner, DisplayStyle.None);
            }

            void ChangeMenuPopupDisplay(VisualElement item, DisplayStyle displayStyle)
            {
                var itemMenuPopup = item.Q("MenuPopup");

                if (m_CurrentMenuPopupOwner != null && itemMenuPopup != m_CurrentMenuPopupOwner)
                {
                    displayStyle = DisplayStyle.None;
                }

                itemMenuPopup.style.display = displayStyle;
                m_CurrentMenuPopupOwner = displayStyle == DisplayStyle.None ? null : item;
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

            void OnOpenButtonClick(EventBase evt)
            {
                if (evt.target is Button button && m_OpenButtons.TryGetValue(button, out var index))
                {
                    m_ListView.SetSelection(index);
                    ChangeMenuPopupDisplay(m_CurrentMenuPopupOwner, DisplayStyle.None);
                }
            }
        }

        public event Action<IAsset> AssetSelected;

        protected override string VisualElementName => "AssetListBox";
        protected override string EmptyListMessage => string.Empty;

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
            m_ListController.ClearList();
            m_ListController.ClearSelection();
            m_ListController.ClearMenuPopupOwner();
        }

        protected override void OnSelectionChange(IEnumerable<object> selectedItems)
        {
            var selection = selectedItems.FirstOrDefault();
            AssetSelected?.Invoke(selection as IAsset);
        }
    }
}
