#if !UC_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetManager
{
    public class AssetManagerSample : MonoBehaviour
    {
        readonly AssetListController m_AssetListController = new();
        readonly AssetCreationController m_AssetCreationController = new();

        [SerializeField]
        UIDocument m_AssetManagerUiDocument;
        [SerializeField]
        UserController m_UserController;
        [SerializeField]
        SearchBarUi m_SearchBarUi;
        [SerializeField]
        VisualTreeAsset m_AssetCreationPanelTemplate;
        [SerializeField]
        VisualTreeAsset m_AssetCreationPanelItemTemplate;
        [SerializeField]
        VisualTreeAsset m_AssetListTemplate;
        [SerializeField]
        VisualTreeAsset m_AssetListItemTemplate;
        [SerializeField]
        VisualTreeAsset m_TagsTemplate;

        VisualElement m_AssetDiscoveryUiDocumentRoot;
        VisualElement m_SampleContainer;
        VisualElement m_AssetCreationPanel;
        VisualElement m_AssetListPanel;

        readonly List<IAsset> m_ProjectAssetsList = new();

        CancellationTokenSource m_NewListCancellationTokenSource = new();
        CancellationTokenSource m_UpdateListCancellationTokenSource = new();

        void Start()
        {
            if (m_AssetManagerUiDocument)
                m_AssetDiscoveryUiDocumentRoot = m_AssetManagerUiDocument.rootVisualElement;

            m_SampleContainer = m_AssetDiscoveryUiDocumentRoot.Q<VisualElement>("ContentPanel");

            m_SearchBarUi.Initialize(m_AssetDiscoveryUiDocumentRoot, m_SampleContainer);
            m_SearchBarUi.DeleteSearchQuery += OnSearchQueryChanged;
            m_SearchBarUi.AddSearchQuery += OnSearchQueryChanged;
            m_SearchBarUi.ClearSearchQuery += OnClearSearchQuery;

            InstantiateAssetCreationPanel();
            InstantiateAssetList();

            // Init controllers
            m_AssetListController.Init(m_AssetDiscoveryUiDocumentRoot, m_AssetListItemTemplate);

            m_UserController.HideContent += HideContent;
            m_UserController.OrganizationSelected += HideContent;
            m_UserController.ProjectSelected += OnProjectSelected;

            m_AssetListController.AssetSelected += OnAssetOpen;
            m_AssetListController.AssetCreated += OnAssetCreated;
        }

        void OnDestroy()
        {
            m_SearchBarUi.DeleteSearchQuery -= OnSearchQueryChanged;
            m_SearchBarUi.AddSearchQuery -= OnSearchQueryChanged;
            m_SearchBarUi.ClearSearchQuery -= OnClearSearchQuery;

            m_UserController.HideContent -= HideContent;
            m_UserController.OrganizationSelected -= HideContent;
            m_UserController.ProjectSelected -= OnProjectSelected;

            m_AssetListController.AssetSelected -= OnAssetOpen;
            m_AssetListController.AssetCreated -= OnAssetCreated;
        }

        void InstantiateAssetCreationPanel()
        {
            m_AssetCreationPanel = m_AssetCreationPanelTemplate.Instantiate();
            m_AssetCreationPanel.style.height = Length.Percent(100);
            HideAssetCreationPanel();
            m_SampleContainer.Add(m_AssetCreationPanel);
        }

        void InstantiateAssetList()
        {
            m_AssetListPanel = m_AssetListTemplate.Instantiate();
            m_AssetListPanel.style.height = Length.Percent(100);
            HideAssetListPanel();
            m_SampleContainer.Add(m_AssetListPanel);
        }

        async void OnProjectSelected()
        {
            m_SearchBarUi.DisplaySearchBar(m_UserController.SelectedProject);

            m_NewListCancellationTokenSource.Cancel();
            m_NewListCancellationTokenSource.Dispose();
            m_NewListCancellationTokenSource = new CancellationTokenSource();

            m_ProjectAssetsList.Clear();
            RefreshAssetList(m_ProjectAssetsList);
            m_SampleContainer.style.display = DisplayStyle.Flex;

            var token = GetCancellationToken();

            var assets = m_UserController.GetAssetsAsync();
            await foreach (var asset in assets.WithCancellation(m_NewListCancellationTokenSource.Token))
            {
                m_ProjectAssetsList.Add(asset);
            }

            if (!token.IsCancellationRequested)
            {
                RefreshAssetList(m_ProjectAssetsList);
            }
        }

        void HideContent()
        {
            m_SampleContainer.style.display = DisplayStyle.None;
            HideAssetListPanel();
            HideAssetCreationPanel();
        }

        async void OnSearchQueryChanged(IAsyncEnumerable<IAsset> assets)
        {
            var token = GetCancellationToken();

            var startTime = DateTime.UtcNow;
            var assetList = new List<IAsset>();
            await foreach (var asset in assets.WithCancellation(token))
            {
                assetList.Add(asset);

                if (DateTime.UtcNow - startTime > TimeSpan.FromSeconds(0.6f))
                {
                    startTime = DateTime.UtcNow;
                    RefreshAssetList(assetList);
                }
            }

            // Attempt final refresh
            if (!token.IsCancellationRequested)
            {
                RefreshAssetList(assetList);
            }
        }

        void OnClearSearchQuery()
        {
            _ = GetCancellationToken();

            RefreshAssetList(m_ProjectAssetsList);
        }

        void RefreshAssetList(List<IAsset> assetsList)
        {
            HideAssetCreationPanel();

            m_AssetListController.ClearAssetList();
            m_AssetListController.PopulateAssetsList(assetsList);
            DisplayAssetListPanel();
        }

        void OnAssetOpen(IAsset asset)
        {
            m_AssetCreationController.Init(m_AssetCreationPanel, m_AssetCreationPanelItemTemplate, m_TagsTemplate);

            HideAssetListPanel();
            m_AssetCreationController.OpenExistingAsset(asset);
            DisplayAssetCreationPanel();
        }

        void OnAssetCreated()
        {
            m_AssetCreationController.Init(m_AssetCreationPanel, m_AssetCreationPanelItemTemplate, m_TagsTemplate);

            HideAssetListPanel();
            m_AssetCreationController.CreateNewAsset();
            DisplayAssetCreationPanel();
        }

        void DisplayAssetCreationPanel()
        {
            m_AssetCreationPanel.style.display = DisplayStyle.Flex;
        }

        void HideAssetCreationPanel()
        {
            m_AssetCreationPanel.style.display = DisplayStyle.None;
        }

        void DisplayAssetListPanel()
        {
            m_AssetListPanel.style.display = DisplayStyle.Flex;
        }

        void HideAssetListPanel()
        {
            m_AssetListPanel.style.display = DisplayStyle.None;
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
