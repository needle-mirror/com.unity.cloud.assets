using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.CollectionManagement
{
    public class AddToCollectionPopupController : PopupController
    {
        readonly AssetListUi m_AssetListUi = new();

        public event Action<IEnumerable<IAsset>> AssetsAddedToCollection;

        public AddToCollectionPopupController(VisualElement root, Func<VisualElement> makeItem)
            : base(root, "AddToCollectionPopup")
        {
            m_AssetListUi.Initialize(m_PopupWindow, makeItem);
            m_AssetListUi.OnSelectionChanged += OnSelectionChanged;

            m_ActionButton.SetEnabled(false);
        }

        public void ApplyFilter(IEnumerable<IAsset> itemToFilter)
        {
            m_AssetListUi.ApplyFilter(itemToFilter);
        }

        public void Populate(IAssetProject project)
        {
            _ = m_AssetListUi.Populate(project);
        }

        protected override void OnClicked()
        {
            base.OnClicked();

            AssetsAddedToCollection?.Invoke(m_AssetListUi.SelectedAssets);
            m_AssetListUi.ClearSelection();
        }

        void OnSelectionChanged()
        {
            m_ActionButton.SetEnabled(m_AssetListUi.SelectedAssets.Any());
        }
    }
}
