#if !UC_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.Cloud.Common;
using Unity.Cloud.Identity;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public class UserController : MonoBehaviour
    {
        static readonly Pagination k_DefaultPagination = new(nameof(IAsset.Name), Range.All);

        [SerializeField]
        UIDocument m_OrganizationListUiDocument;

        [SerializeField]
        UIDocument m_ProjectListUiDocument;

        [SerializeField]
        VisualTreeAsset m_ListItemTemplate;

        [SerializeField]
        bool m_IncludeAllProject = true;

        readonly OrganizationListUi m_OrganizationListUi = new();
        readonly ProjectListUi m_ProjectListUi = new();

        public IOrganization SelectedOrganization => m_OrganizationListUi.SelectedOrganization;
        public IAssetProject SelectedProject => m_ProjectListUi.SelectedProject;
        public bool IsAllProjectSelected => m_ProjectListUi.IsAllProjectSelected;
        public IAssetRepository AssetRepository => m_AssetRepository;

        public event Action ShowContent;
        public event Action HideContent;
        public event Action<OrganizationId> OrganizationSelected;
        public event Action ProjectSelected;

        IAssetRepository m_AssetRepository;
        ICompositeAuthenticator m_Authenticator;
        IOrganizationRepository m_OrganizationRepository;

        public void SetServices(ICompositeAuthenticator authenticator, IAssetRepository assetRepository, IOrganizationRepository organizationRepository)
        {
            m_Authenticator = authenticator;
            m_AssetRepository = assetRepository;
            m_OrganizationRepository = organizationRepository;
        }

        void Start()
        {
            m_OrganizationListUi.Initialize(m_OrganizationListUiDocument.rootVisualElement);
            m_OrganizationListUi.OrganizationSelected += OnOrganizationSelected;

            m_ProjectListUi.Initialize(m_ProjectListUiDocument.rootVisualElement, m_ListItemTemplate);
            m_ProjectListUi.ProjectSelected += OnProjectSelected;

            ProcessAuthenticator();

            m_OrganizationListUi.Hide();
            m_ProjectListUi.Hide();
        }

        void OnDestroy()
        {
            if (m_Authenticator != null)
            {
                m_Authenticator.AuthenticationStateChanged -= OnAuthenticationStateChanged;
            }

            m_OrganizationListUi.OrganizationSelected -= OnOrganizationSelected;
            m_ProjectListUi.ProjectSelected -= OnProjectSelected;
        }

        void ProcessAuthenticator()
        {
            if (m_Authenticator.RequiresGUI)
            {
                m_Authenticator.AuthenticationStateChanged += OnAuthenticationStateChanged;
            }
            else
            {
                OnAuthenticationStateChanged(AuthenticationState.LoggedIn);
            }
        }

        public IAsyncEnumerable<IAsset> GetAssetsAcrossAllProjectsAsync(CancellationToken cancellationToken)
        {
            try
            {
                var projects = m_ProjectListUi.GetProjects().Select(p => p.Descriptor.ProjectId);
                return m_AssetRepository.SearchAssetsAsync(SelectedOrganization.Id, projects, new AssetSearchFilter(), k_DefaultPagination, cancellationToken);
            }
            catch (OperationCanceledException oe)
            {
                Debug.LogException(oe);
                throw;
            }
            catch (AggregateException e)
            {
                Debug.LogException(e.InnerException);
                throw;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        public IAsyncEnumerable<IAsset> GetAssetsAsync(CancellationToken cancellationToken)
        {
            try
            {
                var filter = new AssetSearchFilter
                {
                    IncludedFields = new FieldsFilter
                    {
                        AssetFields = AssetFields.all,
                        FileFields = FileFields.downloadUrl
                    }
                };
                return SelectedProject.SearchAssetsAsync(filter, k_DefaultPagination, cancellationToken);
            }
            catch (OperationCanceledException oe)
            {
                Debug.LogException(oe);
                throw;
            }
            catch (AggregateException e)
            {
                Debug.LogException(e.InnerException);
                throw;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        public IEnumerable<IAssetProject> GetAllProjects()
        {
            return m_ProjectListUi.GetProjects();
        }

        async void OnAuthenticationStateChanged(AuthenticationState newAuthenticationState)
        {
            switch (newAuthenticationState)
            {
                case AuthenticationState.LoggedIn:
                    await m_OrganizationListUi.PopulateOrganizations(m_OrganizationRepository);
                    ShowContent?.Invoke();
                    break;
                case AuthenticationState.LoggedOut:
                    m_OrganizationListUi.Hide();
                    m_ProjectListUi.Hide();
                    HideContent?.Invoke();
                    break;
            }
        }

        async void OnOrganizationSelected()
        {
            OrganizationSelected?.Invoke(m_OrganizationListUi.SelectedOrganization.Id);

            await m_ProjectListUi.Populate(m_AssetRepository, SelectedOrganization, m_IncludeAllProject);
        }

        void OnProjectSelected()
        {
            ProjectSelected?.Invoke();
        }
    }
}
#endif
