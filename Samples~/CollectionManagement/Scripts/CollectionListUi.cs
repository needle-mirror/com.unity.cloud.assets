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

        public IEnumerable<IAssetCollection> Collections => m_Entries;

        protected override string VisualElementName => "CollectionsPanel";
        protected override string EmptyListMessage => "No collections available.";

        public async Task Populate(IAssetProject project)
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

        static async Task<IEnumerable<IAssetCollection>> GetCollectionsAsync(IAssetProject project)
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                return await project.ListCollectionsAsync(cancellationTokenSource.Token);
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
            SelectedCollection = selection as IAssetCollection;
        }
    }
}
#endif
