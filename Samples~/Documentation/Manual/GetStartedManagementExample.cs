namespace Unity.Cloud.Documentation.Assets
{
    #region Example

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Unity.Cloud.Assets;
    using Unity.Cloud.Identity;
    using UnityEngine;

    public class AssetManagementBehaviour
    {
        const int k_DefaultCancellationTimeout = 5000;

        IOrganization[] m_AvailableOrganizations;

        CancellationTokenSource m_ProjectCancellationTokenSrc = new();
        CancellationTokenSource m_AssetCancellationTokenSrc = new();

        public IOrganization[] AvailableOrganizations => m_AvailableOrganizations;
        public IOrganization CurrentOrganization { get; private set; }
        public bool IsOrganizationSelected => CurrentOrganization != null;
        public List<IAssetProject> AvailableProjects { get; } = new();
        public IAssetProject CurrentProject { get; private set; }
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
            CurrentOrganization = null;
        }

        public void SetSelectedOrganization(IOrganization organization)
        {
            CurrentAsset = null;
            CurrentProject = null;
            CurrentOrganization = organization;
            if (CurrentOrganization != null)
            {
                GetProjects();
            }
        }

        public void SetSelectedProject(IAssetProject project)
        {
            CurrentAsset = null;
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
                var organizations = new List<IOrganization>();
                var organizationsAsyncEnumerable = PlatformServices.OrganizationRepository.ListOrganizationsAsync(Range.All);
                await foreach (var organization in organizationsAsyncEnumerable)
                {
                    organizations.Add(organization);
                }
                m_AvailableOrganizations = organizations.ToArray();
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

        public async Task CreateAssetAsync(AssetType assetType)
        {
            var assetCreation = new AssetCreation("GrayTexture_0")
            {
                Description = "Documentation example asset creation.",
                Type = assetType
            };

            var cancellationTokenSrc = new CancellationTokenSource(k_DefaultCancellationTimeout);

            try
            {
                var asset = await CurrentProject.CreateAssetAsync(assetCreation, cancellationTokenSrc.Token);
                if (asset != null)
                {
                    GetAssets();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create asset. {e}");
                throw;
            }
        }

        public async Task UpdateAssetAsync(IAsset asset, IAssetUpdate assetUpdate)
        {
            try
            {
                var cancellationTokenSrc = new CancellationTokenSource(k_DefaultCancellationTimeout);
                await asset.UpdateAsync(assetUpdate, cancellationTokenSrc.Token);
                await asset.RefreshAsync(cancellationTokenSrc.Token);
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

        public void GetProjects()
        {
            m_ProjectCancellationTokenSrc.Cancel();
            m_ProjectCancellationTokenSrc.Dispose();
            m_ProjectCancellationTokenSrc = new CancellationTokenSource();

            try
            {
                var projects = PlatformServices.AssetRepository.ListAssetProjectsAsync(CurrentOrganization.Id, Range.All, m_ProjectCancellationTokenSrc.Token);
                _ = PopulateProjectsAsync(projects);
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

        async Task PopulateProjectsAsync(IAsyncEnumerable<IAssetProject> projects)
        {
            AvailableProjects.Clear();
            CurrentProject = null;

            await foreach (var project in projects.WithCancellation(m_ProjectCancellationTokenSrc.Token))
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
                var token = m_AssetCancellationTokenSrc.Token;
                var assets = CurrentProject.QueryAssets().ExecuteAsync(token);
                _ = PopulateAssetsAsync(assets, token);
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

        async Task PopulateAssetsAsync(IAsyncEnumerable<IAsset> assets, CancellationToken token)
        {
            AvailableAssets.Clear();
            CurrentAsset = null;

            try
            {
                await foreach (var asset in assets.WithCancellation(token))
                {
                    AvailableAssets.Add(asset);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Could not load assets: " + e);
                throw;
            }
        }
    }

    #endregion
}
