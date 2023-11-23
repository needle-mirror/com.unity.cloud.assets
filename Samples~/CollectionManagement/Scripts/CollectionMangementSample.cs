using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        UserController m_UserController;
        [SerializeField]
        VisualTreeAsset m_LayoutTemplate;
        [SerializeField]
        protected VisualTreeAsset m_CollectionListItemTemplate;

        readonly CollectionListUi m_CollectionListUi = new();

        [SerializeField]
        AssetPanelUi m_AssetPanelUi = new();

        CollectionsContextMenuController m_ContextMenu;
        MessagePopupController m_MessagePopupController;

        IAssetProject SelectedProject => m_UserController.SelectedProject;
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

            m_CollectionListUi.Initialize(uiDocumentRoot, m_CollectionListItemTemplate);
            m_CollectionListUi.CollectionSelected += OnCollectionSelected;

            m_AssetPanelUi.Initialize(uiDocumentRoot);
            m_AssetPanelUi.AssetAddedToCollection += OnAssetAddedToCollection;
            m_AssetPanelUi.RemoveAssetFromCollection += OnRemoveAssetFromCollection;

            m_UserController.HideContent += HideContent;
            m_UserController.OrganizationSelected += OnOrganizationSelected;
            m_UserController.ProjectSelected += OnProjectSelected;

            m_MessagePopupController = new MessagePopupController(uiDocumentRoot);

            HideContent();
        }

        void OnDestroy()
        {
            m_UserController.HideContent -= HideContent;
            m_UserController.OrganizationSelected -= OnOrganizationSelected;
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
                await SelectedProject.CreateCollectionAsync(collectionCreation, CancellationToken.None);
            }
            catch (Exception e)
            {
                m_MessagePopupController.ShowMessage("Failed to create collection", $"{e.Message}");
            }

            // Force refresh the list of collections
            OnProjectSelected();
        }

        void OnCollectionSelected()
        {
            m_ContextMenu.OnCollectionSelected(SelectedCollection);
            m_AssetPanelUi.OnCollectionSelected(SelectedCollection);
        }

        async void OnCollectionUpdated(IAssetCollection assetCollection)
        {
            await assetCollection.UpdateAsync(CancellationToken.None);

            // Force refresh the list of collections
            OnProjectSelected();
        }

        async void OnCollectionDeleted(IAssetCollection assetCollection)
        {
            await SelectedProject.DeleteCollectionAsync(assetCollection.GetFullCollectionPath(), CancellationToken.None);

            // Force refresh the list of collections
            OnProjectSelected();
        }

        async void OnRemoveAssetFromCollection(IAsset asset)
        {
            await SelectedCollection.RemoveAssetsAsync(new[] {asset},
                CancellationToken.None);

            // Refresh the list of collections for the asset
            await asset.RefreshAssetCollectionsAsync(CancellationToken.None);

            await Task.Delay(1000);
            // Refresh the list of assets in the collection
            OnCollectionSelected();
        }

        async void OnAssetAddedToCollection(IEnumerable<IAsset> assets)
        {
            var enumerable = assets as IAsset[] ?? assets.ToArray();
            await SelectedCollection.AddAssetsAsync(enumerable,
                CancellationToken.None);

            await Task.Delay(1000);
            // Refresh the list of assets in the collection
            OnCollectionSelected();
        }
    }
}
