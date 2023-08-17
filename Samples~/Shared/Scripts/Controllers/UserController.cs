#if !UC_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Cloud.Identity;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public class UserController : MonoBehaviour
    {
        static readonly Pagination m_DefaultPagination = new(nameof(IAsset.Name), Range.All);

        [SerializeField]
        UIDocument m_SampleUiDocument;
        [SerializeField]
        protected VisualTreeAsset m_ListItemTemplate;

        VisualElement m_SampleUiDocumentRoot;

        readonly OrganizationListUi m_OrganizationListUi = new();
        readonly ProjectListUi m_ProjectListUi = new();

        public IOrganization SelectedOrganization => m_OrganizationListUi.SelectedOrganization;
        public IProject SelectedProject => m_ProjectListUi.SelectedProject;
        public IProject ProjectAll => m_ProjectListUi.ProjectAll;

        public event Action ShowContent;
        public event Action HideContent;
        public event Action OrganizationSelected;
        public event Action ProjectSelected;

        void Start()
        {
            if (m_SampleUiDocument)
                m_SampleUiDocumentRoot = m_SampleUiDocument.rootVisualElement;

            m_OrganizationListUi.Initialize(m_SampleUiDocumentRoot);
            m_OrganizationListUi.OrganizationSelected += OnOrganizationSelected;

            m_ProjectListUi.Initialize(m_SampleUiDocumentRoot, m_ListItemTemplate);
            m_ProjectListUi.ProjectSelected += OnProjectSelected;

            ProcessAuthenticator();

            m_OrganizationListUi.Hide();
            m_ProjectListUi.Hide();
        }

        void OnDestroy()
        {
            if (PlatformServices.AuthenticationStateProvider != null)
            {
                PlatformServices.AuthenticationStateProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
            }

            m_OrganizationListUi.OrganizationSelected -= OnOrganizationSelected;
            m_ProjectListUi.ProjectSelected -= OnProjectSelected;
        }

        void ProcessAuthenticator()
        {
            if (PlatformServices.Authenticator.RequiresGUI)
            {
                PlatformServices.AuthenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
            }
            else
            {
                OnAuthenticationStateChanged(AuthenticationState.LoggedIn);
            }
        }

        public IAsyncEnumerable<IAsset> GetAssetsAcrossAllProjectsAsync()
        {
            try
            {
                IEnumerable<IProject> projects = m_ProjectListUi.GetProjects();

                var cancellationTokenSource = new CancellationTokenSource(20000);
                return PlatformServices.AssetProvider.SearchAsync(SelectedOrganization, projects, new AssetSearchFilter(null), m_DefaultPagination, cancellationTokenSource.Token);
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

        public IAsyncEnumerable<IAsset> GetAssetsAsync()
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                return PlatformServices.AssetProvider.SearchAsync(new AssetSearchFilter(SelectedProject), m_DefaultPagination, cancellationTokenSource.Token);
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

        public void AddProjectAllEntry()
        {
            m_ProjectListUi.AddAllItem();
        }

        public IEnumerable<IProject> GetAllProjects()
        {
            return m_ProjectListUi.GetProjects();
        }

        async void OnAuthenticationStateChanged(AuthenticationState newAuthenticationState)
        {
            switch (newAuthenticationState)
            {
                case AuthenticationState.LoggedIn:
                    await m_OrganizationListUi.PopulateOrganizations();
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
            OrganizationSelected?.Invoke();

            await m_ProjectListUi.Populate(SelectedOrganization);
        }

        void OnProjectSelected()
        {
            ProjectSelected?.Invoke();
        }
    }
}
#endif
