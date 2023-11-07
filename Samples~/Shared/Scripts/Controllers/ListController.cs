#if !UC_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public abstract class ListController<T>
    {
        List<T> m_OriginalList = new();
        List<T> m_ItemsToFilter = new();

        protected List<T> m_List = new();
        protected ListView m_ListView;

        public IEnumerable<T> AllItems => m_List;
        public IEnumerable<object> SelectedItems => m_ListView.selectedItems;

        public virtual void Initialize(ListView listView, VisualTreeAsset itemTemplate, Action<IEnumerable<object>> onSelectionChange)
        {
            m_ListView = listView;

            m_ListView.makeItem = itemTemplate.Instantiate;
            m_ListView.bindItem = OnBindItem;
            m_ListView.unbindItem = OnUnbindItem;

#if UNITY_2022_3_OR_NEWER
            m_ListView.selectionChanged += onSelectionChange;
#else
            m_ListView.onSelectionChange += onSelectionChange;
#endif
        }

        public void Show()
        {
            m_ListView.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            m_ListView.style.display = DisplayStyle.None;
        }

        public void ApplyFilter(IEnumerable<T> itemsToFilter)
        {
            m_ItemsToFilter = itemsToFilter == null ? new List<T>() : new List<T>(itemsToFilter);
            UpdateList();
        }

        public void UpdateList(IEnumerable<T> entries)
        {
            m_OriginalList = entries.ToList();
            UpdateList();

            Show();
        }

        void UpdateList()
        {
            m_List = m_OriginalList.Where(x => !m_ItemsToFilter.Any(y => AreEqual(x, y))).ToList();
            m_ListView.itemsSource = m_List;
            m_ListView.RefreshItems();
        }

        protected virtual bool AreEqual(T item1, T item2)
        {
            return item1.Equals(item2);
        }

        public virtual void ClearList()
        {
            Hide();
            ClearSelection();

            m_ListView.Clear();
        }

        public virtual void ClearSelection()
        {
            m_ListView.ClearSelection();
        }

        public void SetSelectionWithoutNotify(IEnumerable<int> indices)
        {
            m_ListView.SetSelectionWithoutNotify(indices);
        }

        protected abstract void OnBindItem(VisualElement element, int i);

        protected virtual void OnUnbindItem(VisualElement element, int i)
        {
            // Do nothing by default
        }

        readonly Dictionary<VisualElement, int> m_ElementToIndex = new();

        protected void RegisterSelectionCallback(VisualElement element, int index)
        {
            m_ElementToIndex[element] = index;
            element.RegisterCallback<ClickEvent>(OnClick);
        }

        protected void UnregisterSelectionCallback(VisualElement element, int i)
        {
            m_ElementToIndex.Remove(element);
            element.UnregisterCallback<ClickEvent>(OnClick);
        }

        void OnClick(ClickEvent evt)
        {
            if (evt.currentTarget is VisualElement element)
            {
                while (element != null && !element.ClassListContains("unity-list-view__item"))
                {
                    element = element.parent;
                }

                if (element != null && m_ElementToIndex.TryGetValue(element, out var index))
                {
                    if (m_ListView.selectedIndices.Contains(index))
                        m_ListView.RemoveFromSelection(index);
                    else
                    {
                        m_ListView.ClearSelection();
                        m_ListView.AddToSelection(index);
                    }
                }
            }
        }
    }
}
#endif
