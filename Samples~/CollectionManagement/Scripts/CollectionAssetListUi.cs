using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.CollectionManagement
{
    public class CollectionAssetListUi : ListUi<CollectionAssetListUi.AssetListController, IAsset>
    {
        public class AssetListController : ListController<IAsset>
        {
            public override void Initialize(ListView listView, Func<VisualElement> makeItem, Action<IEnumerable<object>> onSelectionChange)
            {
                base.Initialize(listView, makeItem, onSelectionChange);

                m_ListView.selectionType = SelectionType.None;
            }

            protected override void OnBindItem(VisualElement element, int i)
            {
                _ = PopulateItemAsync(element, m_List[i]);

                RegisterSelectionCallback(element, i);
            }

            protected override void OnUnbindItem(VisualElement element, int i)
            {
                UnregisterSelectionCallback(element, i);
            }

            static async Task PopulateItemAsync(VisualElement element, IAsset asset)
            {
                var properties = await asset.GetPropertiesAsync(CancellationToken.None);

                element.Q<Label>().text = properties.Name;
            }
        }

        const string k_NoCollectionMessage = "No collection selected.";
        const string k_FetchingMessage = "Fetching assets list...";

        public event Action<IAsset> AssetSelected;
        public IAsset SelectedAsset { get; private set; }
        public IEnumerable<IAsset> Assets => m_ListController.AllItems;

        protected override string VisualElementName => "AssetsPanel";
        protected override string EmptyListMessage => "No assets in collection.";

        public void Show(IAssetCollection collection)
        {
            Show();

            SetDisplayMessage(collection == null ? k_NoCollectionMessage : k_FetchingMessage);
        }

        public void Populate(IEnumerable<IAsset> assets)
        {
            m_ListController.ClearList();
            m_ListController.ClearSelection();

            UpdateList(assets);

            if (SelectedAsset != null && assets.All(x => x.Descriptor.AssetId != SelectedAsset.Descriptor.AssetId))
            {
                SelectedAsset = null;
            }
        }

        protected override void OnSelectionChange(IEnumerable<object> selectedItems)
        {
            var selection = selectedItems.FirstOrDefault();
            SelectedAsset = selection as IAsset;
            AssetSelected?.Invoke(SelectedAsset);
        }
    }
}
