#if !UC_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.CollectionManagement
{
    public class AssetListUi : ListUi<AssetListUi.AssetListController, IAsset>
    {
        public class AssetListController : ListController<IAsset>
        {
            protected override void OnBindItem(VisualElement element, int i)
            {
                element.Q<Label>("ItemNameLabel").text = m_List[i].Name;
            }

            protected override bool AreEqual(IAsset item1, IAsset item2)
            {
                return item1.Descriptor.Equals(item2.Descriptor);
            }
        }

        static readonly Pagination m_DefaultPagination = new(nameof(IAsset.Name), Range.All);

        CancellationTokenSource m_ListAssetsCancellationTokenSource = new();

        public IEnumerable<IAsset> Assets => m_ListController.AllItems;
        public IEnumerable<IAsset> SelectedAssets => m_ListController.SelectedItems.Cast<IAsset>();

        protected override string VisualElementName => "AssetsListContainer";
        protected override string EmptyListMessage => "No assets available.";

        public void ApplyFilter(IEnumerable<IAsset> itemsToFilter)
        {
            m_ListController.ApplyFilter(itemsToFilter);
        }

        public async Task Populate(IAssetProject project)
        {
            var token = GetCancellationToken();

            var assets = GetAssetsAsync(project, token);

            await UpdateList(null, assets, token, RefreshAssetCollections);
        }

        async Task RefreshAssetCollections(IAsset entry)
        {
            try
            {
                await entry.RefreshAssetCollectionsAsync(m_ListAssetsCancellationTokenSource.Token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        static IAsyncEnumerable<IAsset> GetAssetsAsync(IAssetProject project, CancellationToken token)
        {
            try
            {
                return project.SearchAssetsAsync(new AssetSearchFilter(), m_DefaultPagination, token);
            }
            catch (OperationCanceledException oe)
            {
                Debug.LogException(oe);
                throw;
            }
            catch (AggregateException e)
            {
                Debug.LogException(e.InnerException);
                throw;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        protected override void OnSelectionChange(IEnumerable<object> selectedItems)
        {
            // DO NOTHING
        }

        CancellationToken GetCancellationToken()
        {
            m_ListAssetsCancellationTokenSource.Cancel();
            m_ListAssetsCancellationTokenSource.Dispose();
            m_ListAssetsCancellationTokenSource = new CancellationTokenSource();
            return m_ListAssetsCancellationTokenSource.Token;
        }
    }
}
#endif
