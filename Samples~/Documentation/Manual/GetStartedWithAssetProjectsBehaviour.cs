namespace Unity.Cloud.Documentation.Assets
{
    #region Example

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Unity.Cloud.Assets;
    using Unity.Cloud.Common;
    using Unity.Cloud.Identity;
    using UnityEngine;

    public class AssetManagementBehaviour : BaseAssetBehaviour
    {
        IOrganization[] m_AvailableOrganizations;

        CancellationTokenSource m_ProjectCancellationTokenSrc = new();

        public override bool CanSelectAsset => IsProjectSelected;
        public IOrganization[] AvailableOrganizations => m_AvailableOrganizations;
        public IOrganization CurrentOrganization { get; private set; }
        public bool IsOrganizationSelected => CurrentOrganization != null;
        public List<IAssetProject> AvailableProjects { get; } = new();
        public IAssetProject CurrentProject { get; private set; }
        public bool IsProjectSelected => CurrentProject != null;

        readonly Dictionary<ProjectId, string> m_ProjectNames = new();

        public override void Clear()
        {
            base.Clear();
            
            m_ProjectCancellationTokenSrc.Cancel();
            m_ProjectCancellationTokenSrc.Dispose();

            CurrentProject = null;
            m_ProjectNames.Clear();
            AvailableProjects.Clear();
            CurrentOrganization = null;
        }
        
        public override void ClearParentSelection()
        {
            SetSelectedProject(null);
        }

        public void SetSelectedOrganization(IOrganization organization)
        {
            CurrentAsset = null;
            CurrentProject = null;
            CurrentOrganization = organization;
            if (CurrentOrganization != null)
            {
                _ = GetProjectsAsync();
            }
        }

        public void SetSelectedProject(IAssetProject project)
        {
            CurrentAsset = null;
            CurrentProject = project;
            if (CurrentProject != null)
            {
                Debug.Log($"Selected project: {GetProjectName(CurrentProject.Descriptor.ProjectId)}");
                _ = GetAssetCount(CurrentProject);
                _ = GetAssetsAsync(CurrentProject.QueryAssets());
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

        public async Task GetProjectsAsync()
        {
            m_ProjectCancellationTokenSrc.Cancel();
            m_ProjectCancellationTokenSrc.Dispose();
            m_ProjectCancellationTokenSrc = new CancellationTokenSource();

            try
            {
                var token = m_ProjectCancellationTokenSrc.Token;
                var projects = PlatformServices.AssetRepository.ListAssetProjectsAsync(CurrentOrganization.Id, Range.All, token);

                AvailableProjects.Clear();
                CurrentProject = null;

                await foreach (var project in projects)
                {
                    AvailableProjects.Add(project);

                    var properties = await project.GetPropertiesAsync(token);
                    m_ProjectNames[project.Descriptor.ProjectId] = properties.Name;
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

        public string GetProjectName(ProjectId projectId)
        {
            return m_ProjectNames.TryGetValue(projectId, out var name) ? name : projectId.ToString();
        }

        async Task GetAssetCount(IAssetProject assetProject)
        {
            AssetCount = await assetProject.CountAssetsAsync(CancellationToken.None);
        }
    }

    #endregion
}
