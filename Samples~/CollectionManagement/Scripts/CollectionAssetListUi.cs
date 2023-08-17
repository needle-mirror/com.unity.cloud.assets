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
            public event Action<IAsset> AssetRemovedFromCollection;

            protected override SelectionType SelectionType => SelectionType.None;

            protected override void OnBindItem(VisualElement element, int i)
            {
                element.Q<Label>("ItemNameLabel").text = m_List[i].Name;

                var button = element.Q<Button>("ItemButton");
                button.style.display = DisplayStyle.Flex;
                button.text = "Remove from Collection";
                button.clicked += () =>
                {
                    button.style.display = DisplayStyle.None;
                    AssetRemovedFromCollection?.Invoke(m_List[i]);
                };
            }
        }

        const string k_NoCollectionMessage = "No collection selected.";

        public event Action<IAsset> AssetRemovedFromCollection
        {
            add => m_ListController.AssetRemovedFromCollection += value;
            remove => m_ListController.AssetRemovedFromCollection -= value;
        }

        protected override string VisualElementName => "AssetCollectionsPanel";
        protected override string EmptyListMessage => "No assets in collection.";

        public void Populate(IAssetCollection collection, IEnumerable<IAsset> assets)
        {
            if (collection == null)
            {
                SetDisplayMessage(k_NoCollectionMessage);
                return;
            }

            var filteredAssets = assets.Where(a => a.Collections.Contains(collection.GetFullCollectionPath()));
            UpdateList(filteredAssets);
        }

        protected override void OnSelectionChange(IEnumerable<object> selectedItems)
        {
            // This should never be called since selection type is `None`.
            throw new NotImplementedException();
        }
    }
}
#endif
