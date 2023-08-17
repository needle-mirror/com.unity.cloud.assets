#if !UC_EXCLUDE_SAMPLES
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.CollectionManagement
{
    public class CollectionMangementSample : MonoBehaviour
    {
        [SerializeField]
        UIDocument m_UiDocument;
        [SerializeField]
        UserController m_UserController;
        [SerializeField]
        VisualTreeAsset m_LayoutTemplate;
        [SerializeField]
        protected VisualTreeAsset m_CollectionListItemTemplate;

        readonly CollectionCreationController m_CreationController = new();
        readonly CollectionListUi m_CollectionListUi = new();
        readonly AssetPanelUi m_AssetPanelUi = new();

        IOrganization SelectedOrganization => m_UserController.SelectedOrganization;
        IProject SelectedProject => m_UserController.SelectedProject;

        void Start()
        {
            var uiDocumentRoot = m_UiDocument.rootVisualElement;

            var sampleContainer = uiDocumentRoot.Q<VisualElement>("ContentPanel");
            var layout = m_LayoutTemplate.Instantiate();
            sampleContainer.Add(layout);

            m_CreationController.Initialize(uiDocumentRoot);
            m_CreationController.CollectionCreated += OnCollectionCreated;

            m_CollectionListUi.Initialize(uiDocumentRoot, m_CollectionListItemTemplate);
            m_CollectionListUi.CollectionSelected += OnCollectionSelected;
            m_CollectionListUi.CollectionDeleted += OnCollectionDeleted;

            m_AssetPanelUi.Initialize(uiDocumentRoot, m_CollectionListItemTemplate);
            m_AssetPanelUi.AssetAddedToCollection += OnAssetAddedToCollection;
            m_AssetPanelUi.AssetRemovedFromCollection += OnAssetRemovedFromCollection;

            m_UserController.HideContent += HideContent;
            m_UserController.OrganizationSelected += HideContent;
            m_UserController.ProjectSelected += OnProjectSelected;

            HideContent();
        }

        void OnDestroy()
        {
            m_UserController.HideContent -= HideContent;
            m_UserController.OrganizationSelected -= HideContent;
            m_UserController.ProjectSelected -= OnProjectSelected;

            m_CreationController.Cleanup();
            m_CreationController.CollectionCreated -= OnCollectionCreated;
            m_CollectionListUi.CollectionSelected -= OnCollectionSelected;
            m_AssetPanelUi.Cleanup();
            m_AssetPanelUi.AssetAddedToCollection -= OnAssetAddedToCollection;
            m_AssetPanelUi.AssetRemovedFromCollection -= OnAssetRemovedFromCollection;
        }

        async void OnProjectSelected()
        {
            HideContent();

            await m_CollectionListUi.Populate(m_UserController.SelectedProject);
            m_AssetPanelUi.Populate(m_UserController.SelectedProject);
        }

        void HideContent()
        {
            m_CollectionListUi.Hide();
            m_AssetPanelUi.Hide();
        }

        async void OnCollectionCreated(string collectionName)
        {
            var newCollection = new AssetCollection(collectionName, "Collection created by CollectionManagementSample");
            await PlatformServices.AssetCollectionManager.CreateCollectionAsync(SelectedProject, newCollection, CancellationToken.None);

            // Force refresh the list of collections
            OnProjectSelected();
        }

        void OnCollectionSelected()
        {
            m_AssetPanelUi.OnCollectionSelected(m_CollectionListUi.SelectedCollection);
        }

        async void OnCollectionDeleted(IAssetCollection assetCollection)
        {
            await PlatformServices.AssetCollectionManager.DeleteCollectionAsync(assetCollection, CancellationToken.None);

            // Force refresh the list of collections
            OnProjectSelected();
        }

        async void OnAssetRemovedFromCollection(IAsset asset)
        {
            await PlatformServices.AssetCollectionManager.RemoveAssetsFromCollectionAsync(m_CollectionListUi.SelectedCollection,
                new[] {asset},
                CancellationToken.None);

            // Refresh the list of collections for the asset
            await PlatformServices.AssetManager.GetAssetCollectionsAsync(asset, CancellationToken.None);

            // Refresh the list of assets in the collection
            OnCollectionSelected();
        }

        async void OnAssetAddedToCollection(IAsset asset)
        {
            await PlatformServices.AssetCollectionManager.InsertAssetsToCollectionAsync(m_CollectionListUi.SelectedCollection,
                new[] {asset},
                CancellationToken.None);

            // Refresh the list of collections for the asset
            await PlatformServices.AssetManager.GetAssetCollectionsAsync(asset, CancellationToken.None);

            // Refresh the list of assets in the collection
            OnCollectionSelected();
        }
    }
}
#endif
