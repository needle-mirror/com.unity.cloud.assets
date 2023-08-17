#if !UC_EXCLUDE_SAMPLES
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.CollectionManagement
{
    public class AssetPanelUi
    {
        readonly AssetListUi m_AssetListUi = new();
        readonly CollectionAssetListUi m_CollectionAssetListUi = new();

        VisualElement m_AssetsPanelContainer;
        Button m_DisplayAllButton;
        Button m_DisplayCollectionButton;

        bool m_IsDisplayingAll = true;

        public event Action<IAsset> AssetAddedToCollection
        {
            add => m_AssetListUi.AssetAddedToCollection += value;
            remove => m_AssetListUi.AssetAddedToCollection -= value;
        }

        public event Action<IAsset> AssetRemovedFromCollection
        {
            add => m_CollectionAssetListUi.AssetRemovedFromCollection += value;
            remove => m_CollectionAssetListUi.AssetRemovedFromCollection -= value;
        }

        public void Initialize(VisualElement uiDocumentRoot, VisualTreeAsset listItemTemplate)
        {
            m_AssetsPanelContainer = uiDocumentRoot.Q<VisualElement>("Assets");

            m_DisplayAllButton = m_AssetsPanelContainer.Q<Button>("AllAssetsButton");
            m_DisplayAllButton.clicked += OnDisplayAll;
            m_DisplayCollectionButton = m_AssetsPanelContainer.Q<Button>("CollectionAssetsButton");
            m_DisplayCollectionButton.clicked += OnDisplayCollection;

            m_AssetListUi.Initialize(uiDocumentRoot, listItemTemplate);
            m_CollectionAssetListUi.Initialize(uiDocumentRoot, listItemTemplate);

            OnDisplayChanged();
        }

        public void Cleanup()
        {
            m_DisplayAllButton.clicked -= OnDisplayAll;
            m_DisplayCollectionButton.clicked -= OnDisplayCollection;
        }

        public void Hide()
        {
            m_AssetListUi.Hide();
            m_CollectionAssetListUi.Hide();
            m_AssetsPanelContainer.style.display = DisplayStyle.None;
        }

        public void Populate(IProject project)
        {
            m_AssetsPanelContainer.style.display = DisplayStyle.Flex;

            if (m_IsDisplayingAll)
            {
                m_AssetListUi.Show();
            }

            m_CollectionAssetListUi.Populate(null, null);
            _ = m_AssetListUi.Populate(project);
        }

        public void OnCollectionSelected(IAssetCollection collection)
        {
            if (!m_IsDisplayingAll)
            {
                m_CollectionAssetListUi.Show();
            }

            m_CollectionAssetListUi.Populate(collection, m_AssetListUi.Assets);
            m_AssetListUi.OnCollectionSelected(collection);
        }

        void OnDisplayAll()
        {
            if (m_IsDisplayingAll) return;

            m_IsDisplayingAll = true;
            OnDisplayChanged();
        }

        void OnDisplayCollection()
        {
            if (!m_IsDisplayingAll) return;

            m_IsDisplayingAll = false;
            OnDisplayChanged();
        }

        void OnDisplayChanged()
        {
            m_DisplayAllButton.SetEnabled(!m_IsDisplayingAll);
            m_DisplayCollectionButton.SetEnabled(m_IsDisplayingAll);

            if (m_IsDisplayingAll)
            {
                m_AssetListUi.Show();
                m_CollectionAssetListUi.Hide();
            }
            else
            {
                m_AssetListUi.Hide();
                m_CollectionAssetListUi.Show();
            }
        }
    }
}
#endif
