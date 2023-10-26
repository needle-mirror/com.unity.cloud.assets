#if !UC_EXCLUDE_SAMPLES
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
        static readonly Color k_DefaultButtonBackgroundColor = new Color32(46, 46, 46, 255);
        static readonly Color k_DefaultButtonColor = new Color32(255, 255, 255, 255);
        static readonly Color k_SelectedButtonBackgroundColor = new Color32(41, 161, 255, 255);
        static readonly Color k_SelectedButtonColor = new Color32(0, 0, 0, 255);

        public event Action<IAsset> AssetSelected;
        const int k_ThumbnailSize = 100;
        Dictionary<AssetType, Texture2D> m_DefaultThumbnails;

        public void Init(VisualElement root, VisualTreeAsset assetsGridItemTemplate, Dictionary<AssetType, Texture2D> defaultThumbnails)
        {
            root.style.minWidth = new StyleLength { value = new Length(60.0f, LengthUnit.Percent) };

            m_AssetGridItemTemplate = assetsGridItemTemplate;
            m_DefaultThumbnails = defaultThumbnails;
            m_AssetGridList = root.Q<VisualElement>("AssetGridList");

            var scrollView = root.Q<ScrollView>("AssetGridScrollView");

            scrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
        }

        public void PopulateAssetsGrid(List<IAsset> assetsInfo)
        {
            m_AssetGridList.Clear();
            m_CurrentSelectedButton = null;

            foreach (var asset in assetsInfo)
            {
                var item = m_AssetGridItemTemplate.Instantiate();
                var button = item.Q<Button>("AssetGridItem");
                var icon = button.Q<VisualElement>("Icon");
                var defaultThumbnail = GetDefaultThumbnail(asset.Type);
                if(defaultThumbnail != null)
                    icon.style.backgroundImage = new StyleBackground(defaultThumbnail);

                button.text = asset.Name;
                _= ThumbnailController.GetThumbnail(asset, texture2D =>
                {
                    icon .style.backgroundImage = null;
                    button .style.backgroundImage = new StyleBackground(texture2D);
                }, k_ThumbnailSize);

                m_AssetGridList.Add(item);

                item.RegisterCallback<ClickEvent>(_ =>
                {
                    if (m_CurrentSelectedButton != null)
                    {
                        m_CurrentSelectedButton.schedule.Execute(() =>
                        {
                            m_CurrentSelectedButton.style.backgroundColor = k_DefaultButtonBackgroundColor;
                            m_CurrentSelectedButton.style.color = k_DefaultButtonColor;
                        });
                    }

                    button.schedule.Execute(() =>
                    {
                        button.style.backgroundColor = k_SelectedButtonBackgroundColor;
                        button.style.color = k_SelectedButtonColor;

                        m_CurrentSelectedButton = button;
                    });

                    AssetSelected?.Invoke(asset);
                });
            }
        }

        Texture2D GetDefaultThumbnail(AssetType type)
        {
            if (m_DefaultThumbnails.TryGetValue(type, out var texture))
            {
                return texture;
            }
            return null;
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
#endif
