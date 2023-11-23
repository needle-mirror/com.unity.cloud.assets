using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity.Cloud.Assets.Samples
{
    public static class ThumbnailController
    {
        class ThumbnailDownloadEntry
        {
            public Texture2D Texture2D;
            public readonly List<Action<Texture2D>> Listeners = new();

            public async Task DownloadThumbnail(string url)
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

                Texture2D = DownloadHandlerTexture.GetContent(uwr);

                // Texture is ready
                lock (Listeners)
                {
                    foreach (var listener in Listeners)
                    {
                        listener.Invoke(Texture2D);
                    }

                    Listeners.Clear();
                }
#else
                await Task.CompletedTask;
#endif
            }
        }

        static Dictionary<string, ThumbnailDownloadEntry> m_ThumbnailCache = new();

        public static void GetThumbnail(IAsset asset, Action<Texture2D> thumbnailReadyCallback, int width)
        {
            if (asset.PreviewFileUrl == null) return;

            var resizedUrl = $"https://transformation.unity.com/api/images?url={Uri.EscapeDataString(asset.PreviewFileUrl.ToString())}&width={width}";

            if (!m_ThumbnailCache.TryGetValue(asset.PreviewFile, out var entry))
            {
                // Create new download request
                entry = new ThumbnailDownloadEntry();
                _ = entry.DownloadThumbnail(resizedUrl);

                lock (entry.Listeners)
                {
                    entry.Listeners.Add(thumbnailReadyCallback);
                }

                m_ThumbnailCache.Add(asset.PreviewFile, entry);
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
                }
                else
                {
                    thumbnailReadyCallback.Invoke(entry.Texture2D);
                }
            }
        }
    }
}
