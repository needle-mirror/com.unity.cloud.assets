using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public abstract class ListController<T>
    {
        List<T> m_OriginalList = new();
        Func<T, bool> m_Filter;

        protected List<T> m_List = new();
        protected ListView m_ListView;

        public IEnumerable<T> AllItems => m_List;
        public IEnumerable<object> SelectedItems => m_ListView.selectedItems;

        public virtual void Initialize(ListView listView, VisualTreeAsset itemTemplate, Action<IEnumerable<object>> onSelectionChange)
        {
            m_ListView = listView;

            m_ListView.makeItem = () => OnMakeItem(itemTemplate);
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

        public void ApplyFilter(Func<T, bool> filter)
        {
            m_Filter = filter;
            if (m_OriginalList.Any())
            {
                UpdateList();
            }
        }

        public void UpdateList(IEnumerable<T> entries)
        {
            m_OriginalList = entries.ToList();
            UpdateList();

            Show();
        }

        void UpdateList()
        {
            m_List = m_Filter == null ? m_OriginalList.ToList() : m_OriginalList.Where(m_Filter).ToList();
            m_ListView.itemsSource = m_List;
            m_ListView.Rebuild();
        }

        public virtual void ClearList()
        {
            Hide();

            m_ListView.Clear();
        }

        public void ClearSelection()
        {
            m_ListView.ClearSelection();
        }

        public void SetSelectionWithoutNotify(IEnumerable<int> indices)
        {
            m_ListView.SetSelectionWithoutNotify(indices);
        }

        protected virtual VisualElement OnMakeItem(VisualTreeAsset itemTemplate)
        {
            return itemTemplate.Instantiate();
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
