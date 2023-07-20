#if !UC_EXCLUDE_SAMPLES
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Identity;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetDiscovery
{
    public class AssetDiscoverySample : MonoBehaviour
    {
        internal static readonly Pagination m_DefaultPagination = new(nameof(IAsset.Name), 25);
        const string k_NoProjectsMessage = "No projects available.";

        IOrganizationProvider m_OrganizationProvider;
        IProjectProvider m_ProjectProvider;
        IAssetProvider m_AssetProvider;
        IAssetManager m_AssetManager;

        ProjectsListController m_ProjectsListController;
        AssetsGridController m_AssetsGridController;
        AssetInformationPanelController m_AssetInformationPanelController;
        SearchBarController m_SearchBarController;

        [SerializeField] UIDocument m_UiDocument;
        [SerializeField] VisualTreeAsset m_ProjectsListItemTemplate;
        [SerializeField] VisualTreeAsset m_AssetsGridItemTemplate;
        [SerializeField] VisualTreeAsset m_AssetInformationPanelItemTemplate;
        [SerializeField] VisualTreeAsset m_AssetInformationTagsTemplate;
        [SerializeField] VisualTreeAsset m_SearchBarChipTemplate;

        VisualElement m_UiDocumentRoot;
        VisualElement m_OrganizationsContainer;
        VisualElement m_ProjectsContainer;
        VisualElement m_ProjectsDisplayMessageContainer;
        VisualElement m_AssetGridList;
        VisualElement m_AssetInformationContainer;
        VisualElement m_AssetInformationDownloadSuccessful;
        VisualElement m_SearchBarContainer;
        VisualElement m_SearchBarChipsContainer;
        DropdownField m_OrganizationsDropdown;
        Label m_ProjectsDisplayMessage;
        ListView m_ProjectsListView;
        ScrollView m_AssetGridScrollView;
        ScrollView m_AssetInformationPanelScrollView;
        Button m_SearchBarButton;
        Button m_SearchBarClearButton;
        Button m_AssetDownloadButton;

        List<IAsset> m_CurrentAssetsList;
        List<IAsset> m_UpdatedAssetsList;

        IOrganization m_SelectedOrganization;
        IProject m_SelectedProject;
        IAsset m_SelectedAsset;

        readonly HashSet<IAsset> m_InProgressDownloads = new();

        void Start()
        {
            if (m_UiDocument)
                m_UiDocumentRoot = m_UiDocument.rootVisualElement;

            m_OrganizationProvider = PlatformServices.OrganizationProvider;
            m_ProjectProvider = PlatformServices.ProjectProvider;
            m_AssetProvider = PlatformServices.AssetProvider;
            m_AssetManager = PlatformServices.AssetManager;

            m_OrganizationsContainer = m_UiDocumentRoot.Q<VisualElement>("OrganizationsContainer");
            m_ProjectsContainer = m_UiDocumentRoot.Q<VisualElement>("ProjectsPanel");
            m_ProjectsDisplayMessageContainer = m_UiDocumentRoot.Q<VisualElement>("ProjectsDisplayMessageContainer");
            m_AssetGridList = m_UiDocumentRoot.Q<VisualElement>("AssetGridList");
            m_OrganizationsDropdown = m_UiDocumentRoot.Q<DropdownField>("OrganizationsDropdown");
            m_ProjectsDisplayMessage = m_UiDocumentRoot.Q<Label>("ProjectsDisplayMessage");
            m_ProjectsListView = m_UiDocumentRoot.Q<ListView>("ProjectsList");
            m_AssetGridScrollView = m_UiDocumentRoot.Q<ScrollView>("AssetGridScrollView");
            m_AssetInformationPanelScrollView = m_UiDocumentRoot.Q<ScrollView>("AssetInformationScrollView");
            m_AssetInformationContainer = m_UiDocumentRoot.Q<VisualElement>("AssetInformationContainer");
            m_SearchBarContainer = m_UiDocumentRoot.Q<VisualElement>("SearchBarContainer");
            m_SearchBarButton = m_UiDocumentRoot.Q<Button>("SearchBarButton");
            m_SearchBarClearButton = m_UiDocumentRoot.Q<Button>("SearchBarClearButton");
            m_AssetDownloadButton = m_UiDocumentRoot.Q<Button>("AssetDownloadButton");
            m_AssetInformationDownloadSuccessful = m_UiDocumentRoot.Q<VisualElement>("AssetDownloadSuccessful");

            m_ProjectsListController = new ProjectsListController();
            m_AssetsGridController = new AssetsGridController();
            m_AssetInformationPanelController = new AssetInformationPanelController();
            m_SearchBarController = new SearchBarController();

            m_SearchBarButton.clickable.clicked += OnAddedSearchQuery;
            m_SearchBarClearButton.clickable.clicked += OnClearSearchBarButtonClicked;
            m_AssetDownloadButton.clickable.clicked += OnAssetDownloadButtonClicked;
            m_SearchBarController.deleteQuery += OnDeletedSearchQuery;

            m_SearchBarContainer.AddManipulator(new Clickable(evt =>
            {
                m_SearchBarClearButton.style.display = DisplayStyle.None;
            }));

            m_SearchBarController.Init(m_UiDocumentRoot, m_SearchBarChipTemplate, OnAddedSearchQuery);
            m_AssetsGridController.Init(m_AssetGridList, m_AssetsGridItemTemplate, this);

            if (PlatformServices.Authenticator.RequiresGUI)
            {
                PlatformServices.AuthenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
            }
            else
            {
                OnAuthenticationStateChanged(AuthenticationState.LoggedIn);
            }
        }

        void OnDestroy()
        {
            if (PlatformServices.AuthenticationStateProvider != null)
            {
                PlatformServices.AuthenticationStateProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
            }

            m_SearchBarButton.clickable.clicked -= OnAddedSearchQuery;
            m_SearchBarClearButton.clickable.clicked -= OnClearSearchBarButtonClicked;
            m_AssetDownloadButton.clickable.clicked -= OnAssetDownloadButtonClicked;
            m_SearchBarController.deleteQuery -= OnDeletedSearchQuery;
        }

        void OnAddedSearchQuery()
        {
            m_SearchBarClearButton.style.display = DisplayStyle.Flex;
            m_AssetInformationContainer.style.display = DisplayStyle.None;

            _ = OnAddedSearchQueryAsync();
        }

        async Task OnAddedSearchQueryAsync()
        {
            m_UpdatedAssetsList = await m_SearchBarController.AddChipAsync(m_UpdatedAssetsList);
            OnAssetsListChanged();
        }

        void OnDeletedSearchQuery()
        {
            m_AssetInformationContainer.style.display = DisplayStyle.None;
            if (m_SearchBarController.QueryList.Count == 0)
            {
                ClearSearchBar();
            }

            _ = OnDeletedSearchQueryAsync();
        }

        async Task OnDeletedSearchQueryAsync()
        {
            m_UpdatedAssetsList = await m_SearchBarController.UpdateAssetsListAsync();
            OnAssetsListChanged();
        }

        void OnClearSearchBarButtonClicked()
        {
            ClearSearchBar();
            OnAssetsListChanged();
        }

        async void OnAssetDownloadButtonClicked()
        {
            var assetToDownload = m_SelectedAsset;
            m_InProgressDownloads.Add(assetToDownload);

            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            SetEnabledDownloadButton(false);

            try
            {
                await m_AssetManager.GetAssetDownloadUrlsAsync(assetToDownload, CancellationToken.None);

                foreach (var file in assetToDownload.Files)
                {
                    await using var destination = File.OpenWrite(Path.Combine(path, file.Name));
                    using var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(file.DownloadUrl));

                    using var response = await PlatformServices.HttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead, null,
                        CancellationToken.None);
                    response.EnsureSuccessStatusCode();

                    var source = await response.Content.ReadAsStreamAsync();
                    await source.CopyToAsync(destination);
                }

                StartCoroutine(ShowSuccessfulDownload());
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                m_InProgressDownloads.Remove(assetToDownload);
                if (m_SelectedAsset == assetToDownload)
                {
                    SetEnabledDownloadButton(true);
                }
            }
        }

        void SetEnabledDownloadButton(bool enable)
        {
            m_AssetDownloadButton.SetEnabled(enable);
            m_AssetDownloadButton.text = enable ? "Download" : "Downloading...";
        }

        IEnumerator ShowSuccessfulDownload()
        {
            m_AssetInformationDownloadSuccessful.style.display = DisplayStyle.Flex;
            yield return new WaitForSeconds(3f);
            m_AssetInformationDownloadSuccessful.style.display = DisplayStyle.None;
        }

        async void OnAuthenticationStateChanged(AuthenticationState newAuthenticationState)
        {
            switch (newAuthenticationState)
            {
                case AuthenticationState.LoggedIn:
                    await PopulateOrganizations();
                    break;
                case AuthenticationState.LoggedOut:
                    ClearAllContent();
                    break;
            }
        }

        void DisplayOrganizations()
        {
            m_OrganizationsContainer.style.display = DisplayStyle.Flex;
            m_OrganizationsDropdown.label = "Organizations";
        }

        async void OnOrganizationSelected(IOrganization selectedOrganization)
        {
            ClearAssets();
            m_SelectedOrganization = selectedOrganization;
            Debug.Log($"Organization Selected: {selectedOrganization.Name}");
            await PopulateProjects();
        }

        async void OnProjectSelected(IProject selectedProject)
        {
            ClearAssets();
            m_SelectedProject = selectedProject;
            Debug.Log($"Project Selected: {selectedProject.Name}");
            await PopulateAssets();
        }

        void OnAssetSelected(IAsset selectedAsset)
        {
            if (m_SelectedAsset == selectedAsset) return;

            m_SelectedAsset = selectedAsset;

            if (m_SelectedAsset == null)
            {
                m_AssetInformationContainer.style.display = DisplayStyle.None;
            }
            else
            {
                Debug.Log($"Asset Selected: {selectedAsset.Name}");

                SetEnabledDownloadButton(!m_InProgressDownloads.Contains(m_SelectedAsset));
                DisplayAssetInformationPanel(selectedAsset);
            }
        }

        async Task PopulateOrganizations()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var organizations = await m_OrganizationProvider.GetOrganizationsAsync(cancellationTokenSource.Token);

            m_OrganizationsDropdown.choices = GetOrganizationsList(organizations);

            DisplayOrganizations();

            m_OrganizationsDropdown.value = organizations.First().Name;
            OnOrganizationSelected(organizations.First());

            m_OrganizationsDropdown.RegisterValueChangedCallback(evt =>
            {
                OnOrganizationSelected(organizations.First(info => info.Name == evt.newValue));
            });
        }

        async Task PopulateProjects()
        {
            m_ProjectsContainer.style.display = DisplayStyle.Flex;

            var projects = await GetProjectsAsync(m_SelectedOrganization);

            if (projects.Length == 0)
            {
                m_ProjectsListView.style.display = DisplayStyle.None;
                m_ProjectsDisplayMessageContainer.style.display = DisplayStyle.Flex;
                m_ProjectsDisplayMessage.text = k_NoProjectsMessage;
            }
            else
            {
                m_ProjectsDisplayMessageContainer.style.display = DisplayStyle.None;
                m_ProjectsListView.style.display = DisplayStyle.Flex;

                if (m_ProjectsListView.selectedItem != null)
                    m_ProjectsListView.ClearSelection();
                m_ProjectsListController.Init(m_ProjectsListView, projects, m_ProjectsListItemTemplate);

                m_ProjectsListView.RegisterCallback<ClickEvent>(evt =>
                {
                    OnProjectSelected(m_ProjectsListView.selectedItem as IProject);
                });
            }
        }

        async Task PopulateAssets()
        {
            m_AssetGridList.style.display = DisplayStyle.Flex;
            m_SearchBarContainer.style.display = DisplayStyle.Flex;
            m_SearchBarController.UpdateSearchBarProjectsLabel(m_SelectedOrganization, m_SelectedProject);

            m_CurrentAssetsList = (await GetAssetsAsync(m_SelectedProject)).ToList();

            if (m_AssetGridList.childCount != 0)
                m_AssetGridList.Clear();
            m_AssetsGridController.PopulateAssetsGrid(m_CurrentAssetsList);

            m_AssetGridScrollView.RegisterCallback<ClickEvent>(evt =>
            {
                OnAssetSelected(m_AssetsGridController.GetAsset());
            });

            await UpdateSearchBarValues(SearchBarController.SearchCriterion.Name);
            await UpdateSearchBarValues(SearchBarController.SearchCriterion.Tags);
        }

        async Task UpdateSearchBarValues(SearchBarController.SearchCriterion criterion)
        {
            try
            {
                var parameters = new AggregationParameters(criterion.ToString());
                var aggregation = await m_AssetProvider.AggregateAsync(new AssetSearchFilter(m_SelectedOrganization, m_SelectedProject), parameters, CancellationToken.None);
                m_SearchBarController.UpdateSearchValues(criterion, aggregation.Values.Keys.ToArray());
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

        void OnAssetsListChanged()
        {
            if (m_AssetGridList.childCount != 0)
                m_AssetGridList.Clear();

            if (m_SearchBarController.QueryList.Count == 0)
                m_UpdatedAssetsList = m_CurrentAssetsList;

            m_AssetsGridController.PopulateAssetsGrid(m_SearchBarController.QueryList.Count == 0 ? m_CurrentAssetsList : m_UpdatedAssetsList);
        }

        async Task<IProject[]> GetProjectsAsync(IOrganization currentOrganization)
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var projects = await m_ProjectProvider.GetCurrentUserProjectList(currentOrganization, m_DefaultPagination, cancellationTokenSource.Token);
                return projects.Elements;
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

        async Task<IAsset[]> GetAssetsAsync(IProject currentProject)
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var assets = await m_AssetProvider.SearchAsync(new AssetSearchFilter(m_SelectedOrganization, currentProject), m_DefaultPagination, cancellationTokenSource.Token);
                return assets.Elements;
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

        void DisplayAssetInformationPanel(IAsset selectedAsset)
        {
            m_AssetInformationContainer.style.display = DisplayStyle.Flex;
            m_AssetInformationContainer.Q<Label>("AssetInformationLabel").text = selectedAsset.Name;

            m_AssetInformationPanelController.Init(m_AssetInformationPanelScrollView, selectedAsset,
                m_AssetInformationPanelItemTemplate, m_AssetInformationTagsTemplate);
        }

        static List<string> GetOrganizationsList(IOrganization[] organization)
        {
            return organization.Select(info => info.Name).ToList();
        }

        void ClearAllContent()
        {
            m_OrganizationsContainer.style.display = DisplayStyle.None;
            m_ProjectsContainer.style.display = DisplayStyle.None;
            m_AssetGridList.style.display = DisplayStyle.None;
            m_SearchBarContainer.style.display = DisplayStyle.None;
            m_AssetInformationContainer.style.display = DisplayStyle.None;
        }

        void ClearAssets()
        {
            ClearSearchBar();

            m_AssetGridList.style.display = DisplayStyle.None;
            m_SearchBarContainer.style.display = DisplayStyle.None;
            m_AssetInformationContainer.style.display = DisplayStyle.None;
        }

        void ClearSearchBar()
        {
            m_SearchBarClearButton.style.display = DisplayStyle.None;
            m_SearchBarController.ClearSearchBar();
        }
    }
}
#endif
