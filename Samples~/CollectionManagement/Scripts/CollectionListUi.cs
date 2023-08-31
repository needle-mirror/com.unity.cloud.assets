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
    public class CollectionListUi : ListUi<CollectionListUi.CollectionListController, IAssetCollection>
    {
        public class CollectionListController : ListController<IAssetCollection>
        {
            protected override void OnBindItem(VisualElement element, int i)
            {
                element.Q<Label>("ItemNameLabel").text = m_List[i].Name;
            }
        }

        public event Action CollectionSelected;

        IAssetCollection m_SelectedCollection;

        public IAssetCollection SelectedCollection
        {
            get => m_SelectedCollection;
            private set
            {
                m_SelectedCollection = value;
                Debug.Log($"Collection Selected: {m_SelectedCollection?.Name}");
                CollectionSelected?.Invoke();
            }
        }

        protected override string VisualElementName => "CollectionsPanel";
        protected override string EmptyListMessage => "No collections available.";

        public async Task Populate(IProject project)
        {
            Show();

            var collections = await GetCollectionsAsync(project);
            UpdateList(collections, true);

            if (m_SelectedCollection != null)
            {
                var collectionPath = m_SelectedCollection.GetFullCollectionPath();
                if (collections.All(x => x.GetFullCollectionPath() != collectionPath))
                {
                    SelectedCollection = null;
                }
            }
        }

        static async Task<IAssetCollection[]> GetCollectionsAsync(IProject project)
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                return await PlatformServices.AssetCollectionManager.ListCollectionsAsync(project, cancellationTokenSource.Token);
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
            var selection = selectedItems.FirstOrDefault();
            if (selection == m_SelectedCollection)
            {
                m_ListController.SetSelectionWithoutNotify(Array.Empty<int>());
                SelectedCollection = null;
            }
            else
            {
                SelectedCollection = selection as IAssetCollection;
            }
        }
    }
}
#endif
