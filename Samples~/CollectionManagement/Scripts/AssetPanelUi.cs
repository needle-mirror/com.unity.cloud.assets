using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.CollectionManagement
{
    [Serializable]
    public class AssetPanelUi
    {
        readonly CollectionAssetListUi m_CollectionAssetListUi = new();
        ContextMenuController m_ContextMenu;
        AddToCollectionPopupController m_AddToCollectionPopup;

        [SerializeField]
        VisualTreeAsset m_ListItemTemplate;

        [SerializeField]
        VisualTreeAsset m_PopupListItemTemplate;

        IAssetProject m_CurrentProject;
        CancellationTokenSource m_CancellationTokenSource = new();

        public event Action<IEnumerable<IAsset>> AddAssetsToCollection
        {
            add => m_AddToCollectionPopup.AssetsAddedToCollection += value;
            remove => m_AddToCollectionPopup.AssetsAddedToCollection -= value;
        }

        public event Action<IAsset> RemoveAssetFromCollection;

        public void Initialize(VisualElement uiDocumentRoot)
        {
            m_CollectionAssetListUi.Initialize(uiDocumentRoot, m_ListItemTemplate);
            m_CollectionAssetListUi.AssetSelected += OnAssetSelected;
            m_CollectionAssetListUi.ListUpdated += OnCollectionAssetListUiUpdated;
            m_AddToCollectionPopup = new AddToCollectionPopupController(uiDocumentRoot, m_PopupListItemTemplate);

            m_ContextMenu = new ContextMenuController(uiDocumentRoot.Q("AssetsContextMenu"));
            m_ContextMenu.RegisterButtonAction("Add", m_AddToCollectionPopup.Show);
            m_ContextMenu.RegisterButtonAction("Remove", RemoveAsset);

            m_ContextMenu.SetEnabled(false);

            OnAssetSelected(null);
        }

        public void Cleanup()
        {
            m_CollectionAssetListUi.AssetSelected -= OnAssetSelected;
            m_ContextMenu.UnregisterButtonAction("Add", m_AddToCollectionPopup.Show);
            m_ContextMenu.UnregisterButtonAction("Remove", RemoveAsset);
        }

        public void Hide()
        {
            m_CollectionAssetListUi.Hide();
            m_AddToCollectionPopup.Hide();
        }

        public void Populate(IAssetProject project)
        {
            m_CurrentProject = project;
            m_AddToCollectionPopup.Populate(project);
        }

        public void OnCollectionSelected(IAssetCollection collection)
        {
            m_ContextMenu.SetEnabled(collection != null);

            if (m_CancellationTokenSource != null)
            {
                m_CancellationTokenSource.Cancel();
                m_CancellationTokenSource.Dispose();
            }

            m_CancellationTokenSource = new CancellationTokenSource();

            m_CollectionAssetListUi.Show(collection);

            if (collection != null)
            {
                _ = Populate(m_CurrentProject, collection, m_CancellationTokenSource.Token);
            }
        }

        async Task Populate(IAssetProject project, IAssetCollection collection, CancellationToken token)
        {
            var filter = new AssetSearchFilter();
            filter.Collections.WhereContains(collection.Descriptor.Path);

            try
            {
                var assets = project.QueryAssets().SelectWhereMatchesFilter(filter).ExecuteAsync(token);
                var assetList = new List<IAsset>();
                await foreach (var asset in assets)
                {
                    assetList.Add(asset);
                }

                if (!token.IsCancellationRequested)
                    m_CollectionAssetListUi.Populate(assetList);
            }
            catch (Exception e)
            {
                e.LogException();
            }
        }

        void OnAssetSelected(IAsset asset)
        {
            if (asset == null)
            {
                m_ContextMenu.SetButtonVisibility("Add", true);
                m_ContextMenu.SetButtonVisibility("Remove", false);
            }
            else
            {
                m_ContextMenu.SetButtonVisibility("Add", false);
                m_ContextMenu.SetButtonVisibility("Remove", true);
            }
        }

        void OnCollectionAssetListUiUpdated()
        {
            m_AddToCollectionPopup.ApplyFilter(m_CollectionAssetListUi.Assets);
        }

        void RemoveAsset()
        {
            RemoveAssetFromCollection?.Invoke(m_CollectionAssetListUi.SelectedAsset);
        }
    }
}
