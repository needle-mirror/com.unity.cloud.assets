using System;
using System.Threading.Tasks;
using System.Collections.Generic;
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

            public async Task DownloadThumbnail(Uri uri)
            {
#if USE_WEBTEXTURE
                using var uwr = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbGET);
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

        static Dictionary<Uri, ThumbnailDownloadEntry> m_ThumbnailCache = new();

        public static async Task GetThumbnail(IAsset asset, Action<Texture2D> thumbnailReadyCallback)
        {
            var previewFileUrl = await asset.GetPreviewUrlAsync(default);
            if (previewFileUrl == null) return;

            if (!m_ThumbnailCache.TryGetValue(previewFileUrl, out var entry))
            {
                // Create new download request
                entry = new ThumbnailDownloadEntry();
                _ = entry.DownloadThumbnail(previewFileUrl);

                lock (entry.Listeners)
                {
                    entry.Listeners.Add(thumbnailReadyCallback);
                }

                m_ThumbnailCache.Add(previewFileUrl, entry);
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
