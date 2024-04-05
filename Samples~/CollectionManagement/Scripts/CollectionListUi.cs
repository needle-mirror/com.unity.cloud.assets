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

        public IEnumerable<IAssetCollection> Collections => m_ListController.AllItems;

        protected override string VisualElementName => "CollectionsPanel";
        protected override string EmptyListMessage => "No collections available.";

        public async Task Populate(IAssetProject project)
        {
            Show();

            var collections = (await GetCollectionsAsync(project)).ToArray();

            m_ListController.ClearList();
            m_ListController.ClearSelection();

            UpdateList(collections);

            if (m_SelectedCollection != null)
            {
                var collectionPath = m_SelectedCollection.Descriptor.Path;
                if (collections.All(x => x.Descriptor.Path != collectionPath))
                {
                    SelectedCollection = null;
                }
            }
        }

        static async Task<IEnumerable<IAssetCollection>> GetCollectionsAsync(IAssetProject project)
        {
            try
            {
                var results = project.ListCollectionsAsync(Range.All, CancellationToken.None);
                var collections = new List<IAssetCollection>();
                await foreach (var collection in results)
                {
                    collections.Add(collection);
                }

                return collections;
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
            var selection = selectedItems.FirstOrDefault();
            SelectedCollection = selection as IAssetCollection;
        }
    }
}
