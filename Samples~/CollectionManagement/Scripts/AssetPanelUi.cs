#if !UC_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
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

        public event Action<IEnumerable<IAsset>> AssetAddedToCollection
        {
            add => m_AddToCollectionPopup.AssetsAddedToCollection += value;
            remove => m_AddToCollectionPopup.AssetsAddedToCollection -= value;
        }

        public event Action<IAsset> RemoveAssetFromCollection;

        public void Initialize(VisualElement uiDocumentRoot)
        {
            m_CollectionAssetListUi.Initialize(uiDocumentRoot, m_ListItemTemplate);
            m_CollectionAssetListUi.AssetSelected += OnAssetSelected;
            m_AddToCollectionPopup = new AddToCollectionPopupController(uiDocumentRoot, m_PopupListItemTemplate);
            m_AddToCollectionPopup.AssetListUpdated += OnAssetListUpdated;

            m_ContextMenu = new ContextMenuController(uiDocumentRoot.Q("AssetsContextMenu"));
            m_ContextMenu.RegisterButtonAction("Add", m_AddToCollectionPopup.Show);
            m_ContextMenu.RegisterButtonAction("Remove", RemoveAsset);

            m_ContextMenu.SetEnabled(false);

            OnAssetSelected(null);
        }

        public void Cleanup()
        {
            m_CollectionAssetListUi.AssetSelected -= OnAssetSelected;
            m_AddToCollectionPopup.AssetListUpdated -= OnAssetListUpdated;
            m_ContextMenu.UnregisterButtonAction("Add", m_AddToCollectionPopup.Show);
            m_ContextMenu.UnregisterButtonAction("Remove", RemoveAsset);
        }

        public void Hide()
        {
            m_CollectionAssetListUi.Hide();
            m_AddToCollectionPopup.Hide();
        }

        public void Populate(IProject project)
        {
            m_CollectionAssetListUi.Populate(null, null);
            m_AddToCollectionPopup.Populate(project);
        }

        public void OnCollectionSelected(IAssetCollection collection)
        {
            m_ContextMenu.SetEnabled(collection != null);

            m_CollectionAssetListUi.Populate(collection, m_AddToCollectionPopup.Assets);
        }

        void OnAssetListUpdated()
        {
            m_CollectionAssetListUi.Populate(m_AddToCollectionPopup.Assets);
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

        void RemoveAsset()
        {
            RemoveAssetFromCollection?.Invoke(m_CollectionAssetListUi.SelectedAsset);
        }
    }
}
#endif
