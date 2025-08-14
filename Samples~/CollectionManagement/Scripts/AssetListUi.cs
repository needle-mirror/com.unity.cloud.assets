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
                _ = PopulateItemAsync(element, m_List[i]);
            }

            static async Task PopulateItemAsync(VisualElement element, IAsset asset)
            {
                var properties = await asset.GetPropertiesAsync(CancellationToken.None);

                element.Q<Label>().text = properties.Name;
            }
        }

        CancellationTokenSource m_ListAssetsCancellationTokenSource = new();

        public IEnumerable<IAsset> Assets => m_ListController.AllItems;
        public IEnumerable<IAsset> SelectedAssets => m_ListController.SelectedItems.Cast<IAsset>();
        public event Action OnSelectionChanged;

        protected override string VisualElementName => "AssetsListContainer";
        protected override string EmptyListMessage => "No assets available.";

        public void ApplyFilter(IEnumerable<IAsset> itemsToFilter)
        {
            m_ListController.ApplyFilter(x => !itemsToFilter.Any(y => x.Descriptor.Equals(y.Descriptor)));
        }

        public async Task Populate(IAssetProject project)
        {
            var token = GetCancellationToken();

            var assets = GetAssetsAsync(project, token);

            await UpdateList(null, assets, token);
        }

        static IAsyncEnumerable<IAsset> GetAssetsAsync(IAssetProject project, CancellationToken token)
        {
            try
            {
                return project.QueryAssets().ExecuteAsync(token);
            }
            catch (OperationCanceledException oe)
            {
                oe.LogException();
                return null;
            }
            catch (Exception e)
            {
                e.LogException();
                throw;
            }
        }

        protected override void OnSelectionChange(IEnumerable<object> selectedItems)
        {
            OnSelectionChanged?.Invoke();
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
