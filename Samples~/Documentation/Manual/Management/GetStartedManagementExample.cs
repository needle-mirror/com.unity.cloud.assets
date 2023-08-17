namespace Unity.Cloud.Assets.Documentation.Management
{
    #region Example

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using UnityEngine;

    public class AssetManagementBehaviour
    {
        static readonly Pagination m_DefaultPagination = new(nameof(IAsset.Name), Range.All);
        const int k_DefaultCancellationTimeout = 5000;

        IOrganization[] m_AvailableOrganizations;
        IOrganization m_CurrentOrganization;

        CancellationTokenSource m_ProjectCancellationTokenSrc = new ();
        CancellationTokenSource m_AssetCancellationTokenSrc = new ();

        public IOrganization[] AvailableOrganizations => m_AvailableOrganizations;
        public IOrganization CurrentOrganization => m_CurrentOrganization;
        public bool IsOrganizationSelected => m_CurrentOrganization != null;
        public List<IProject> AvailableProjects { get; } = new();
        public IProject CurrentProject { get; private set; }
        public bool IsProjectSelected => CurrentProject != null;
        public List<IAsset> AvailableAssets { get; } = new();
        public IAsset CurrentAsset { get; set; }

        public void Clear()
        {
            m_ProjectCancellationTokenSrc.Cancel();
            m_ProjectCancellationTokenSrc.Dispose();
            m_AssetCancellationTokenSrc.Cancel();
            m_AssetCancellationTokenSrc.Dispose();

            CurrentAsset = null;
            CurrentProject = null;
            m_CurrentOrganization = null;
        }

        public void SetSelectedOrganization(IOrganization organization)
        {
            m_CurrentOrganization = organization;
            if (m_CurrentOrganization != null)
            {
                GetProjects();
            }
        }

        public void SetSelectedProject(IProject project)
        {
            CurrentProject = project;
            if (CurrentProject != null)
            {
                GetAssets();
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

        public async Task CreateAssetAsync()
        {
            var assetCreation = new AssetCreation
            {
                Project = CurrentProject,
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
                    GetAssets();
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
                GetAssets();
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

        public void GetProjects()
        {
            m_ProjectCancellationTokenSrc.Cancel();
            m_ProjectCancellationTokenSrc.Dispose();
            m_ProjectCancellationTokenSrc = new CancellationTokenSource();

            try
            {
                var projects = PlatformServices.ProjectProvider.ListProjectsAsync(m_CurrentOrganization, m_DefaultPagination, m_ProjectCancellationTokenSrc.Token);
                _ = PopulateProjectsAsync(projects);
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

        async Task PopulateProjectsAsync(IAsyncEnumerable<IProject> projects)
        {
            AvailableProjects.Clear();
            CurrentProject = null;

            await foreach(var project in projects.WithCancellation(m_ProjectCancellationTokenSrc.Token))
            {
                AvailableProjects.Add(project);
            }
        }

        void GetAssets()
        {
            m_AssetCancellationTokenSrc.Cancel();
            m_AssetCancellationTokenSrc.Dispose();
            m_AssetCancellationTokenSrc = new CancellationTokenSource();

            try
            {
                var assets = PlatformServices.AssetManager.SearchAsync(new AssetSearchFilter(CurrentProject), m_DefaultPagination, m_AssetCancellationTokenSrc.Token);
                _ = PopulateAssetsAsync(assets);
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

        async Task PopulateAssetsAsync(IAsyncEnumerable<IAsset> assets)
        {
            AvailableAssets.Clear();
            CurrentAsset = null;

            await foreach(var asset in assets.WithCancellation(m_AssetCancellationTokenSrc.Token))
            {
                AvailableAssets.Add(asset);
            }
        }
    }

    #endregion
}
