#if !UC_EXCLUDE_SAMPLES
using System;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.CollectionManagement
{
    public class CollectionsContextMenuController : ContextMenuController
    {
        readonly CreateCollectionPopupController m_CreatePopup;
        readonly EditCollectionPopupController m_EditPopup;

        IAssetCollection m_AssetCollection;

        public event Action<IAssetCollection> CollectionCreated
        {
            add => m_CreatePopup.CollectionCreated += value;
            remove => m_CreatePopup.CollectionCreated -= value;
        }

        public event Action<IAssetCollection> CollectionUpdated;
        public event Action<IAssetCollection> CollectionDeleted;

        public CollectionsContextMenuController(VisualElement root)
            : base(root.Q("CollectionsContextMenu"))
        {
            m_CreatePopup = new CreateCollectionPopupController(root);
            m_EditPopup = new EditCollectionPopupController(root, OnCollectionUpdated);

            RegisterButtonAction("Create", m_CreatePopup.Show);
            RegisterButtonAction("Edit", () => m_EditPopup.Show());
            RegisterButtonAction("Delete", () => CollectionDeleted?.Invoke(m_AssetCollection));

            UpdateButtonVisibility();
        }

        public void OnCollectionSelected(IAssetCollection assetCollection)
        {
            m_AssetCollection = assetCollection;

            if (assetCollection != null)
            {
                m_EditPopup.SetAssetCollection(assetCollection);
            }

            UpdateButtonVisibility();
        }

        public override void Hide()
        {
            base.Hide();

            m_CreatePopup.Hide();
            m_EditPopup.Hide();
        }

        void OnCollectionUpdated()
        {
            CollectionUpdated?.Invoke(m_AssetCollection);
        }

        void UpdateButtonVisibility()
        {
            SetButtonVisibility("Create", m_AssetCollection == null);
            SetButtonVisibility("Edit", m_AssetCollection != null);
            SetButtonVisibility("Delete", m_AssetCollection != null);
        }
    }
}
#endif
