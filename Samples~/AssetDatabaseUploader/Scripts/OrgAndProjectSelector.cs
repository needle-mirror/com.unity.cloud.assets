#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Identity;
using UnityEngine;

namespace Unity.Cloud.Assets.Samples.AssetDatabaseUploader
{
    [Serializable]
    [ExecuteInEditMode]
    public class OrgAndProjectSelector : MonoBehaviour
    {
        public event Action OnOrgOrProjectChanged;

        static readonly Pagination k_ProjectPagination = new(nameof(IAssetProject.Name), Range.All);

        IOrganizationRepository m_OrganizationRepository;
        IAssetRepository m_AssetRepository;
        AssetDatabaseUploaderSample m_AssetDatabaseUploaderSample;

        List<IOrganization> m_Organizations;
        [HideInInspector]
        [SerializeField]
        string m_SelectedOrganizationId;
        IOrganization m_SelectedOrganization;

        List<IAssetProject> m_Projects;
        [SerializeField]
        [HideInInspector]
        string m_SelectedProjectId;
        IAssetProject m_SelectedProject;

        public List<IOrganization> Organizations => m_Organizations;

        public IOrganization SelectedOrganization => m_SelectedOrganization;

        public List<IAssetProject> Projects => m_Projects;

        public IAssetProject SelectedProject => m_SelectedProject;

        /// <summary>
        /// Initialize the <see cref="OrgAndProjectSelector"/> with the given providers.
        /// </summary>
        /// <param name="assetDatabaseUploaderSample"></param>
        /// <param name="organizationRepository"></param>
        /// <param name="assetRepository"></param>
        public async Task Initialize(AssetDatabaseUploaderSample assetDatabaseUploaderSample, IOrganizationRepository organizationRepository, IAssetRepository assetRepository)
        {
            m_AssetDatabaseUploaderSample = assetDatabaseUploaderSample;

            m_OrganizationRepository = organizationRepository;
            if (m_OrganizationRepository == null)
            {
                Debug.LogError($"An {nameof(IOrganizationRepository)} is required to initialize {nameof(OrgAndProjectSelector)}");
            }

            m_AssetRepository = assetRepository;
            if (m_AssetRepository == null)
            {
                Debug.LogError($"An {nameof(IAssetRepository)} is required to initialize {nameof(OrgAndProjectSelector)}");
            }

            await FetchOrganizationsAndProjectsAsync();
        }

        /// <summary>
        /// Fetch the organizations and projects.
        /// </summary>
        public async Task FetchOrganizationsAndProjectsAsync()
        {
            try
            {
                var orgsArray = await m_OrganizationRepository.ListOrganizationsAsync();

                m_Organizations = orgsArray?.ToList();
                if (m_Organizations?.Count > 0)
                {
                    var selectedOrg = m_Organizations.FirstOrDefault(org => org.Id.ToString() == m_SelectedOrganizationId);
                    m_SelectedOrganization = selectedOrg ?? m_Organizations[0];
                    m_SelectedOrganizationId = m_SelectedOrganization.Id.ToString();// Ensure the selected organization id is set

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
                var projects = m_AssetRepository.ListAssetProjectsAsync(m_SelectedOrganization.Id, k_ProjectPagination, cancellationTokenSource.Token);
                m_Projects = new List<IAssetProject>();
                await foreach (var project in projects)
                {
                    m_Projects.Add(project);
                }

                if (m_Projects.Count > 0)
                {
                    var selectedProj = m_Projects.FirstOrDefault(project => project.Descriptor.ProjectId.ToString() == m_SelectedProjectId);
                    m_SelectedProject = selectedProj ?? m_Projects[0];
                    m_SelectedProjectId = m_SelectedProject.Descriptor.ProjectId.ToString(); // Ensure the selected project id is set
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
                organization?.Id.ToString() == m_SelectedOrganizationId)
                return false;

            m_SelectedOrganization = organization;
            m_SelectedOrganizationId = organization?.Id.ToString() ?? string.Empty;

            return true;
        }

        /// <summary>
        /// Change the selected project and return true if the project changed.
        /// </summary>
        /// <param name="project">the new organization value. </param>
        /// <returns>true if the selected organization changed.</returns>
        public bool ChangeSelectedProject(IAssetProject project)
        {
            if (project == m_SelectedProject ||
                project?.Descriptor.ProjectId.ToString() == m_SelectedProjectId)
                return false;

            m_SelectedProject = project;
            m_SelectedProjectId = project?.Descriptor.ProjectId.ToString() ?? string.Empty;

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
