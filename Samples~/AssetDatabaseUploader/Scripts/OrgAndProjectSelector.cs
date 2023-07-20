#if !UC_EXCLUDE_SAMPLES && UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Cloud.Assets.Samples.AssetDatabaseUploader
{
    [Serializable]
    [ExecuteInEditMode]
    public class OrgAndProjectSelector : MonoBehaviour
    {
        public event Action OnOrgOrProjectChanged;

        static readonly Pagination k_ProjectPagination = new(nameof(IProject.Name), 25);

        IOrganizationProvider m_OrganizationProvider;
        IProjectProvider m_ProjectProvider;
        AssetDatabaseUploaderSample m_AssetDatabaseUploaderSample;

        List<IOrganization> m_Organizations;
        [HideInInspector]
        [SerializeField]
        ulong m_SelectedOrganizationId;
        IOrganization m_SelectedOrganization;

        List<IProject> m_Projects;
        [SerializeField]
        [HideInInspector]
        string m_SelectedProjectId;
        IProject m_SelectedProject;

        public List<IOrganization> Organizations => m_Organizations;

        public IOrganization SelectedOrganization => m_SelectedOrganization;

        public List<IProject> Projects => m_Projects;

        public IProject SelectedProject => m_SelectedProject;

        /// <summary>
        /// Initialize the <see cref="OrgAndProjectSelector"/> with the given providers.
        /// </summary>
        /// <param name="assetDatabaseUploaderSample"></param>
        /// <param name="organizationProvider"></param>
        /// <param name="projectProvider"></param>
        public async Task Initialize(AssetDatabaseUploaderSample assetDatabaseUploaderSample, IOrganizationProvider organizationProvider, IProjectProvider projectProvider)
        {
            m_AssetDatabaseUploaderSample = assetDatabaseUploaderSample;

            m_OrganizationProvider = organizationProvider;
            if (m_OrganizationProvider == null)
            {
                Debug.LogError($"An {nameof(IOrganizationProvider)} is required to initialize {nameof(OrgAndProjectSelector)}");
            }

            m_ProjectProvider = projectProvider;
            if (m_ProjectProvider == null)
            {
                Debug.LogError($"An {nameof(IProjectProvider)} is required to initialize {nameof(OrgAndProjectSelector)}");
            }

            await FetchOrganizationsAndProjectsAsync();
        }

        /// <summary>
        /// Fetch the organizations and projects.
        /// </summary>
        public async Task FetchOrganizationsAndProjectsAsync()
        {
            var cancellationTokenSource = new CancellationTokenSource(m_AssetDatabaseUploaderSample.CancellationTokenTimeout);

            try
            {
                var orgsArray = await m_OrganizationProvider.GetOrganizationsAsync(cancellationTokenSource.Token);

                m_Organizations = orgsArray?.ToList();
                if (m_Organizations?.Count > 0)
                {
                    var selectedOrg = m_Organizations.FirstOrDefault(org => org.GenesisId == m_SelectedOrganizationId);
                    m_SelectedOrganization = selectedOrg ?? m_Organizations[0];
                    m_SelectedOrganizationId = m_SelectedOrganization.GenesisId;// Ensure the selected organization id is set

                    await FetchProjectsAsync();
                }
                else
                {
                    ClearAll();
                }
            }
            catch (OperationCanceledException oe)
            {
                Debug.LogException(oe);

                ClearAll();
            }
            catch (Exception e)
            {
                Debug.LogException(e);

                ClearAll();
            }
        }

        /// <summary>
        /// Fetch the projects for the selected organization.
        /// </summary>
        public async Task FetchProjectsAsync()
        {
            var cancellationTokenSource = new CancellationTokenSource(m_AssetDatabaseUploaderSample.CancellationTokenTimeout);

            try
            {
                var projectsPage = await m_ProjectProvider.GetCurrentUserProjectList(m_SelectedOrganization, k_ProjectPagination, cancellationTokenSource.Token);

                m_Projects = projectsPage?.Elements.ToList();
                if (m_Projects?.Count > 0)
                {
                    var selectedProj = m_Projects.FirstOrDefault(project => project.Id == m_SelectedProjectId);
                    m_SelectedProject = selectedProj ?? m_Projects[0];
                    m_SelectedProjectId = m_SelectedProject.Id;// Ensure the selected project id is set
                }
                else
                {
                    ClearProjects();
                }
            }
            catch (OperationCanceledException oe)
            {
                Debug.LogException(oe);

                ClearProjects();
            }
            catch (Exception e)
            {
                Debug.LogException(e);

                ClearProjects();
            }
        }

        /// <summary>
        /// Change the selected organization and return true if the organization changed.
        /// </summary>
        /// <param name="organization">the new organization value. </param>
        /// <returns>true if the selected organization changed.</returns>
        public bool ChangeSelectedOrganization(IOrganization organization)
        {
            if (organization == m_SelectedOrganization ||
                organization?.GenesisId == m_SelectedOrganizationId)
                return false;

            m_SelectedOrganization = organization;
            m_SelectedOrganizationId = organization?.GenesisId ?? 0;

            return true;
        }

        /// <summary>
        /// Change the selected project and return true if the project changed.
        /// </summary>
        /// <param name="project">the new organization value. </param>
        /// <returns>true if the selected organization changed.</returns>
        public bool ChangeSelectedProject(IProject project)
        {
            if (project == m_SelectedProject ||
                project?.Id == m_SelectedProjectId)
                return false;

            m_SelectedProject = project;
            m_SelectedProjectId = project?.Id;

            OnOrgOrProjectChanged?.Invoke();

            return true;
        }

        void ClearAll()
        {
            m_Organizations = null;
            m_SelectedOrganization = null;

            ClearProjects();
        }

        void ClearProjects()
        {
            m_Projects = null;
            m_SelectedProject = null;
        }
    }
}
#endif
