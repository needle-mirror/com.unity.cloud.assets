using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.CollectionManagement
{
    public class CollectionAssetListUi : ListUi<CollectionAssetListUi.AssetListController, IAsset>
    {
        public class AssetListController : ListController<IAsset>
        {
            public override void Initialize(ListView listView, VisualTreeAsset itemTemplate, Action<IEnumerable<object>> onSelectionChange)
            {
                base.Initialize(listView, itemTemplate, onSelectionChange);

                m_ListView.selectionType = SelectionType.None;
            }

            protected override void OnBindItem(VisualElement element, int i)
            {
                element.Q<Label>("ItemNameLabel").text = m_List[i].Name;

                RegisterSelectionCallback(element, i);
            }

            protected override void OnUnbindItem(VisualElement element, int i)
            {
                UnregisterSelectionCallback(element, i);
            }
        }

        const string k_NoCollectionMessage = "No collection selected.";
        const string k_FetchingMessage = "Fetching assets list...";

        public event Action<IAsset> AssetSelected;
        public IAsset SelectedAsset { get; private set; }
        public IEnumerable<IAsset> Assets => m_Entries;

        protected override string VisualElementName => "AssetsPanel";
        protected override string EmptyListMessage => "No assets in collection.";

        public void Show(IAssetCollection collection)
        {
            Show();

            var currentCollection = collection;

            SetDisplayMessage(currentCollection == null ? k_NoCollectionMessage : k_FetchingMessage);
        }

        public void Populate(IEnumerable<IAsset> assets)
        {
            UpdateList(assets, true);
        }

        protected override void OnSelectionChange(IEnumerable<object> selectedItems)
        {
            var selection = selectedItems.FirstOrDefault();
            SelectedAsset = selection as IAsset;
            AssetSelected?.Invoke(SelectedAsset);
        }
    }
}
