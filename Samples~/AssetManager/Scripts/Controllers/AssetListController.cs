#if !UC_EXCLUDE_SAMPLES

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetManager
{
    public class AssetListController
    {
        VisualElement m_AssetList;
        VisualTreeAsset m_AssetListItemTemplate;

        VisualElement m_CurrentMenuPopupOwnerItem;

        internal event Action<IAsset> AssetSelected;
        internal event Action AssetCreated;

        internal void Init(VisualElement root, VisualTreeAsset itemTemplate)
        {
            root.RegisterCallback<ClickEvent>(HandleOutClickEvent);

            m_AssetListItemTemplate = itemTemplate;

            m_AssetList = root.Q<VisualElement>("AssetList");
            m_AssetList.RegisterCallback<ClickEvent>(HandleOutClickEvent);

            var scrollView = root.Q<ScrollView>("AssetListScrollView");
            scrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;

            var addButton = root.Q<Button>("AddAssetButton");
            addButton.RegisterCallback<ClickEvent>(_ =>
            {
                AssetCreated?.Invoke();
            });
        }

        internal void PopulateAssetsList(List<IAsset> assetsList)
        {
            foreach (var asset in assetsList)
            {
                var item = m_AssetListItemTemplate.Instantiate();

                item.Q<Label>("TitleLabel").text = asset.Name;
                item.Q<Label>("IngestedDateLabel").text = asset.AuthoringInfo.Updated.ToString("MMM dd, yyyy");
                item.Q<Label>("IngestedTimeLabel").text = asset.AuthoringInfo.Updated.ToString("h:mm tt GMT");
                item.Q<Label>("DescriptionLabel").text = asset.Description;
                item.Q<Label>("TagsLabel").text = asset.Tags.FirstOrDefault();
                item.Q<Label>("TypeLabel").text = asset.Type.ToString();
                item.Q<Label>("VersionLabel").text = asset.Descriptor.AssetVersion.ToString();
                item.Q<Label>("StatusLabel").text = asset.Status;

                m_AssetList.Add(item);

                item.Q<Button>("ExpandButton").RegisterCallback<ClickEvent>(evt =>
                {
                    ChangeMenuPopupDisplay(item, DisplayStyle.Flex);

                    evt.StopImmediatePropagation();
                });

                item.Q<Button>("OpenButton").RegisterCallback<ClickEvent>(_ =>
                {
                    AssetSelected?.Invoke(asset);

                    ChangeMenuPopupDisplay(m_CurrentMenuPopupOwnerItem, DisplayStyle.None);
                });

                ManageAssetListItemStyling(item);
            }
        }

        static void ManageAssetListItemStyling(VisualElement asset)
        {
            asset.Q<VisualElement>("LeftTopPanel").RegisterCallback<MouseOverEvent>(_ =>
            {
                asset.Q<VisualElement>("LeftTopPanel").style.backgroundColor = new Color(1f, 1f, 1f, 0.12f);
                asset.Q<VisualElement>("RightPanel").style.backgroundColor = new Color(0.25f, 0.25f, 0.25f, 1f);
            });

            asset.Q<VisualElement>("LeftTopPanel").RegisterCallback<MouseOutEvent>(_ =>
            {
                asset.Q<VisualElement>("LeftTopPanel").style.backgroundColor = new Color(0.07f, 0.07f, 0.07f, 1f);
                asset.Q<VisualElement>("RightPanel").style.backgroundColor = new Color(0.14f, 0.14f, 0.14f, 1f);
            });
        }

        internal void ClearAssetList()
        {
            m_AssetList.Clear();
            m_CurrentMenuPopupOwnerItem = null;
        }

        void ChangeMenuPopupDisplay(VisualElement item, DisplayStyle displayStyle)
        {
            var itemMenuPopup = item.Q<VisualElement>("MenuPopup");

            if (displayStyle == DisplayStyle.Flex)
            {
                if (m_CurrentMenuPopupOwnerItem != null && itemMenuPopup != m_CurrentMenuPopupOwnerItem)
                {
                    ChangeMenuPopupDisplay(m_CurrentMenuPopupOwnerItem, DisplayStyle.None);
                }

                itemMenuPopup.style.display = displayStyle;

                m_CurrentMenuPopupOwnerItem = item;
            }
            else
            {
                itemMenuPopup.style.display = displayStyle;

                m_CurrentMenuPopupOwnerItem = null;
            }
        }

        void HandleOutClickEvent(ClickEvent evt)
        {
            if (m_CurrentMenuPopupOwnerItem == null) return;

            ChangeMenuPopupDisplay(m_CurrentMenuPopupOwnerItem, DisplayStyle.None);
        }
    }
}
#endif
