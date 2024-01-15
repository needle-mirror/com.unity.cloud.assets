using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetDiscovery
{
    public interface IAssetsGridController
    {
        void Init(VisualElement root, VisualTreeAsset assetsGridItemTemplate, Dictionary<AssetType, Texture2D> defaultThumbnails);
        event Action<IAsset> AssetSelected;
        void PopulateAssetsGrid(List<IAsset> assetsInfo);
        void DisplayAssetGrid();
        void ClearAssetGrid();
        void HideAssetGrid();
    }

    class AssetsGridController : IAssetsGridController
    {
        VisualElement m_AssetGridList;
        VisualTreeAsset m_AssetGridItemTemplate;
        Button m_CurrentSelectedButton;
        Label m_CurrentSelectedLabel;
        static readonly Color k_DefaultBackgroundColor = new Color32(46, 46, 46, 255);
        static readonly Color k_DefaultTextColor = new Color32(210, 210, 210, 255);
        static readonly Color k_SelectedBackgroundColor = new Color32(41, 161, 255, 255);
        static readonly Color k_SelectedTextColor = new Color32(46, 46, 46, 255);

        public event Action<IAsset> AssetSelected;
        Dictionary<AssetType, Texture2D> m_DefaultThumbnails;

        public void Init(VisualElement root, VisualTreeAsset assetsGridItemTemplate, Dictionary<AssetType, Texture2D> defaultThumbnails)
        {
            root.style.minWidth = new StyleLength { value = new Length(60.0f, LengthUnit.Percent) };

            m_AssetGridItemTemplate = assetsGridItemTemplate;
            m_DefaultThumbnails = defaultThumbnails;
            m_AssetGridList = root.Q("AssetGridList");

            var scrollView = root.Q<ScrollView>("AssetGridScrollView");
            scrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
        }

        public void PopulateAssetsGrid(List<IAsset> assetsInfo)
        {
            foreach (var asset in assetsInfo)
            {
                var item = m_AssetGridItemTemplate.Instantiate();

                var button = item.Q<Button>("AssetGridItem");

                var label = item.Q<Label>();
                label.text = asset.Name;

                SetThumbnail(asset, button);

                m_AssetGridList.Add(item);

                item.RegisterCallback<ClickEvent>(_ =>
                {
                    m_CurrentSelectedButton?.schedule.Execute(() =>
                    {
                        m_CurrentSelectedButton.style.backgroundColor = k_DefaultBackgroundColor;
                        m_CurrentSelectedLabel.style.backgroundColor = k_DefaultBackgroundColor;
                        m_CurrentSelectedLabel.style.color = k_DefaultTextColor;
                    });

                    button.schedule.Execute(() =>
                    {
                        button.style.backgroundColor = k_SelectedBackgroundColor;
                        label.style.backgroundColor = k_SelectedBackgroundColor;
                        label.style.color = k_SelectedTextColor;

                        m_CurrentSelectedButton = button;
                        m_CurrentSelectedLabel = label;
                    });

                    AssetSelected?.Invoke(asset);
                });
            }
        }

        void SetThumbnail(IAsset asset, VisualElement container)
        {
            var icon = container.Q("Icon");

            // Set the default thumbnail
            var defaultThumbnail = GetDefaultThumbnail(asset.Type);
            if (defaultThumbnail != null)
            {
                icon.style.backgroundImage = new StyleBackground(defaultThumbnail);
            }

            // When a thumbnail is successfully retrieved, set it as the background image and the default is cleared.`
            ThumbnailController.GetThumbnail(asset, texture2D =>
            {
                icon.style.backgroundImage = null;
                container.style.backgroundImage = new StyleBackground(texture2D);
            });
        }

        Texture2D GetDefaultThumbnail(AssetType type)
        {
            return m_DefaultThumbnails.TryGetValue(type, out var texture) ? texture : null;
        }

        public void DisplayAssetGrid()
        {
            if (m_AssetGridList != null) m_AssetGridList.style.display = DisplayStyle.Flex;
        }

        public void HideAssetGrid()
        {
            if (m_AssetGridList != null) m_AssetGridList.style.display = DisplayStyle.None;
        }

        public void ClearAssetGrid()
        {
            if (m_AssetGridList != null && m_AssetGridList.childCount != 0)
                m_AssetGridList.Clear();

            m_CurrentSelectedButton = null;
        }
    }
}
