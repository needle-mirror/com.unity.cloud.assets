using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetManager
{
    public class AssetController : ProjectController
    {
        readonly AssetListUi m_AssetListUi = new();

        [SerializeField]
        VisualTreeAsset m_AssetListTemplate;
        [SerializeField]
        VisualTreeAsset m_AssetListItemTemplate;
        [SerializeField]
        SearchBarUi m_SearchBarUi;

        readonly List<IAsset> m_ProjectAssetsList = new();

        CancellationTokenSource m_NewListCancellationTokenSource = new();

        public event Action<IAsset> AssetSelected
        {
            add => m_AssetListUi.AssetSelected += value;
            remove => m_AssetListUi.AssetSelected -= value;
        }

        public event Action CreateAsset;

        public VisualElement AssetListPanel { get; private set; }

        protected override void Start()
        {
            base.Start();

            var searchBarPanel = RootVisualElement.Q<VisualElement>("SearchBarContainer");

            m_SearchBarUi.Initialize(RootVisualElement, searchBarPanel);
            m_SearchBarUi.DeleteSearchQuery += OnSearchQueryChanged;
            m_SearchBarUi.AddSearchQuery += OnSearchQueryChanged;
            m_SearchBarUi.ClearSearchQuery += OnClearSearchQuery;

            AssetListPanel = m_AssetListTemplate.Instantiate();
            AssetListPanel.style.height = Length.Percent(100);
            AssetListPanel.style.width = Length.Percent(100);
            AssetListPanel.style.display = DisplayStyle.None;

            var contentPanel = RootVisualElement.Q<VisualElement>("ContentPanel");
            contentPanel.Add(AssetListPanel);

            var addButton = AssetListPanel.Q<Button>("AddAssetButton");
            addButton.RegisterCallback<ClickEvent>(_ => CreateAsset?.Invoke());

            m_AssetListUi.Initialize(AssetListPanel, m_AssetListItemTemplate);

            ProjectSelected += OnProjectSelected;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            m_SearchBarUi.DeleteSearchQuery -= OnSearchQueryChanged;
            m_SearchBarUi.AddSearchQuery -= OnSearchQueryChanged;
            m_SearchBarUi.ClearSearchQuery -= OnClearSearchQuery;

            ProjectSelected -= OnProjectSelected;
        }

        public void ClearSelection()
        {
            m_AssetListUi.ClearSelection();
        }

        public async void OnAssetCreated(IAsset asset)
        {
            await OnProjectSelectedAsync();
            m_AssetListUi.SelectAsset(asset);
        }

        public void OnAssetUpdated(IAsset asset)
        {
            m_AssetListUi.UpdateAsset(asset);

            if (IsAllProjectSelected)
            {
                m_SearchBarUi.UpdateSearchBarValues(AssetRepository, GetAllProjects().Select(x => x.Descriptor));
            }
            else if (SelectedProject != null)
            {
                m_SearchBarUi.UpdateSearchBarValues(SelectedProject);
            }
        }

        async void OnProjectSelected()
        {
             await OnProjectSelectedAsync();
        }

        async Task OnProjectSelectedAsync()
        {
            // Handle 'All' selection
            if (IsAllProjectSelected) return;

            m_SearchBarUi.DisplaySearchBar(SelectedProject);

            m_NewListCancellationTokenSource.Cancel();
            m_NewListCancellationTokenSource.Dispose();
            m_NewListCancellationTokenSource = new CancellationTokenSource();

            var newListToken = m_NewListCancellationTokenSource.Token;

            m_ProjectAssetsList.Clear();
            RefreshAssetList(m_ProjectAssetsList);

            if (SelectedProject == null) return;

            var token = m_SearchBarUi.GetSearchCancellationToken();

            var assets = GetAssetsAsync(newListToken);

            var nextDisplayTrigger = 40;
            var assetList = new List<IAsset>();
            await foreach (var asset in assets.WithCancellation(token))
            {
                m_ProjectAssetsList.Add(asset);
                assetList.Add(asset);

                if (m_ProjectAssetsList.Count > nextDisplayTrigger)
                {
                    nextDisplayTrigger *= 2;

                    m_AssetListUi.PopulateAssetsList(assetList);
                    assetList.Clear();
                }
            }

            if (!token.IsCancellationRequested)
            {
                m_AssetListUi.PopulateAssetsList(assetList);
            }
        }

        async void OnSearchQueryChanged(IAsyncEnumerable<IAsset> assets, CancellationToken cancellationToken)
        {
            var assetList = new List<IAsset>();
            RefreshAssetList(assetList);

            var nextDisplayTrigger = 40;
            await foreach (var asset in assets.WithCancellation(cancellationToken))
            {
                assetList.Add(asset);

                if (assetList.Count > nextDisplayTrigger)
                {
                    nextDisplayTrigger *= 2;

                    m_AssetListUi.PopulateAssetsList(assetList);
                    assetList.Clear();
                }
            }

            // Attempt final refresh
            if (!cancellationToken.IsCancellationRequested)
            {
                m_AssetListUi.PopulateAssetsList(assetList);
            }
        }

        void OnClearSearchQuery()
        {
            _ = m_SearchBarUi.GetSearchCancellationToken();

            RefreshAssetList(m_ProjectAssetsList);
        }

        void RefreshAssetList(IEnumerable<IAsset> assetsList)
        {
            m_AssetListUi.ClearAssetList();
            m_AssetListUi.PopulateAssetsList(assetsList);
        }
    }
}
