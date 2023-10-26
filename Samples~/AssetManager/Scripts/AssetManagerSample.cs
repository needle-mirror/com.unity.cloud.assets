#if !UC_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetManager
{
    public class AssetManagerSample : MonoBehaviour
    {
        readonly AssetListController m_AssetListController = new();
        readonly AssetCreationController m_AssetCreationController = new();
        readonly DatasetCreationController m_DatasetCreationController = new();

        [SerializeField]
        UIDocument m_AssetManagerUiDocument;
        [SerializeField]
        UserController m_UserController;
        [SerializeField]
        SearchBarUi m_SearchBarUi;
        [SerializeField]
        DialogUi m_MessageDialogUi;
        [SerializeField]
        DialogUi m_AssetFilePathDialogUi;

        [SerializeField]
        VisualTreeAsset m_AssetCreationPanelTemplate;
        [SerializeField]
        VisualTreeAsset m_DatasetCreationTemplate;
        [SerializeField]
        VisualTreeAsset m_FileListItemTemplate;
        [SerializeField]
        VisualTreeAsset m_DatasetListItemTemplate;
        [SerializeField]
        VisualTreeAsset m_AssetListTemplate;
        [SerializeField]
        VisualTreeAsset m_AssetListItemTemplate;
        [SerializeField]
        VisualTreeAsset m_TagsTemplate;

        VisualElement m_AssetManagerUiDocumentRoot;
        VisualElement m_ContentPanel;
        VisualElement m_AssetCreationPanel;
        VisualElement m_DatasetCreationPanel;
        VisualElement m_AssetListPanel;

        readonly List<IAsset> m_ProjectAssetsList = new();

        CancellationTokenSource m_NewListCancellationTokenSource = new();
        CancellationTokenSource m_UpdateListCancellationTokenSource = new();
        OrganizationId m_OrganizationId;

        void Start()
        {
            if (m_AssetManagerUiDocument)
                m_AssetManagerUiDocumentRoot = m_AssetManagerUiDocument.rootVisualElement;

            var editingContainer = m_AssetManagerUiDocumentRoot.Q<VisualElement>("EditingContainer");
            var dialogContainer = m_AssetManagerUiDocumentRoot.Q<VisualElement>("DialogContainer");
            var searchBarPanel= m_AssetManagerUiDocumentRoot.Q<VisualElement>("SearchBarContainer");
            m_ContentPanel = m_AssetManagerUiDocumentRoot.Q<VisualElement>("ContentPanel");

            m_SearchBarUi.Initialize(m_AssetManagerUiDocumentRoot, searchBarPanel);
            m_SearchBarUi.DeleteSearchQuery += OnSearchQueryChanged;
            m_SearchBarUi.AddSearchQuery += OnSearchQueryChanged;
            m_SearchBarUi.ClearSearchQuery += OnClearSearchQuery;

            // Keep order to ensure correct display overlay
            InstantiateDatasetCreationPanel();
            InstantiateAssetCreationPanel();
            InstantiateAssetList();
            m_AssetFilePathDialogUi.Initialize(editingContainer, dialogContainer, "Asset File Path");
            m_MessageDialogUi.Initialize(editingContainer, dialogContainer, "Message");

            // Init controllers
            m_AssetListController.Init(m_AssetManagerUiDocumentRoot, m_AssetListItemTemplate);

            m_UserController.HideContent += HideContent;
            m_UserController.OrganizationSelected += OnOrganizationSelected;
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
            m_UserController.OrganizationSelected -= OnOrganizationSelected;
            m_UserController.ProjectSelected -= OnProjectSelected;

            m_AssetListController.AssetSelected -= OnAssetOpen;
            m_AssetListController.AssetCreated -= OnAssetCreated;
        }

        void InstantiateAssetCreationPanel()
        {
            m_AssetCreationPanel = m_AssetCreationPanelTemplate.Instantiate();
            m_AssetCreationPanel.style.height = Length.Percent(100);
            m_AssetCreationPanel.style.width = Length.Percent(100);
            HideElement(m_AssetCreationPanel);

            m_ContentPanel.Add(m_AssetCreationPanel);

            m_AssetCreationController.Init
            (
                m_AssetCreationPanel,
                m_DatasetCreationController,
                m_DatasetListItemTemplate,
                m_TagsTemplate,
                RefreshAsset,
                m_MessageDialogUi.dialogController
            );

            m_AssetCreationPanel.Q<Button>("BackBtn").RegisterCallback<ClickEvent>(_ =>
            {
                HideElement(m_AssetCreationPanel);
                DisplayElement(m_AssetListPanel);
            });
        }

        void InstantiateDatasetCreationPanel()
        {
            m_DatasetCreationPanel = m_DatasetCreationTemplate.Instantiate();
            m_DatasetCreationPanel.style.height = Length.Percent(100);
            m_DatasetCreationPanel.style.width = Length.Percent(100);
            HideElement(m_DatasetCreationPanel);

            m_ContentPanel.Add(m_DatasetCreationPanel);

            m_DatasetCreationController.Init
            (
                m_DatasetCreationPanel,
                m_FileListItemTemplate,
                m_TagsTemplate,
                m_MessageDialogUi.dialogController,
                m_AssetFilePathDialogUi.dialogController
            );
        }

        void InstantiateAssetList()
        {
            m_AssetListPanel = m_AssetListTemplate.Instantiate();
            m_AssetListPanel.style.height = Length.Percent(100);
            m_AssetListPanel.style.width = Length.Percent(100);
            HideElement(m_AssetListPanel);

            m_ContentPanel.Add(m_AssetListPanel);
        }

        async void OnProjectSelected()
        {
            // Handle 'All' selection
            if (m_UserController.IsAllProjectSelected) return;

            m_SearchBarUi.DisplaySearchBar(m_UserController.SelectedProject);

            m_NewListCancellationTokenSource.Cancel();
            m_NewListCancellationTokenSource.Dispose();
            m_NewListCancellationTokenSource = new CancellationTokenSource();

            var newListToken = m_NewListCancellationTokenSource.Token;

            m_ProjectAssetsList.Clear();
            RefreshAssetList(m_ProjectAssetsList);
            m_ContentPanel.style.display = DisplayStyle.Flex;

            if(m_UserController.SelectedProject == null) return;

            var token = GetCancellationToken();

            var assets = m_UserController.GetAssetsAsync(newListToken);
            await foreach (var asset in assets.WithCancellation(token))
            {
                m_ProjectAssetsList.Add(asset);
            }

            if (!token.IsCancellationRequested)
            {
                RefreshAssetList(m_ProjectAssetsList);
            }
        }

        void OnOrganizationSelected(OrganizationId orgId)
        {
            m_OrganizationId = orgId;
            HideContent();
        }

        void HideContent()
        {
            m_ContentPanel.style.display = DisplayStyle.None;
            HideElement(m_AssetListPanel);
            HideElement(m_AssetCreationPanel);
            HideElement(m_DatasetCreationPanel);
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
            HideElement(m_DatasetCreationPanel);
            HideElement(m_AssetCreationPanel);

            m_AssetListController.ClearAssetList();
            m_AssetListController.PopulateAssetsList(assetsList);
            DisplayElement(m_AssetListPanel);
        }

        void OnAssetOpen(IAsset asset)
        {
            HideElement(m_AssetListPanel);
            HideElement(m_DatasetCreationPanel);
            m_AssetCreationController.OpenAsset(asset);
            DisplayElement(m_AssetCreationPanel);
        }

        void OnAssetCreated()
        {
            HideElement(m_AssetListPanel);
            HideElement(m_AssetCreationPanel);
            m_AssetCreationController.CreateNewAsset(m_UserController.SelectedProject);
        }

        async Task<IAsset> RefreshAsset(IAsset asset)
        {
            return await m_UserController.SelectedProject.GetAssetAsync(asset.Descriptor.AssetId, asset.Descriptor.AssetVersion, null, CancellationToken.None);
        }

        static void DisplayElement(VisualElement element)
        {
            if(element == null)
                return;

            element.style.display = DisplayStyle.Flex;
        }

        static void HideElement(VisualElement element)
        {
            if(element == null)
                return;

            element.style.display = DisplayStyle.None;
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
