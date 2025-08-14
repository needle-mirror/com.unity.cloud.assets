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

    public class AssetLibrariesBehaviour : BaseAssetBehaviour
    {
        public override bool CanSelectAsset => IsAssetLibrarySelected;
        public List<IAssetLibrary> AvailableLibraries { get; } = new();
        public IAssetLibrary CurrentAssetLibrary { get; private set; }
        public bool IsAssetLibrarySelected => CurrentAssetLibrary != null;
        public List<IAssetLibraryJob> AvailableAssetLibraryJobs { get; } = new();
        public IAssetLibraryJob CurrentAssetLibraryJob { get; private set; }
        public bool IsAssetLibraryJobSelected => CurrentAssetLibraryJob != null;

        readonly Dictionary<AssetLibraryId, AssetLibraryProperties> m_AssetLibraryProperties = new();
        readonly Dictionary<AssetLibraryJobId, AssetLibraryJobProperties> m_AssetLibraryJobProperties = new();
        
        CancellationTokenSource m_LibraryCancellationTokenSrc = new();
        CancellationTokenSource m_JobCancellationTokenSrc = new();

        public override void Clear()
        {
            base.Clear();

            if (m_LibraryCancellationTokenSrc != null)
            {
                m_LibraryCancellationTokenSrc.Cancel();
                m_LibraryCancellationTokenSrc.Dispose();
            }

            if (m_JobCancellationTokenSrc != null)
            {
                m_JobCancellationTokenSrc.Cancel();
                m_JobCancellationTokenSrc.Dispose();
            }

            CurrentAssetLibrary = null;
            m_AssetLibraryProperties.Clear();
            AvailableLibraries.Clear();
            CurrentAssetLibraryJob = null;
            m_AssetLibraryJobProperties.Clear();
            AvailableAssetLibraryJobs.Clear();
        }

        public override void ClearParentSelection()
        {
            SetSelectedAssetLibrary(null);
        }

        public async Task GetAssetLibrariesAsync()
        {
            var cancellationToken = GetLibraryListCancellationToken();

            var libraryId = CurrentAssetLibrary?.Id;
            CurrentAssetLibrary = null;
            m_AssetLibraryProperties.Clear();
            AvailableLibraries.Clear();

            var asyncList = PlatformServices.AssetRepository.ListAssetLibrariesAsync(Range.All, cancellationToken);
            await foreach (var assetLibrary in asyncList)
            {
                AvailableLibraries.Add(assetLibrary);

                var properties = await assetLibrary.GetPropertiesAsync(cancellationToken);

                m_AssetLibraryProperties.Add(assetLibrary.Id, properties);

                if (assetLibrary.Id == libraryId)
                {
                    SetSelectedAssetLibrary(assetLibrary);
                }
            }
        }

        public void SetSelectedAssetLibrary(IAssetLibrary assetLibrary)
        {
            CurrentAsset = null;
            CurrentAssetLibrary = assetLibrary;
            if (CurrentAssetLibrary != null)
            {
                Debug.Log("Selected library: " + assetLibrary.Id);
                _ = GetAssetCount(assetLibrary);
                _ = GetAssetsAsync(assetLibrary.QueryAssets());
            }
        }

        public string GetAssetLibraryName(AssetLibraryId assetLibraryId)
        {
            return m_AssetLibraryProperties.TryGetValue(assetLibraryId, out var properties) ? properties.Name : assetLibraryId.ToString();
        }

        public async Task GetAssetLibraryJobsAsync()
        {
            var cancellationToken = GetJobListCancellationToken();

            var jobId = CurrentAssetLibraryJob?.Id;
            CurrentAssetLibraryJob = null;
            m_AssetLibraryJobProperties.Clear();
            AvailableAssetLibraryJobs.Clear();

            var asyncList = PlatformServices.AssetRepository.ListAssetLibraryJobsAsync(Range.All, cancellationToken);
            await foreach (var job in asyncList)
            {
                AvailableAssetLibraryJobs.Add(job);

                var properties = await job.GetPropertiesAsync(cancellationToken);

                m_AssetLibraryJobProperties.Add(job.Id, properties);

                if (job.Id == jobId)
                {
                    SetSelectedAssetLibraryJob(job);
                }
            }
        }

        public void SetSelectedAssetLibraryJob(IAssetLibraryJob assetLibraryJob)
        {
            CurrentAssetLibraryJob = assetLibraryJob;
            if (CurrentAssetLibrary != null)
            {
                Debug.Log("Selected library job: " + assetLibraryJob.Id);
            }
        }
        
        public string GetAssetLibraryJobName(AssetLibraryJobId assetLibraryJobId)
        {
            return m_AssetLibraryJobProperties.TryGetValue(assetLibraryJobId, out var properties) ? properties.Name : assetLibraryJobId.ToString();
        }
        
        public bool TryGetAssetLibraryJobProperties(AssetLibraryJobId assetLibraryJobId, out AssetLibraryJobProperties properties)
        {
            return m_AssetLibraryJobProperties.TryGetValue(assetLibraryJobId, out properties);
        }

        public void IncludeProperties(AssetLibraryJobId assetLibraryJobId, AssetLibraryJobProperties properties)
        {
            m_AssetLibraryJobProperties[assetLibraryJobId] = properties;
        }

        async Task GetAssetCount(IAssetLibrary assetLibrary)
        {
            AssetCount = await assetLibrary.CountAssetsAsync(CancellationToken.None);
        }

        CancellationToken GetLibraryListCancellationToken()
        {
            if (m_LibraryCancellationTokenSrc != null)
            {
                m_LibraryCancellationTokenSrc.Cancel();
                m_LibraryCancellationTokenSrc.Dispose();
            }

            m_LibraryCancellationTokenSrc = new CancellationTokenSource();
            return m_LibraryCancellationTokenSrc.Token;
        }

        CancellationToken GetJobListCancellationToken()
        {
            if (m_JobCancellationTokenSrc != null)
            {
                m_JobCancellationTokenSrc.Cancel();
                m_JobCancellationTokenSrc.Dispose();
            }

            m_JobCancellationTokenSrc = new CancellationTokenSource();
            return m_JobCancellationTokenSrc.Token;
        }
    }

    #endregion
}
