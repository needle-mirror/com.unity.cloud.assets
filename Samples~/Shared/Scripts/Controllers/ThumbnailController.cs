#if !UC_EXCLUDE_SAMPLES
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Unity.Cloud.Assets;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity.Cloud.Assets.Samples
{
    public static class ThumbnailController
    {
        class ThumbnailDownloadEntry
        {
            public Texture2D Texture2D;
            public readonly List<Action<Texture2D>> Listeners = new List<Action<Texture2D>>();
        }

        static Dictionary<string, ThumbnailDownloadEntry> m_ThumbnailCache = new Dictionary<string, ThumbnailDownloadEntry>();

        public static async Task GetThumbnail(IAsset asset, Action<Texture2D> thumbnailReadyCallback, int width)
        {
            var file = await asset.GetFileAsync(asset.PreviewFile, CancellationToken.None);
            var url = await file.GetDownloadUrlAsync(CancellationToken.None);

            var resizedUrl = $"https://transformation.unity.com/api/images?url={Uri.EscapeDataString(url.ToString())}&width={width}";

            if (!m_ThumbnailCache.TryGetValue(file.Descriptor.Path, out var entry))
            {
                // Create new download request
                entry = new ThumbnailDownloadEntry
                {
                    Texture2D = await DownloadThumbnail(resizedUrl)
                };

                lock (entry.Listeners)
                {
                    entry.Listeners.Add(thumbnailReadyCallback);
                }

                m_ThumbnailCache.Add(file.Descriptor.Path, entry);
            }
            else
            {
                // Texture is being downloaded
                if (entry.Texture2D == null)
                {
                    lock (entry.Listeners)
                    {
                        entry.Listeners.Add(thumbnailReadyCallback);
                    }

                    return;
                }
            }

            // Texture is ready
            lock (entry.Listeners)
            {
                foreach (var listener in entry.Listeners)
                {
                    listener.Invoke(entry.Texture2D);
                    thumbnailReadyCallback.Invoke(entry.Texture2D);
                }
            }
        }

        static async Task<Texture2D> DownloadThumbnail(string url)
        {
        #if USE_WEBTEXTURE
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(url));

            using var uwr = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
            uwr.downloadHandler = new DownloadHandlerTexture();

            var operation = uwr.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            return DownloadHandlerTexture.GetContent(uwr);
        #else
            return null;
        #endif
        }
    }
}
#endif
