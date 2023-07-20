namespace Unity.Cloud.Assets.Documentation.Management
{
    #region Example

    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using UnityEngine;

    public class AssetManagementBehaviour
    {
        static readonly Pagination m_DefaultPagination = new(nameof(IAsset.Name), 25);
        const int k_DefaultCancellationTimeout = 5000;

        IOrganization[] m_AvailableOrganizations;
        IOrganization m_CurrentOrganization;

        IProjectPage m_AvailableProjects;
        IProject m_CurrentProject;

        IAssetPage m_AvailableAssets;

        public IOrganization[] AvailableOrganizations => m_AvailableOrganizations;
        public IOrganization CurrentOrganization => m_CurrentOrganization;
        public bool IsOrganizationSelected => m_CurrentOrganization != null;
        public IProjectPage AvailableProjects => m_AvailableProjects;
        public IProject CurrentProject => m_CurrentProject;
        public bool IsProjectSelected => m_CurrentProject != null;
        public IAssetPage AvailableAssets => m_AvailableAssets;
        public IAsset CurrentAsset { get; set; }

        public void Clear()
        {
            m_CurrentProject = null;
            m_AvailableProjects = null;
            m_CurrentOrganization = null;
        }

        public void SetSelectedOrganization(IOrganization organization)
        {
            m_CurrentOrganization = organization;
            if (m_CurrentOrganization != null)
            {
                _ = GetProjectsAsync();
            }
        }

        public void SetSelectedProject(IProject project)
        {
            m_CurrentProject = project;
            if (m_CurrentProject != null)
            {
                _ = GetAssetsAsync();
            }
        }

        public async Task GetOrganizationsAsync()
        {
            m_AvailableOrganizations = null;

            try
            {
                var cancellationTokenSrc = new CancellationTokenSource();
                m_AvailableOrganizations = await PlatformServices.OrganizationProvider.GetOrganizationsAsync(cancellationTokenSrc.Token);
            }
            catch (OperationCanceledException oe)
            {
                Debug.LogError(oe);
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

        public async Task GetProjectsAsync()
        {
            m_AvailableProjects = null;

            try
            {
                var cancellationTokenSrc = new CancellationTokenSource();
                m_AvailableProjects = await PlatformServices.ProjectProvider.GetCurrentUserProjectList(m_CurrentOrganization, m_DefaultPagination, cancellationTokenSrc.Token);
            }
            catch (OperationCanceledException oe)
            {
                Debug.LogError(oe);
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

        public void GetPreviousProjects()
        {
            m_AvailableProjects = m_AvailableProjects?.PreviousPage as IProjectPage;
        }

        public Task GetNextAvailableProjectsAsync()
        {
            return Task.CompletedTask;
        }

        public void GetPreviousAssets()
        {
            m_AvailableAssets = m_AvailableAssets?.PreviousPage as IAssetPage;
        }

        public Task GetNextAvailableAssetsAsync()
        {
            return Task.CompletedTask;
        }

        public async Task CreateAssetAsync()
        {
            var assetCreation = new AssetCreation
            {
                Organization = m_CurrentOrganization,
                Project = m_CurrentProject,
                Name = "GrayTexture_0",
                Description = $"Documentation example asset creation.",
                Type = nameof(Texture2D),
                Version = 1,
                VersionName = "1.0.0"
            };

            var cancellationTokenSrc = new CancellationTokenSource(k_DefaultCancellationTimeout);

            try
            {
                var asset = await PlatformServices.AssetManager.CreateAssetAsync(assetCreation, cancellationTokenSrc.Token);
                if (asset != null)
                {
                    // Refresh available assets
                    await GetAssetsAsync();

                    // Set created asset as current asset
                    CurrentAsset = asset;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create asset. {e.Message}");
                throw;
            }
        }

        public async Task UpdateAssetAsync(IAsset asset)
        {
            try
            {
                var cancellationTokenSrc = new CancellationTokenSource(k_DefaultCancellationTimeout);

                await PlatformServices.AssetManager.UpdateAssetAsync(asset, cancellationTokenSrc.Token);
            }
            catch (OperationCanceledException oe)
            {
                Debug.LogError(oe);
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

        public async Task DeleteAssetAsync(IAsset asset)
        {
            try
            {
                var cancellationTokenSrc = new CancellationTokenSource(k_DefaultCancellationTimeout);
                await PlatformServices.AssetManager.DeleteAssetAsync(asset, cancellationTokenSrc.Token);

                // Refresh available assets
                await GetAssetsAsync();

                // Reset current asset
                CurrentAsset = null;
            }
            catch (OperationCanceledException oe)
            {
                Debug.LogError(oe);
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

        async Task GetAssetsAsync()
        {
            m_AvailableAssets = null;
            CurrentAsset = null;

            try
            {
                var cancellationTokenSrc = new CancellationTokenSource(k_DefaultCancellationTimeout);
                m_AvailableAssets = await PlatformServices.AssetManager.SearchAsync(new AssetSearchFilter(m_CurrentOrganization, m_CurrentProject), m_DefaultPagination, cancellationTokenSrc.Token);
            }
            catch (OperationCanceledException oe)
            {
                Debug.LogError(oe);
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
    }

    #endregion
}
