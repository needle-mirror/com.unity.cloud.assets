using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.Cloud.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.CollectionManagement
{
    public class CollectionMangementSample : MonoBehaviour
    {
        [SerializeField]
        UIDocument m_UiDocument;
        [SerializeField]
        ProjectController m_ProjectController;
        [SerializeField]
        VisualTreeAsset m_LayoutTemplate;

        readonly CollectionListUi m_CollectionListUi = new();
        readonly AssetPanelUi m_AssetPanelUi = new();

        CollectionsContextMenuController m_ContextMenu;

        IAssetProject SelectedProject => m_ProjectController.SelectedProject;
        IAssetCollection SelectedCollection => m_CollectionListUi.SelectedCollection;

        void Start()
        {
            var uiDocumentRoot = m_UiDocument.rootVisualElement;

            var sampleContainer = uiDocumentRoot.Q("ContentPanel");
            var layout = m_LayoutTemplate.Instantiate();
            layout.style.height = Length.Percent(100);
            layout.style.width = Length.Percent(100);
            sampleContainer.Add(layout);

            m_ContextMenu = new CollectionsContextMenuController(uiDocumentRoot, ValidateCollectionName);
            m_ContextMenu.CollectionCreated += OnCollectionCreated;
            m_ContextMenu.CollectionUpdated += OnCollectionUpdated;
            m_ContextMenu.CollectionDeleted += OnCollectionDeleted;

            m_CollectionListUi.Initialize(uiDocumentRoot, default);
            m_CollectionListUi.CollectionSelected += OnCollectionSelected;

            m_AssetPanelUi.Initialize(uiDocumentRoot);
            m_AssetPanelUi.AddAssetsToCollection += AddAssetsToCollection;
            m_AssetPanelUi.RemoveAssetFromCollection += RemoveAssetFromCollection;

            m_ProjectController.HideContent += HideContent;
            m_ProjectController.OrganizationSelected += OnOrganizationSelected;
            m_ProjectController.ProjectSelected += OnProjectSelected;

            HideContent();
        }

        void OnDestroy()
        {
            m_ProjectController.HideContent -= HideContent;
            m_ProjectController.OrganizationSelected -= OnOrganizationSelected;
            m_ProjectController.ProjectSelected -= OnProjectSelected;

            if (m_ContextMenu != null)
            {
                m_ContextMenu.CollectionCreated -= OnCollectionCreated;
                m_ContextMenu.CollectionUpdated -= OnCollectionUpdated;
                m_ContextMenu.CollectionDeleted -= OnCollectionDeleted;
            }

            m_CollectionListUi.CollectionSelected -= OnCollectionSelected;
            m_AssetPanelUi.Cleanup();
            m_AssetPanelUi.AddAssetsToCollection -= AddAssetsToCollection;
            m_AssetPanelUi.RemoveAssetFromCollection -= RemoveAssetFromCollection;
        }

        async void OnProjectSelected()
        {
            HideContent();

            if (SelectedProject != null)
            {
                await m_CollectionListUi.Populate(SelectedProject);
                m_AssetPanelUi.Populate(SelectedProject);
            }
        }

        void OnOrganizationSelected(OrganizationId orgId)
        {
            HideContent();
        }

        void HideContent()
        {
            m_ContextMenu.Hide();
            m_CollectionListUi.Hide();
            m_AssetPanelUi.Hide();
        }

        (bool, string) ValidateCollectionName(string s)
        {
            return m_CollectionListUi.Collections.Any(x => x.Name == s)
                ? (false, "Collection name already exists.")
                : (!string.IsNullOrWhiteSpace(s), string.Empty);
        }

        async void OnCollectionCreated(IAssetCollectionCreation collectionCreation)
        {
            try
            {
                await SelectedProject.CreateCollectionLiteAsync(collectionCreation, CancellationToken.None);
            }
            catch (Exception e)
            {
                DialogService.ShowMessage(e, "Creation failed", $"Failed to create collection {collectionCreation.Name} with reason: {e.Message}");
            }

            // Force refresh the list of collections
            OnProjectSelected();
        }

        void OnCollectionSelected()
        {
            m_ContextMenu.OnCollectionSelected(SelectedCollection);
            m_AssetPanelUi.OnCollectionSelected(SelectedCollection);
        }

        async void OnCollectionUpdated(IAssetCollection assetCollection, IAssetCollectionUpdate assetCollectionUpdate)
        {
            await assetCollection.UpdateAsync(assetCollectionUpdate, CancellationToken.None);

            // Force refresh the list of collections
            OnProjectSelected();
        }

        async void OnCollectionDeleted(IAssetCollection assetCollection)
        {
            await SelectedProject.DeleteCollectionAsync(assetCollection.Descriptor.Path, CancellationToken.None);

            // Force refresh the list of collections
            OnProjectSelected();
        }

        async void RemoveAssetFromCollection(IAsset asset)
        {
            await SelectedCollection.UnlinkAssetsAsync(new[] {asset},
                CancellationToken.None);

            // Refresh the asset
            _ = asset.RefreshAsync(default);

            // Refresh the list of assets in the collection
            OnCollectionSelected();
        }

        async void AddAssetsToCollection(IEnumerable<IAsset> assets)
        {
            var enumerable = assets as IAsset[] ?? assets.ToArray();
            await SelectedCollection.LinkAssetsAsync(enumerable, CancellationToken.None);

            foreach (var asset in enumerable)
            {
                _ = asset.RefreshAsync(default);
            }

            // Refresh the list of assets in the collection
            OnCollectionSelected();
        }
    }
}
