#if !UC_EXCLUDE_SAMPLES
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetDiscovery
{
    class AssetsGridController
    {
        VisualElement m_AssetGridList;
        VisualTreeAsset m_AssetGridItemTemplate;
        MonoBehaviour m_CoroutineHandler;

        IAsset m_SelectedAsset;
        internal event Action<IAsset> AssetSelected;

        internal void Init(VisualElement root, VisualTreeAsset assetGridItem, MonoBehaviour coroutineHandler)
        {
            m_AssetGridItemTemplate = assetGridItem;
            m_AssetGridList = root.Q<VisualElement>("AssetGridList");
            var scrollView = root.Q<ScrollView>("AssetGridScrollView");

            scrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            m_CoroutineHandler = coroutineHandler;
        }

        internal void PopulateAssetsGrid(List<IAsset> assetsInfo)
        {
            m_AssetGridList.Clear();

            foreach (var asset in assetsInfo)
            {
                var item = m_AssetGridItemTemplate.Instantiate();
                Button button = item.Q<Button>("AssetGridItem");

                button.text = asset.Name;
                DownloadThumbnail(button, asset);

                m_AssetGridList.Add(item);

                item.RegisterCallback<ClickEvent>(_ =>
                {
                    AssetSelected?.Invoke(asset);
                });
            }
        }

        internal void DisplayAssetGrid()
        {
            if (m_AssetGridList != null) m_AssetGridList.style.display = DisplayStyle.Flex;
        }

        internal void HideAssetGrid()
        {
            if (m_AssetGridList != null) m_AssetGridList.style.display = DisplayStyle.None;
        }

        internal void ClearAssetGrid()
        {
            if (m_AssetGridList != null && m_AssetGridList.childCount != 0)
                m_AssetGridList.Clear();
        }

        void DownloadThumbnail(VisualElement visual, IAsset asset)
        {
            foreach (var file in asset.Files)
            {
                if (file.Id == asset.PreviewFileId && file.DownloadUrl != null)
                {
                    using var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(file.DownloadUrl));
                    m_CoroutineHandler.StartCoroutine(SetImage(visual, file.DownloadUrl));
                    break;
                }
            }
        }

        static IEnumerator SetImage(VisualElement visual, string url)
        {
#if USE_WEBTEXTURE
            using var uwr = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
            uwr.downloadHandler = new DownloadHandlerTexture();
            yield return uwr.SendWebRequest();
            visual.style.backgroundImage = new StyleBackground(DownloadHandlerTexture.GetContent(uwr));
#endif
        }
    }
}
#endif
