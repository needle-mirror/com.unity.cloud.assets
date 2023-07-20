namespace Unity.Cloud.Assets.Documentation.Discovery
{
    #region Example

    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Unity.Cloud.Assets;
    using UnityEngine;

    public class AssetDiscoveryBehaviour
    {
        static readonly Pagination m_DefaultPagination = new(nameof(IAsset.Name), 25);

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

        async Task GetAssetsAsync()
        {
            m_AvailableAssets = null;
            CurrentAsset = null;

            try
            {
                var cancellationTokenSrc = new CancellationTokenSource();
                m_AvailableAssets = await PlatformServices.AssetProvider.SearchAsync(new AssetSearchFilter(m_CurrentOrganization, m_CurrentProject), m_DefaultPagination, cancellationTokenSrc.Token);
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

        public void GetPreviousAssets()
        {
            m_AvailableAssets = m_AvailableAssets?.PreviousPage as IAssetPage;
        }

        public Task GetNextAvailableAssetsAsync()
        {
            return Task.CompletedTask;
        }
    }

    #endregion
}
