#if !UC_EXCLUDE_SAMPLES
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
        UserController m_UserController;

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

            m_SearchBarUi.Initialize(m_UiDocumentRoot, m_UiDocumentRoot.Q<VisualElement>("SearchBarContainer"));
            m_SearchBarUi.DeleteSearchQuery += OnSearchQueryChanged;
            m_SearchBarUi.AddSearchQuery += OnSearchQueryChanged;
            m_SearchBarUi.ClearSearchQuery += OnClearSearchQuery;

            var assetGridLayout = m_AssetGridLayoutTemplate.Instantiate();
            assetGridLayout.style.height = Length.Percent(100);

            var assetInformationLayout = m_AssetInformationLayoutTemplate.Instantiate();
            assetInformationLayout.style.height = Length.Percent(100);

            HideAssetDiscoveryLayout();

            var assetGridContainer =  m_UiDocumentRoot.Q<VisualElement>("ContentPanel");
            assetGridContainer.Add(assetGridLayout);
            assetGridContainer.Add(assetInformationLayout);

            // Init controllers
            var thumbnails = new Dictionary<AssetType, Texture2D>();
            foreach(var defaultThumbnail in m_DefaultThumbnails)
            {
                thumbnails.Add(defaultThumbnail.AssetType, defaultThumbnail.Thumbnail);
            }

            m_AssetsGridController.Init(assetGridLayout, m_AssetsGridItemTemplate, thumbnails);
            m_AssetInformationPanelController.Init(assetInformationLayout, m_InformationItemTemplate, m_InformationTagsTemplate, m_DataSetInformationPanelItemTemplate, this);

            m_UserController.ShowContent += ShowAssetDiscoveryLayout;
            m_UserController.HideContent += HideAssetDiscoveryLayout;
            m_UserController.OrganizationSelected += OnOrganizationSelected;
            m_UserController.ProjectSelected += OnProjectSelected;

            m_AssetsGridController.AssetSelected += OnAssetSelected;
        }

        void OnDestroy()
        {
            m_UserController.ShowContent -= ShowAssetDiscoveryLayout;
            m_UserController.HideContent -= HideAssetDiscoveryLayout;
            m_UserController.OrganizationSelected -= OnOrganizationSelected;
            m_UserController.ProjectSelected -= OnProjectSelected;

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

        void OnAssetsListChanged(List<IAsset> assetsList)
        {
            m_AssetsGridController.PopulateAssetsGrid(assetsList);
            m_AssetsGridController.DisplayAssetGrid();
        }

        void OnOrganizationSelected(OrganizationId orgId)
        {
            ClearContent();
        }

        async void OnProjectSelected()
        {
            // On project selected, clear the content
            HideInformationRightPanel();

            if (m_UserController.IsAllProjectSelected)
            {
                m_SearchBarUi.DisplaySearchBar(m_UserController.AssetRepository, m_UserController.SelectedOrganization.Id, m_UserController.GetAllProjects());
            }
            else
            {
                m_SearchBarUi.DisplaySearchBar(m_UserController.SelectedProject);
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
            if (m_UserController.IsAllProjectSelected)
            {
                assets = m_UserController.GetAssetsAcrossAllProjectsAsync(newListToken);
            }
            else if (m_UserController.SelectedProject != null)
            {
                assets = m_UserController.GetAssetsAsync(newListToken);
            }

            if (assets != null)
            {
                await foreach (var asset in assets.WithCancellation(newListToken))
                {
                    m_ProjectAssetsList.Add(asset);
                }
            }

            if (!updateToken.IsCancellationRequested)
            {
                OnAssetsListChanged(m_ProjectAssetsList);
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

            var startTime = DateTime.UtcNow;
            var assetList = new List<IAsset>();
            await foreach (var asset in assets.WithCancellation(token))
            {
                assetList.Add(asset);

                if (DateTime.UtcNow - startTime > TimeSpan.FromSeconds(0.6f))
                {
                    startTime = DateTime.UtcNow;
                    OnAssetsListChanged(assetList);
                }
            }

            // Attempt final refresh
            if (!token.IsCancellationRequested)
            {
                OnAssetsListChanged(assetList);
            }
        }

        void OnClearSearchQuery()
        {
            _ = GetCancellationToken();

            OnAssetsListChanged(m_ProjectAssetsList);
        }

        CancellationToken GetCancellationToken()
        {
            m_UpdateListCancellationTokenSource.Cancel();
            m_UpdateListCancellationTokenSource.Dispose();

            m_UpdateListCancellationTokenSource = new CancellationTokenSource();
            return m_UpdateListCancellationTokenSource.Token;
        }
    }
}
#endif
