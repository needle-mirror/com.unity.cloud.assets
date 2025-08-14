namespace Unity.Cloud.Documentation.Assets
{
    #region Example

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Unity.Cloud.Assets;
    using Unity.Cloud.Common;
    using UnityEngine;

    public abstract class BaseAssetBehaviour
    {
        internal class LogProgress : IProgress<HttpProgress>
        {
            string m_Id;

            public LogProgress(string id)
            {
                m_Id = id;
            }

            public void Report(HttpProgress value)
            {
                if (value.UploadProgress.HasValue)
                {
                    Debug.Log($"Upload progress for {m_Id}: {value.UploadProgress * 100} %");
                }

                if (value.DownloadProgress.HasValue)
                {
                    Debug.Log($"Download progress for {m_Id}: {value.DownloadProgress * 100} %");
                }
            }
        }

        CancellationTokenSource m_AssetCancellationTokenSrc = new();
        Dictionary<AssetId, List<AssetVersion>> m_AssetVersions = new();
        Dictionary<AssetVersion, AssetProperties> m_AssetProperties { get; } = new();

        public abstract bool CanSelectAsset { get; }
        public List<IAsset> AvailableAssets { get; } = new();
        public int AssetCount { get; protected set; }
        public IAsset CurrentAsset { get; set; }

        public virtual void Clear()
        {
            m_AssetCancellationTokenSrc.Cancel();
            m_AssetCancellationTokenSrc.Dispose();

            CurrentAsset = null;
        }

        public abstract void ClearParentSelection();

        public async Task GetAssetsAsync(AssetQueryBuilder assetQueryBuilder, AssetDescriptor? selectedAsset = null)
        {
            m_AssetCancellationTokenSrc.Cancel();
            m_AssetCancellationTokenSrc.Dispose();
            m_AssetCancellationTokenSrc = new CancellationTokenSource();

            try
            {
                var token = m_AssetCancellationTokenSrc.Token;
                var assets = assetQueryBuilder.ExecuteAsync(token);

                AvailableAssets.Clear();
                m_AssetVersions.Clear();
                m_AssetProperties.Clear();
                CurrentAsset = null;

                await foreach (var asset in assets)
                {
                    AvailableAssets.Add(asset);
                    if (asset.Descriptor == selectedAsset)
                    {
                        CurrentAsset = asset;
                    }
                    
                    IncludeAssetVersion(asset.Descriptor);

                    var properties = await asset.GetPropertiesAsync(token);
                    m_AssetProperties[asset.Descriptor.AssetVersion] = properties;
                }
            }
            catch (OperationCanceledException oe)
            {
                Debug.Log(oe);
            }
            catch (AggregateException e)
            {
                Debug.LogError(e.InnerException);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public bool TryGetAssetProperties(AssetVersion assetVersion, out AssetProperties properties) => m_AssetProperties.TryGetValue(assetVersion, out properties);
    
        public bool TryGetAssetProperties(AssetId assetId, out AssetProperties properties)
        {
            if (m_AssetVersions.TryGetValue(assetId, out var versions) && versions.Count > 0)
            {
                return TryGetAssetProperties(versions[0], out properties);
            }

            properties = default;
            return false;
        }

        public void IncludeProperties(AssetDescriptor assetDescriptor, AssetProperties properties)
        {
            IncludeAssetVersion(assetDescriptor);
            m_AssetProperties[assetDescriptor.AssetVersion] = properties;
        }

        void IncludeAssetVersion(AssetDescriptor assetDescriptor)
        {
            if (!m_AssetVersions.TryGetValue(assetDescriptor.AssetId, out var versions))
            {
                versions = new List<AssetVersion>();
                m_AssetVersions[assetDescriptor.AssetId] = versions;
            }
            versions.Add(assetDescriptor.AssetVersion);
        }
    }

    #endregion
}
