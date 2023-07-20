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

        internal void Init(VisualElement assetGridList, VisualTreeAsset assetGridItem, MonoBehaviour coroutineHandler)
        {
            m_AssetGridList = assetGridList;
            m_AssetGridItemTemplate = assetGridItem;
            m_CoroutineHandler = coroutineHandler;
        }

        internal void PopulateAssetsGrid(List<IAsset> assetsInfo)
        {
            foreach (var asset in assetsInfo)
            {
                var item = m_AssetGridItemTemplate.Instantiate();
                Button button = item.Q<Button>("AssetGridItem");

                button.text = asset.Name;
                DownloadThumbnail(button, asset);

                m_AssetGridList.Add(item);

                item.RegisterCallback<ClickEvent>(evt =>
                {
                    m_SelectedAsset = asset;
                });
            }
        }

        internal IAsset GetAsset()
        {
            return m_SelectedAsset;
        }


        void  DownloadThumbnail(Button button, IAsset asset)
        {
            foreach (var file in asset.Files)
            {
                if (file.Id == asset.PreviewFileId && file.DownloadUrl != null)
                {
                    using var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(file.DownloadUrl));
                    m_CoroutineHandler.StartCoroutine(SetImage(button, file.DownloadUrl));
                    break;
                }
            }
        }


        IEnumerator SetImage(Button button, string url)
        {
#if USE_WEBTEXTURE
            using var uwr = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
            uwr.downloadHandler = new DownloadHandlerTexture();
            yield return uwr.SendWebRequest();
            button.style.backgroundImage = new StyleBackground(DownloadHandlerTexture.GetContent(uwr));
#endif
        }
    }
}
#endif
