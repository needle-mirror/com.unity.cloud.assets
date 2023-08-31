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
            class ToggleController
            {
                readonly Toggle m_Toggle;
                readonly ListView m_ListView;

                int m_Target = -1;

                public ToggleController(ListView listView, Toggle toggle)
                {
                    m_ListView = listView;
                    m_Toggle = toggle;
                    m_Toggle.UnregisterCallback<ChangeEvent<bool>>(OnToggleChanged);
                    m_Toggle.RegisterCallback<ChangeEvent<bool>>(OnToggleChanged);
                }

                ~ToggleController()
                {
                    m_Toggle.UnregisterCallback<ChangeEvent<bool>>(OnToggleChanged);
                }

                public void SetTarget(int target, bool isSelected)
                {
                    m_Target = target;
                    m_Toggle.SetValueWithoutNotify(isSelected);
                }

                public void Unselect()
                {
                    m_Toggle.SetValueWithoutNotify(false);
                }

                void OnToggleChanged(ChangeEvent<bool> evt)
                {
                    if (m_Target < 0) return;

                    if (evt.newValue)
                    {
                        m_ListView.AddToSelection(m_Target);
                    }
                    else
                    {
                        m_ListView.RemoveFromSelection(m_Target);
                    }
                }
            }

            readonly Dictionary<Toggle, ToggleController> m_ToggleControllers = new();

            protected override void OnBindItem(VisualElement element, int i)
            {
                var asset = m_List[i];

                element.Q<Label>("ItemNameLabel").text = asset.Name;

                var toggle = element.Q<Toggle>();
                if (!m_ToggleControllers.TryGetValue(toggle, out var toggleController))
                {
                    toggleController = new ToggleController(m_ListView, toggle);
                    m_ToggleControllers.Add(toggle, toggleController);
                }

                toggleController.SetTarget(i, m_ListView.selectedItems.Contains(asset));
            }

            public override void ClearList()
            {
                base.ClearList();

                m_ToggleControllers.Clear();
            }

            public override void ClearSelection()
            {
                base.ClearSelection();

                foreach (var kvp in m_ToggleControllers)
                {
                    kvp.Value.Unselect();
                }
            }
        }

        static readonly Pagination m_DefaultPagination = new(nameof(IAsset.Name), Range.All);

        CancellationTokenSource m_ListAssetsCancellationTokenSource = new();

        public IEnumerable<IAsset> Assets => m_ListController.AllItems;
        public IEnumerable<IAsset> SelectedAssets => m_ListController.SelectedItems.Cast<IAsset>();

        protected override string VisualElementName => "AssetsListContainer";
        protected override string EmptyListMessage => "No assets available.";

        public async Task Populate(IProject project)
        {
            var token = GetCancellationToken();

            var assets = GetAssetsAsync(project, token);

            async Task OnEntryRetrieved(IAsset entry)
            {
                try
                {
                    await PlatformServices.AssetManager.GetAssetCollectionsAsync(entry, token);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            await UpdateList(null, assets, token, OnEntryRetrieved);
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
