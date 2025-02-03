using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Unity.Cloud.Common;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity.Cloud.Assets.Samples
{
    public static class ThumbnailController
    {
        class ThumbnailDownloadEntry
        {
            const int k_Dimension = 128;
            const int k_Lifetime = 180000; // 3 minutes

            readonly AssetId m_AssetId;
            readonly Action<AssetId> m_OnLifetimeComplete;

            public readonly List<Action<Texture2D>> Listeners = new();
            public bool IsLoaded { get; private set; }
            public Texture2D Texture2D { get; private set; }

            public ThumbnailDownloadEntry(AssetId assetId, FileDescriptor previewFileDescriptor, Action<AssetId> onLifetimeComplete, CancellationToken token)
            {
                m_AssetId = assetId;
                m_OnLifetimeComplete = onLifetimeComplete;


                _ = GetDownloadUrl(previewFileDescriptor, token);
            }

            async Task GetDownloadUrl(FileDescriptor fileDescriptor, CancellationToken token)
            {
                Uri previewFileUrl = null;
                try
                {
                    var file = await PlatformServices.AssetRepository.GetFileAsync(fileDescriptor, token);
                    previewFileUrl = await file.GetResizedImageDownloadUrlAsync(k_Dimension, token);
                }
                catch (TaskCanceledException)
                {
                    m_OnLifetimeComplete?.Invoke(m_AssetId);
                    return;
                }
                catch (NotFoundException)
                {
                    // No preview available
                }
                catch (ObjectDisposedException)
                {
                    // Ignore
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                if (previewFileUrl == null)
                {
                    IsLoaded = true;
                    return;
                }

                await DownloadThumbnail(previewFileUrl);
            }

            async Task DownloadThumbnail(Uri uri)
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
                IsLoaded = true;

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
                await Task.Delay(k_Lifetime);

                m_OnLifetimeComplete?.Invoke(m_AssetId);
            }
        }

        static CancellationTokenSource m_CancellationTokenSource = new();
        static Dictionary<AssetId, ThumbnailDownloadEntry> m_ThumbnailCache = new();

        public static void RefreshCancellationToken()
        {
            m_CancellationTokenSource.Cancel();
            m_CancellationTokenSource.Dispose();
            m_CancellationTokenSource = new CancellationTokenSource();
        }

        public static void GetThumbnail(AssetId assetId, FileDescriptor previewFileDescriptor, Action<Texture2D> thumbnailReadyCallback)
        {
            if (!m_ThumbnailCache.TryGetValue(assetId, out var thumbnail))
            {
                // Create new thumbnail entry
                thumbnail = new ThumbnailDownloadEntry(assetId, previewFileDescriptor, RemoveThumbnail, m_CancellationTokenSource.Token);
                m_ThumbnailCache.Add(assetId, thumbnail);

                lock (thumbnail.Listeners)
                {
                    thumbnail.Listeners.Add(thumbnailReadyCallback);
                }
            }
            else
            {
                // Texture is being downloaded
                if (thumbnail.IsLoaded)
                {
                    thumbnailReadyCallback.Invoke(thumbnail.Texture2D);
                }
                else
                {
                    lock (thumbnail.Listeners)
                    {
                        thumbnail.Listeners.Add(thumbnailReadyCallback);
                    }
                }
            }
        }

        static void RemoveThumbnail(AssetId assetId)
        {
            m_ThumbnailCache.Remove(assetId);
        }
    }
}
