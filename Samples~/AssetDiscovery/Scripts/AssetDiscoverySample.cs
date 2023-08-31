#if !UC_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        VisualTreeAsset m_AssetDiscoveryLayoutTemplate;
        [SerializeField]
        VisualTreeAsset m_AssetsGridItemTemplate;
        [SerializeField]
        VisualTreeAsset m_AssetInformationPanelItemTemplate;
        [SerializeField]
        VisualTreeAsset m_AssetInformationTagsTemplate;

        VisualElement m_UiDocumentRoot;
        VisualElement m_SampleContainer;
        VisualElement m_AssetDiscoveryLayout;

        AssetsGridController m_AssetsGridController;
        AssetInformationPanelController m_AssetInformationPanelController;

        readonly List<IAsset> m_ProjectAssetsList = new();

        IAsset m_SelectedAsset;

        CancellationTokenSource m_NewListCancellationTokenSource = new();
        CancellationTokenSource m_UpdateListCancellationTokenSource = new();

        void Start()
        {
            if (m_UiDocument)
                m_UiDocumentRoot = m_UiDocument.rootVisualElement;

            m_AssetsGridController = new AssetsGridController();
            m_AssetInformationPanelController = new AssetInformationPanelController();

            m_SampleContainer = m_UiDocumentRoot.Q<VisualElement>("ContentPanel");

            InstantiateDiscoveryLayout();

            // Init controllers
            m_AssetsGridController.Init(m_AssetDiscoveryLayout, m_AssetsGridItemTemplate, this);
            m_AssetInformationPanelController.Init(m_AssetDiscoveryLayout, m_AssetInformationPanelItemTemplate, m_AssetInformationTagsTemplate, this);

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

        void InstantiateDiscoveryLayout()
        {
            m_SearchBarUi.Initialize(m_UiDocumentRoot, m_SampleContainer);
            m_SearchBarUi.DeleteSearchQuery += OnSearchQueryChanged;
            m_SearchBarUi.AddSearchQuery += OnSearchQueryChanged;
            m_SearchBarUi.ClearSearchQuery += OnClearSearchQuery;

            m_AssetDiscoveryLayout = m_AssetDiscoveryLayoutTemplate.Instantiate();
            m_AssetDiscoveryLayout.style.height = Length.Percent(100);
            HideAssetDiscoveryLayout();

            m_SampleContainer.Add(m_AssetDiscoveryLayout);
        }

        void ShowAssetDiscoveryLayout()
        {
            m_AssetDiscoveryLayout.style.display = DisplayStyle.Flex;
        }

        void HideAssetDiscoveryLayout()
        {
            m_AssetDiscoveryLayout.style.display = DisplayStyle.None;
            ClearContent();
        }

        void OnAssetSelected(IAsset selectedAsset)
        {
            if (m_SelectedAsset == selectedAsset) return;

            m_SelectedAsset = selectedAsset;
            Debug.Log($"Asset Selected: {m_SelectedAsset.Name}");
            DisplayAssetInformationPanel();
        }

        void OnAssetsListChanged(List<IAsset> assetsList)
        {
            m_AssetsGridController.PopulateAssetsGrid(assetsList);
            m_AssetsGridController.DisplayAssetGrid();
        }

        void OnOrganizationSelected()
        {
            ClearContent();
        }

        async void OnProjectSelected()
        {
            // On project selected, clear the content
            HideAssetInformationPanel();

            if (m_UserController.IsAllProjectSelected)
            {
                m_SearchBarUi.DisplaySearchBar(m_UserController.SelectedOrganization, m_UserController.GetAllProjects());
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
            m_SampleContainer.style.display = DisplayStyle.Flex;

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
            m_AssetInformationPanelController.DisplayAssetInformationPanel();
            m_AssetInformationPanelController.SetAssetPanelName(m_SelectedAsset);

            m_AssetInformationPanelController.PopulateAssetPanel(m_SelectedAsset);
        }

        void HideAssetInformationPanel()
        {
            m_AssetInformationPanelController.HideAssetInformationPanel();
        }

        void ClearContent()
        {
            m_SampleContainer.style.display = DisplayStyle.None;
            m_AssetsGridController.HideAssetGrid();
            m_AssetInformationPanelController.HideAssetInformationPanel();
        }

        async void OnSearchQueryChanged(IAsyncEnumerable<IAsset> assets)
        {
            m_AssetInformationPanelController.HideAssetInformationPanel();

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
