using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;
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
        readonly List<ITrashedAsset> m_ProjectTrashedAssetsList = new();

        CancellationTokenSource m_NewListCancellationTokenSource = new();
        bool m_IsViewingTrash = false;

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

            m_SearchBarUi.Initialize(RootVisualElement, searchBarPanel, AssetRepository);
            m_SearchBarUi.DeleteSearchQuery += OnSearchQueryChanged;
            m_SearchBarUi.AddSearchQuery += OnSearchQueryChanged;
            m_SearchBarUi.ClearSearchQuery += OnClearSearchQuery;

            AssetListPanel = m_AssetListTemplate.Instantiate();
            AssetListPanel.style.flexGrow = 1;
            AssetListPanel?.Hide();

            var contentPanel = RootVisualElement.Q<VisualElement>("ContentPanel");
            contentPanel.Add(AssetListPanel);

            var addButton = AssetListPanel.Q<Button>("AddAssetButton");
            addButton.RegisterCallback<ClickEvent>(_ => CreateAsset?.Invoke());

            var viewTrashButton = AssetListPanel.Q<Button>("ViewTrashButton");
            viewTrashButton.RegisterCallback<ClickEvent>(_ => OnViewTrashButtonClicked());

            m_AssetListUi.Initialize(AssetListPanel, MakeItem);
            m_AssetListUi.RemoveAsset += OnTrashAsset;
            m_AssetListUi.RestoreAsset += OnRestoreAsset;
            m_AssetListUi.DeletePermanentlyAsset += OnDeletePermanentlyAsset;

            ProjectSelected += OnProjectSelected;

            Application.lowMemory += OnLowMemory;
        }

        VisualElement MakeItem()
        {
            var element = m_AssetListItemTemplate.Instantiate();

            element.Q("LeftTopPanel").RegisterCallback<MouseOverEvent>(_ =>
            {
                element.Q("LeftTopPanel").style.backgroundColor = new Color(0.14f, 0.14f, 0.14f, 1f);
                element.Q("RightPanel").style.backgroundColor = new Color(0.19f, 0.19f, 0.19f, 1f);
            });

            element.Q("LeftTopPanel").RegisterCallback<MouseOutEvent>(_ =>
            {
                element.Q("LeftTopPanel").style.backgroundColor = new Color(0.07f, 0.07f, 0.07f, 1f);
                element.Q("RightPanel").style.backgroundColor = new Color(0.14f, 0.14f, 0.14f, 1f);
            });

            return element;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            m_SearchBarUi.DeleteSearchQuery -= OnSearchQueryChanged;
            m_SearchBarUi.AddSearchQuery -= OnSearchQueryChanged;
            m_SearchBarUi.ClearSearchQuery -= OnClearSearchQuery;

            m_AssetListUi.RemoveAsset -= OnTrashAsset;
            m_AssetListUi.RestoreAsset -= OnRestoreAsset;
            m_AssetListUi.DeletePermanentlyAsset -= OnDeletePermanentlyAsset;

            ProjectSelected -= OnProjectSelected;
        }

        public void OnBackButtonClicked(ClickEvent evt)
        {
            AssetListPanel?.Show();

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
            m_SearchBarUi.UpdateSearchBarValues();
        }

        async void OnProjectSelected()
        {
            m_AssetListUi.ClearSelection();
            m_IsViewingTrash = false;
            UpdateTrashViewUI();

            await OnProjectSelectedAsync();
        }

        async Task OnProjectSelectedAsync()
        {
            // Handle 'All' selection
            if (IsAllProjectSelected) return;

            var newListToken = GetNewListToken();

            m_ProjectAssetsList.Clear();
            m_ProjectTrashedAssetsList.Clear();

            if (!m_IsViewingTrash)
            {
                m_SearchBarUi.DisplaySearchBar(SelectedProject);
            }

            if (SelectedProject == null) return;

            var updateToken = m_SearchBarUi.GetSearchCancellationToken();

            if (m_IsViewingTrash)
            {
                var nextDisplayTrigger = 40;
                try
                {
                    await foreach (var trashed in GetTrashedAssetsAsync(newListToken))
                    {
                        m_ProjectTrashedAssetsList.Add(trashed);

                        if (m_ProjectTrashedAssetsList.Count > nextDisplayTrigger && !updateToken.IsCancellationRequested)
                        {
                            nextDisplayTrigger *= 2;
                            m_AssetListUi.PopulateTrashedAssetsList(m_ProjectTrashedAssetsList);
                        }
                    }
                }
                catch (Exception e)
                {
                    e.LogException();
                }

                if (!updateToken.IsCancellationRequested)
                    m_AssetListUi.PopulateTrashedAssetsList(m_ProjectTrashedAssetsList);
            }
            else
            {
                var nextDisplayTrigger = 40;
                var assetList = new List<IAsset>();
                try
                {
                    await foreach (var asset in GetAssetsAsync(newListToken))
                    {
                        m_ProjectAssetsList.Add(asset);
                        assetList.Add(asset);

                        if (m_ProjectAssetsList.Count > nextDisplayTrigger && !updateToken.IsCancellationRequested)
                        {
                            nextDisplayTrigger *= 2;
                            m_AssetListUi.PopulateAssetsList(assetList);
                        }
                    }
                }
                catch (Exception e)
                {
                    e.LogException();
                }

                if (!updateToken.IsCancellationRequested)
                    m_AssetListUi.PopulateAssetsList(m_ProjectAssetsList);
            }
        }

        async void OnSearchQueryChanged(IAsyncEnumerable<IAsset> assets, CancellationToken cancellationToken)
        {
            var assetList = new List<IAsset>();
            RefreshAssetList(assetList);

            var nextDisplayTrigger = 40;
            try
            {
                await foreach (var asset in assets.WithCancellation(cancellationToken))
                {
                    assetList.Add(asset);

                    if (assetList.Count > nextDisplayTrigger)
                    {
                        nextDisplayTrigger *= 2;

                        m_AssetListUi.PopulateAssetsList(assetList);
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
                m_AssetListUi.PopulateAssetsList(assetList);
            }
        }

        void OnClearSearchQuery()
        {
            _ = m_SearchBarUi.GetSearchCancellationToken();

            if (m_IsViewingTrash)
            {
                m_AssetListUi.ClearAssetList();
                m_AssetListUi.PopulateTrashedAssetsList(m_ProjectTrashedAssetsList);
            }
            else
            {
                RefreshAssetList(m_ProjectAssetsList);
            }
        }

        void RefreshAssetList(IEnumerable<IAsset> assetsList)
        {
            m_AssetListUi.ClearAssetList();
            m_AssetListUi.PopulateAssetsList(assetsList);
        }

        async void OnTrashAsset(IAsset asset)
        {
            if (SelectedProject == null) return;
            await SelectedProject.TrashAssetsAsync(new[] { asset.Descriptor.AssetId }, CancellationToken.None);
            await Task.Delay(500);
            m_AssetListUi.ClearAssetList();
            await OnProjectSelectedAsync();
        }

        async void OnRestoreAsset(AssetDescriptor descriptor)
        {
            if (SelectedProject == null) return;
            await SelectedProject.RestoreTrashedAssetsAsync(new[] { descriptor.AssetId }, CancellationToken.None);
            await Task.Delay(500);
            m_AssetListUi.ClearAssetList();
            await OnProjectSelectedAsync();
        }

        async void OnDeletePermanentlyAsset(AssetDescriptor descriptor)
        {
            if (SelectedProject == null) return;
            await SelectedProject.DeleteAssetsFromTrashAsync(new[] { descriptor.AssetId }, CancellationToken.None);
            await Task.Delay(500);
            m_AssetListUi.ClearAssetList();
            await OnProjectSelectedAsync();
        }

        CancellationToken GetNewListToken()
        {
            m_NewListCancellationTokenSource?.Cancel();
            m_NewListCancellationTokenSource?.Dispose();
            m_NewListCancellationTokenSource = null;

            m_NewListCancellationTokenSource = new CancellationTokenSource();
            return m_NewListCancellationTokenSource.Token;
        }

        void OnLowMemory()
        {
            _ = GetNewListToken();
            _ = m_SearchBarUi.GetSearchCancellationToken();

            Resources.UnloadUnusedAssets();

            DialogService.ShowMessage("Low Memory", "The application is running low on memory. Some assets may not be displayed correctly.");
        }

        void OnViewTrashButtonClicked()
        {
            m_IsViewingTrash = !m_IsViewingTrash;
            UpdateTrashViewUI();
            _ = OnProjectSelectedAsync();
        }

        void UpdateTrashViewUI()
        {
            var assetListLabel = AssetListPanel.Q<Label>("AssetListLabel");
            var viewTrashButton = AssetListPanel.Q<Button>("ViewTrashButton");
            var addAssetButton = AssetListPanel.Q<Button>("AddAssetButton");

            if (m_IsViewingTrash)
            {
                assetListLabel.text = "Trash";
                viewTrashButton.text = "View Assets";
                addAssetButton.style.display = DisplayStyle.None;
            }
            else
            {
                assetListLabel.text = "Manage Assets";
                viewTrashButton.text = "View Trash";
                addAssetButton.style.display = DisplayStyle.Flex;
            }

            m_AssetListUi.SetViewingTrash(m_IsViewingTrash);
        }

        IAsyncEnumerable<ITrashedAsset> GetTrashedAssetsAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (SelectedProject == null) return null;
                return SelectedProject.QueryTrashedAssets().ExecuteAsync(cancellationToken);
            }
            catch (OperationCanceledException oe)
            {
                oe.LogException("GetTrashedAssetsAsync");
                return null;
            }
            catch (Exception e)
            {
                e.LogException("GetTrashedAssetsAsync");
                throw;
            }
        }
    }
}
