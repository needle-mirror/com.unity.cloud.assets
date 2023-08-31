#if !UC_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        readonly CollectionListUi m_CollectionListUi = new();

        [SerializeField]
        AssetPanelUi m_AssetPanelUi = new();

        CollectionsContextMenuController m_ContextMenu;

        IOrganization SelectedOrganization => m_UserController.SelectedOrganization;
        IProject SelectedProject => m_UserController.SelectedProject;

        void Start()
        {
            var uiDocumentRoot = m_UiDocument.rootVisualElement;

            var sampleContainer = uiDocumentRoot.Q("ContentPanel");
            var layout = m_LayoutTemplate.Instantiate();
            sampleContainer.Add(layout);

            m_ContextMenu = new CollectionsContextMenuController(uiDocumentRoot);
            m_ContextMenu.CollectionCreated += OnCollectionCreated;
            m_ContextMenu.CollectionUpdated += OnCollectionUpdated;
            m_ContextMenu.CollectionDeleted += OnCollectionDeleted;

            m_CollectionListUi.Initialize(uiDocumentRoot, m_CollectionListItemTemplate);
            m_CollectionListUi.CollectionSelected += OnCollectionSelected;

            m_AssetPanelUi.Initialize(uiDocumentRoot);
            m_AssetPanelUi.AssetAddedToCollection += OnAssetAddedToCollection;
            m_AssetPanelUi.RemoveAssetFromCollection += OnRemoveAssetFromCollection;

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

            if (m_ContextMenu != null)
            {
                m_ContextMenu.CollectionCreated -= OnCollectionCreated;
                m_ContextMenu.CollectionUpdated -= OnCollectionUpdated;
                m_ContextMenu.CollectionDeleted -= OnCollectionDeleted;
            }

            m_CollectionListUi.CollectionSelected -= OnCollectionSelected;
            m_AssetPanelUi.Cleanup();
            m_AssetPanelUi.AssetAddedToCollection -= OnAssetAddedToCollection;
            m_AssetPanelUi.RemoveAssetFromCollection -= OnRemoveAssetFromCollection;
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

        void HideContent()
        {
            m_ContextMenu.Hide();
            m_CollectionListUi.Hide();
            m_AssetPanelUi.Hide();
        }

        async void OnCollectionCreated(IAssetCollection collection)
        {
            await PlatformServices.AssetCollectionManager.CreateCollectionAsync(SelectedProject, collection, CancellationToken.None);

            // Force refresh the list of collections
            OnProjectSelected();
        }

        void OnCollectionSelected()
        {
            m_ContextMenu.OnCollectionSelected(m_CollectionListUi.SelectedCollection);
            m_AssetPanelUi.OnCollectionSelected(m_CollectionListUi.SelectedCollection);
        }

        async void OnCollectionUpdated(IAssetCollection assetCollection)
        {
            await PlatformServices.AssetCollectionManager.UpdateCollectionAsync(assetCollection, CancellationToken.None);

            // Force refresh the list of collections
            OnProjectSelected();
        }

        async void OnCollectionDeleted(IAssetCollection assetCollection)
        {
            await PlatformServices.AssetCollectionManager.DeleteCollectionAsync(assetCollection, CancellationToken.None);

            // Force refresh the list of collections
            OnProjectSelected();
        }

        async void OnRemoveAssetFromCollection(IAsset asset)
        {
            await PlatformServices.AssetCollectionManager.RemoveAssetsFromCollectionAsync(m_CollectionListUi.SelectedCollection,
                new[] {asset},
                CancellationToken.None);

            // Refresh the list of collections for the asset
            await PlatformServices.AssetManager.GetAssetCollectionsAsync(asset, CancellationToken.None);

            // Refresh the list of assets in the collection
            OnCollectionSelected();
        }

        async void OnAssetAddedToCollection(IEnumerable<IAsset> assets)
        {
            var enumerable = assets as IAsset[] ?? assets.ToArray();
            await PlatformServices.AssetCollectionManager.InsertAssetsToCollectionAsync(m_CollectionListUi.SelectedCollection,
                enumerable,
                CancellationToken.None);

            var taskList = new List<Task>();

            // Refresh the list of collections for each modified asset
            foreach (var asset in enumerable)
            {
                var task = PlatformServices.AssetManager.GetAssetCollectionsAsync(asset, CancellationToken.None);
                taskList.Add(task);
            }

            await Task.WhenAll(taskList);

            // Refresh the list of assets in the collection
            OnCollectionSelected();
        }
    }
}
#endif
