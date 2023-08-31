#if !UC_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.CollectionManagement
{
    public class AddToCollectionPopupController : PopupController
    {
        readonly AssetListUi m_AssetListUi = new();

        public event Action<IEnumerable<IAsset>> AssetsAddedToCollection;

        public event Action AssetListUpdated
        {
            add => m_AssetListUi.ListUpdated += value;
            remove => m_AssetListUi.ListUpdated -= value;
        }

        public IEnumerable<IAsset> Assets => m_AssetListUi.Assets;

        public AddToCollectionPopupController(VisualElement root, VisualTreeAsset listItemTemplate)
            : base(root, "AddToCollectionPopup", null)
        {
            m_AssetListUi.Initialize(m_PopupWindow, listItemTemplate);
        }

        public void Populate(IProject project)
        {
            _ = m_AssetListUi.Populate(project);
        }

        protected override void OnClicked()
        {
            base.OnClicked();

            AssetsAddedToCollection?.Invoke(m_AssetListUi.SelectedAssets);
            m_AssetListUi.ClearSelection();
        }
    }
}
#endif
