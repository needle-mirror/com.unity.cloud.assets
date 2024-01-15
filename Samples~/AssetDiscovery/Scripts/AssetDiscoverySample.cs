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
        VisualTreeAsset m_InformationItemTemplate;

        [SerializeField]
        VisualTreeAsset m_InformationTagsTemplate;

        [SerializeField]
        VisualTreeAsset m_DataSetInformationPanelItemTemplate;

        [SerializeField]
        DefaultThumbnail[] m_DefaultThumbnails;

        VisualElement m_UiDocumentRoot;
        VisualElement m_ContentPanel;

        IAssetsGridController m_AssetsGridController;
        IAssetInformationPanelController m_AssetInformationPanelController;

        readonly List<IAsset> m_ProjectAssetsList = new();

        IAsset m_SelectedAsset;

        CancellationTokenSource m_NewListCancellationTokenSource = new();
        CancellationTokenSource m_UpdateListCancellationTokenSource = new();

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
            m_AssetInformationPanelController = new AssetInformationPanelController();

            m_ContentPanel = m_UiDocumentRoot.Q<VisualElement>("Content");

            m_SearchBarUi.FieldsToInclude = m_ProjectController.FieldsToInclude;
            m_SearchBarUi.Initialize(m_UiDocumentRoot, m_UiDocumentRoot.Q<VisualElement>("SearchBarContainer"));
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
            m_AssetInformationPanelController.Init(assetInformationLayout, m_InformationItemTemplate, m_InformationTagsTemplate, m_DataSetInformationPanelItemTemplate, this);

            m_ProjectController.ShowContent += ShowAssetDiscoveryLayout;
            m_ProjectController.HideContent += HideAssetDiscoveryLayout;
            m_ProjectController.OrganizationSelected += OnOrganizationSelected;
            m_ProjectController.ProjectSelected += OnProjectSelected;

            m_AssetsGridController.AssetSelected += OnAssetSelected;
        }

        void OnDestroy()
        {
            CancelUpdate();
            m_NewListCancellationTokenSource.Cancel();
            m_NewListCancellationTokenSource.Dispose();

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

        void OnOrganizationSelected(OrganizationId orgId)
        {
            ClearContent();
        }

        async void OnProjectSelected()
        {
            // On project selected, clear the content
            HideInformationRightPanel();

            if (m_ProjectController.IsAllProjectSelected)
            {
                m_SearchBarUi.DisplaySearchBar(m_ProjectController.AssetRepository, m_ProjectController.SelectedOrganizationId, m_ProjectController.GetAllProjects());
            }
            else
            {
                m_SearchBarUi.DisplaySearchBar(m_ProjectController.SelectedProject);
            }

            m_NewListCancellationTokenSource.Cancel();
            m_NewListCancellationTokenSource.Dispose();
            m_NewListCancellationTokenSource = new CancellationTokenSource();

            var newListToken = m_NewListCancellationTokenSource.Token;

            m_ProjectAssetsList.Clear();
            m_AssetsGridController.ClearAssetGrid();
            m_ContentPanel.style.display = DisplayStyle.Flex;

            var updateToken = GetCancellationToken();

            IAsyncEnumerable<IAsset> assets = null;
            if (m_ProjectController.IsAllProjectSelected)
            {
                assets = m_ProjectController.GetAssetsAcrossAllProjectsAsync(newListToken);
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
                    var nextDisplayTrigger = 40;
                    await foreach (var asset in assets.WithCancellation(newListToken))
                    {
                        m_ProjectAssetsList.Add(asset);
                        assetsList.Add(asset);

                        if (m_ProjectAssetsList.Count > nextDisplayTrigger && !updateToken.IsCancellationRequested)
                        {
                            nextDisplayTrigger *= 2;

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

        async void OnSearchQueryChanged(IAsyncEnumerable<IAsset> assets)
        {
            m_AssetInformationPanelController.HideInformationPanel();

            var token = GetCancellationToken();

            m_AssetsGridController.ClearAssetGrid();
            m_AssetsGridController.DisplayAssetGrid();
            var assetList = new List<IAsset>();

            var nextDisplayTrigger = 40;
            await foreach (var asset in assets.WithCancellation(token))
            {
                assetList.Add(asset);

                if (assetList.Count > nextDisplayTrigger)
                {
                    nextDisplayTrigger *= 2;
                    m_AssetsGridController.PopulateAssetsGrid(assetList);
                    assetList.Clear();
                }
            }

            // Attempt final refresh
            if (!token.IsCancellationRequested)
            {
                m_AssetsGridController.PopulateAssetsGrid(assetList);
            }
        }

        void OnClearSearchQuery()
        {
            CancelUpdate();

            m_AssetsGridController.ClearAssetGrid();
            m_AssetsGridController.DisplayAssetGrid();
            m_AssetsGridController.PopulateAssetsGrid(m_ProjectAssetsList);
        }

        void CancelUpdate()
        {
            if (m_UpdateListCancellationTokenSource != null)
            {
                m_UpdateListCancellationTokenSource.Cancel();
                m_UpdateListCancellationTokenSource.Dispose();
            }

            m_UpdateListCancellationTokenSource = null;
        }

        CancellationToken GetCancellationToken()
        {
            CancelUpdate();

            m_UpdateListCancellationTokenSource = new CancellationTokenSource();
            return m_UpdateListCancellationTokenSource.Token;
        }
    }
}
