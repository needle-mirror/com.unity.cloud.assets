#if !UC_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.CollectionManagement
{
    public class CollectionAssetListUi : ListUi<CollectionAssetListUi.AssetListController, IAsset>
    {
        public class AssetListController : ListController<IAsset>
        {
            protected override void OnBindItem(VisualElement element, int i)
            {
                element.Q<Label>("ItemNameLabel").text = m_List[i].Name;
            }
        }

        const string k_NoCollectionMessage = "No collection selected.";

        IAssetCollection m_CurrentCollection;

        public event Action<IAsset> AssetSelected;
        public IAsset SelectedAsset { get; private set; }

        protected override string VisualElementName => "AssetsPanel";
        protected override string EmptyListMessage => "No assets in collection.";

        public void Populate(IAssetCollection collection, IEnumerable<IAsset> assets)
        {
            Show();

            m_CurrentCollection = collection;

            if (m_CurrentCollection == null)
            {
                SetDisplayMessage(k_NoCollectionMessage);
                return;
            }

            Populate(assets);
        }

        public void Populate(IEnumerable<IAsset> assets)
        {
            if (m_CurrentCollection == null) return;

            var filteredAssets = assets.Where(a => a.Collections.Contains(m_CurrentCollection.GetFullCollectionPath()));
            UpdateList(filteredAssets, true);
        }

        protected override void OnSelectionChange(IEnumerable<object> selectedItems)
        {
            var selection = selectedItems.FirstOrDefault();
            if (selection == SelectedAsset)
            {
                m_ListController.SetSelectionWithoutNotify(Array.Empty<int>());
                SelectedAsset = null;
            }
            else
            {
                SelectedAsset = selection as IAsset;
            }

            AssetSelected?.Invoke(SelectedAsset);
        }
    }
}
#endif
