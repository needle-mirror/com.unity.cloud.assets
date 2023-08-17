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
            public event Action<IAssetCollection> AssetCollectionDeleted;

            protected override void OnBindItem(VisualElement element, int i)
            {
                element.Q<Label>("ItemNameLabel").text = m_List[i].Name;

                var button = element.Q<Button>("ItemButton");
                button.style.display = DisplayStyle.Flex;
                button.text = "Delete";
                button.clicked += () =>
                {
                    button.style.display = DisplayStyle.None;
                    AssetCollectionDeleted?.Invoke(m_List[i]);
                };
            }
        }

        public event Action CollectionSelected;

        IAssetCollection m_SelectedCollection;

        public event Action<IAssetCollection> CollectionDeleted
        {
            add => m_ListController.AssetCollectionDeleted += value;
            remove => m_ListController.AssetCollectionDeleted -= value;
        }

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
            UpdateList(collections);

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
            SelectedCollection = selectedItems.FirstOrDefault() as IAssetCollection;
        }
    }
}
#endif
