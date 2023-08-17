#if !UC_EXCLUDE_SAMPLES
using System;
using System.Collections;
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
            public event Action<IAsset> AssetAddedToCollection;

            CollectionPath m_CollectionPath;

            protected override SelectionType SelectionType => SelectionType.None;

            protected override void OnBindItem(VisualElement element, int i)
            {
                var asset = m_List[i];

                element.Q<Label>("ItemNameLabel").text = asset.Name;

                var button = element.Q<Button>("ItemButton");
                button.style.display = string.IsNullOrEmpty(m_CollectionPath) || asset.Collections.Contains(m_CollectionPath)
                    ? DisplayStyle.None
                    : DisplayStyle.Flex;
                button.text = "Add to Collection";
                button.clicked += () =>
                {
                    button.style.display = DisplayStyle.None;
                    AssetAddedToCollection?.Invoke(asset);
                };
            }

            public void SetSelectedCollection(IAssetCollection assetCollection)
            {
                m_CollectionPath = assetCollection?.GetFullCollectionPath();
                m_ListView?.RefreshItems();
            }
        }

        static readonly Pagination m_DefaultPagination = new(nameof(IAsset.Name), Range.All);

        CancellationTokenSource m_ListAssetsCancellationTokenSource = new();

        public IEnumerable<IAsset> Assets => m_ListController.List;

        public event Action<IAsset> AssetAddedToCollection
        {
            add => m_ListController.AssetAddedToCollection += value;
            remove => m_ListController.AssetAddedToCollection -= value;
        }

        protected override string VisualElementName => "AssetsPanel";
        protected override string EmptyListMessage => "No assets available.";

        public void OnCollectionSelected(IAssetCollection collection)
        {
            m_ListController.SetSelectedCollection(collection);
        }

        public async Task Populate(IProject project)
        {
            m_ListAssetsCancellationTokenSource.Cancel();
            m_ListAssetsCancellationTokenSource.Dispose();
            m_ListAssetsCancellationTokenSource = new CancellationTokenSource();

            var assets = GetAssetsAsync(project, m_ListAssetsCancellationTokenSource.Token);

            async Task OnEntryRetrieved(IAsset entry)
            {
                try
                {
                    await PlatformServices.AssetManager.GetAssetCollectionsAsync(entry, m_ListAssetsCancellationTokenSource.Token);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            await UpdateList(assets, m_ListAssetsCancellationTokenSource.Token, OnEntryRetrieved);
        }

        static IAsyncEnumerable<IAsset> GetAssetsAsync(IProject project, CancellationToken token)
        {
            try
            {
                return PlatformServices.AssetProvider.SearchAsync(new AssetSearchFilter(project), m_DefaultPagination, token);
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
            // This should never be called since selection type is `None`.
            throw new NotImplementedException();
        }
    }
}
#endif
