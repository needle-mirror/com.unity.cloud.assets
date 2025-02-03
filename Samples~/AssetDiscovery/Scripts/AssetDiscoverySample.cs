using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetDiscovery
{
    public class AssetDiscoverySample : MonoBehaviour
    {
        const int k_RefreshIncrement = 100;

        [SerializeField]
        UIDocument m_UiDocument;

        [SerializeField]
        ProjectController m_ProjectController;

        [SerializeField]
        SearchBarUi m_SearchBarUi;

        [SerializeField]
        VisualTreeAsset m_AssetGridLayoutTemplate;

        [SerializeField]
        VisualTreeAsset m_AssetInformationLayoutTemplate;

        [SerializeField]
        VisualTreeAsset m_AssetsGridItemTemplate;

        [SerializeField]
        DefaultThumbnail[] m_DefaultThumbnails;

        VisualElement m_UiDocumentRoot;
        VisualElement m_ContentPanel;

        AssetsGridController m_AssetsGridController;
        AssetInformationPanelController m_AssetInformationPanelController;

        readonly List<IAsset> m_ProjectAssetsList = new();
        OrganizationId m_OrganizationId;
        Dictionary<string, string> m_FieldToName;
        Dictionary<string, string> m_StatusFlowToName;

        IAsset m_SelectedAsset;

        CancellationTokenSource m_NewListCancellationTokenSource;

        [Serializable]
        public struct DefaultThumbnail
        {
            [field: SerializeField]
            public AssetType AssetType { get; set; }

            [field: SerializeField]
            public Texture2D Thumbnail { get; set; }
        }

        void Start()
        {
            if (m_UiDocument)
                m_UiDocumentRoot = m_UiDocument.rootVisualElement;

            m_AssetsGridController = new AssetsGridController();
            m_AssetInformationPanelController = new AssetInformationPanelController(m_ProjectController);

            m_ContentPanel = m_UiDocumentRoot.Q<VisualElement>("Content");

            m_SearchBarUi.Initialize(m_UiDocumentRoot, m_UiDocumentRoot.Q<VisualElement>("SearchBarContainer"), m_ProjectController.AssetRepository);
            m_SearchBarUi.DeleteSearchQuery += OnSearchQueryChanged;
            m_SearchBarUi.AddSearchQuery += OnSearchQueryChanged;
            m_SearchBarUi.ClearSearchQuery += OnClearSearchQuery;

            var assetGridLayout = m_AssetGridLayoutTemplate.Instantiate();
            assetGridLayout.style.height = Length.Percent(100);

            var assetInformationLayout = m_AssetInformationLayoutTemplate.Instantiate();
            assetInformationLayout.style.height = Length.Percent(100);

            HideAssetDiscoveryLayout();

            var assetGridContainer = m_UiDocumentRoot.Q<VisualElement>("ContentPanel");
            assetGridContainer.Add(assetGridLayout);
            assetGridContainer.Add(assetInformationLayout);

            // Init controllers
            var thumbnails = new Dictionary<AssetType, Texture2D>();
            foreach (var defaultThumbnail in m_DefaultThumbnails)
            {
                thumbnails.Add(defaultThumbnail.AssetType, defaultThumbnail.Thumbnail);
            }

            m_AssetsGridController.Init(assetGridLayout, m_AssetsGridItemTemplate, thumbnails);
            m_AssetInformationPanelController.Init(assetInformationLayout, GetFieldNameAsync, GetStatusFlowNameAsync);

            m_ProjectController.ShowContent += ShowAssetDiscoveryLayout;
            m_ProjectController.HideContent += HideAssetDiscoveryLayout;
            m_ProjectController.OrganizationSelected += OnOrganizationSelected;
            m_ProjectController.ProjectSelected += OnProjectSelected;

            m_AssetsGridController.AssetSelected += OnAssetSelected;

            Application.lowMemory += OnLowMemory;
            UnityEngine.Device.Application.lowMemory += OnLowMemory;
        }

        void OnDestroy()
        {
            CancelNewList();

            m_ProjectController.ShowContent -= ShowAssetDiscoveryLayout;
            m_ProjectController.HideContent -= HideAssetDiscoveryLayout;
            m_ProjectController.OrganizationSelected -= OnOrganizationSelected;
            m_ProjectController.ProjectSelected -= OnProjectSelected;

            m_SearchBarUi.DeleteSearchQuery -= OnSearchQueryChanged;
            m_SearchBarUi.AddSearchQuery -= OnSearchQueryChanged;
            m_SearchBarUi.ClearSearchQuery -= OnClearSearchQuery;
            m_AssetsGridController.AssetSelected -= OnAssetSelected;
        }

        void ShowAssetDiscoveryLayout()
        {
            m_AssetsGridController.DisplayAssetGrid();
            m_AssetInformationPanelController.DisplayInformationPanel();
        }

        void HideAssetDiscoveryLayout()
        {
            m_AssetsGridController.HideAssetGrid();
            m_AssetInformationPanelController.HideInformationPanel();

            ClearContent();
        }

        void OnAssetSelected(IAsset selectedAsset)
        {
            if (m_SelectedAsset == selectedAsset) return;

            m_SelectedAsset = selectedAsset;
            DisplayAssetInformationPanel();
        }

        void OnOrganizationSelected(OrganizationId organizationId)
        {
            ClearContent();
            PopulateOrganizationFields(organizationId);
        }

        void PopulateOrganizationFields(OrganizationId organizationId)
        {
            if (m_OrganizationId == organizationId) return;

            m_OrganizationId = organizationId;

            _ = PopulateFieldToName();
            _ = PopluateStatusFlowToName();
        }

        async Task PopulateFieldToName()
        {
            m_FieldToName = null;

            var dictionary = new Dictionary<string, string>();
            await foreach (var fieldDefinition in PlatformServices.AssetRepository.ListFieldDefinitionsAsync(m_OrganizationId, Range.All, default))
            {
                var properties = await fieldDefinition.GetPropertiesAsync(default);
                dictionary[fieldDefinition.Descriptor.FieldKey] = properties.DisplayName;
            }

            m_FieldToName = dictionary;
        }

        async Task<string> GetFieldNameAsync(string fieldKey)
        {
            while (m_StatusFlowToName == null)
            {
                await Task.Yield();
            }

            return m_FieldToName.GetValueOrDefault(fieldKey, fieldKey);
        }

        async Task PopluateStatusFlowToName()
        {
            m_StatusFlowToName = null;

            var dictionary = new Dictionary<string, string>();
            await foreach (var statusFlow in PlatformServices.AssetRepository.ListStatusFlowsAsync(m_OrganizationId, Range.All, default))
            {
                dictionary[statusFlow.Descriptor.StatusFlowId] = statusFlow.Name;
            }

            m_StatusFlowToName = dictionary;
        }

        async Task<string> GetStatusFlowNameAsync(string statusFlowId)
        {
            while (m_StatusFlowToName == null)
            {
                await Task.Yield();
            }

            return m_StatusFlowToName.GetValueOrDefault(statusFlowId, statusFlowId);
        }

        async void OnProjectSelected()
        {
            // On project selected, clear the content
            HideInformationRightPanel();

            m_ProjectAssetsList.Clear();

            if (m_ProjectController.IsAllProjectSelected)
            {
                m_SearchBarUi.DisplaySearchBar(m_ProjectController.SelectedOrganizationId);
            }
            else
            {
                m_SearchBarUi.DisplaySearchBar(m_ProjectController.SelectedProject);
            }

            CancelNewList();
            m_NewListCancellationTokenSource = new CancellationTokenSource();

            var newListToken = m_NewListCancellationTokenSource.Token;

            m_ProjectAssetsList.Clear();
            m_AssetsGridController.ClearAssetGrid();
            m_ContentPanel.style.display = DisplayStyle.Flex;

            var updateToken = m_SearchBarUi.GetSearchCancellationToken();

            IAsyncEnumerable<IAsset> assets = null;
            if (m_ProjectController.IsAllProjectSelected)
            {
                assets = m_ProjectController.GetAssetsAcrossAllProjectsAsync(m_ProjectController.SelectedOrganizationId, newListToken);
            }
            else if (m_ProjectController.SelectedProject != null)
            {
                assets = m_ProjectController.GetAssetsAsync(newListToken);
            }

            m_AssetsGridController.DisplayAssetGrid();
            var assetsList = new List<IAsset>();

            if (assets != null)
            {
                try
                {
                    var nextDisplayTrigger = 30;
                    await foreach (var asset in assets.WithCancellation(newListToken))
                    {
                        m_ProjectAssetsList.Add(asset);
                        assetsList.Add(asset);

                        if (m_ProjectAssetsList.Count > nextDisplayTrigger && !updateToken.IsCancellationRequested)
                        {
                            nextDisplayTrigger += k_RefreshIncrement;

                            m_AssetsGridController.PopulateAssetsGrid(assetsList);
                            assetsList.Clear();
                        }
                    }
                }
                catch (Exception e)
                {
                    e.LogException();
                }
            }

            if (newListToken.IsCancellationRequested) return;

            if (!updateToken.IsCancellationRequested)
            {
                m_AssetsGridController.PopulateAssetsGrid(assetsList);
            }
        }

        void DisplayAssetInformationPanel()
        {
            m_AssetInformationPanelController.DisplayInformationPanel();
            m_AssetInformationPanelController.PopulateAssetPanel(m_SelectedAsset);
            _ = m_AssetInformationPanelController.PopulateDatasetsPanel(m_SelectedAsset.ListDatasetsAsync(Range.All, CancellationToken.None));
        }

        void HideInformationRightPanel()
        {
            m_AssetInformationPanelController.HideInformationPanel();
        }

        void ClearContent()
        {
            m_ContentPanel.style.display = DisplayStyle.None;
            m_AssetsGridController.HideAssetGrid();
            m_AssetInformationPanelController.HideInformationPanel();
        }

        async void OnSearchQueryChanged(IAsyncEnumerable<IAsset> assets, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            m_AssetInformationPanelController.HideInformationPanel();

            m_AssetsGridController.ClearAssetGrid();
            m_AssetsGridController.DisplayAssetGrid();
            var assetList = new List<IAsset>();

            var nextDisplayTrigger = 30;
            try
            {
                await foreach (var asset in assets.WithCancellation(cancellationToken))
                {
                    assetList.Add(asset);

                    if (assetList.Count > nextDisplayTrigger)
                    {
                        nextDisplayTrigger += k_RefreshIncrement;
                        m_AssetsGridController.PopulateAssetsGrid(assetList);
                        assetList.Clear();
                    }
                }
            }
            catch (Exception e)
            {
                e.LogException();
            }

            // Attempt final refresh
            if (!cancellationToken.IsCancellationRequested)
            {
                m_AssetsGridController.PopulateAssetsGrid(assetList);
            }
        }

        void OnClearSearchQuery()
        {
            _ = m_SearchBarUi.GetSearchCancellationToken();

            m_AssetsGridController.ClearAssetGrid();
            m_AssetsGridController.DisplayAssetGrid();
            m_AssetsGridController.PopulateAssetsGrid(m_ProjectAssetsList);
        }

        void CancelNewList()
        {
            m_NewListCancellationTokenSource?.Cancel();
            m_NewListCancellationTokenSource?.Dispose();
            m_NewListCancellationTokenSource = null;
        }

        void OnLowMemory()
        {
            CancelNewList();
            _ = m_SearchBarUi.GetSearchCancellationToken();

            Resources.UnloadUnusedAssets();

            DialogService.ShowMessage("Low Memory", "The application is running low on memory. Some assets may not be displayed correctly.");
        }
    }
}
