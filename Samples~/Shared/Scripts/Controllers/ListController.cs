#if !UC_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public abstract class ListController<T>
    {
        protected List<T> m_List = new();
        protected ListView m_ListView;

        public IEnumerable<T> AllItems => m_List;
        public IEnumerable<object> SelectedItems => m_ListView.selectedItems;

        public void Initialize(ListView listView, VisualTreeAsset itemTemplate, Action<IEnumerable<object>> onSelectionChange)
        {
            m_ListView = listView;

            m_ListView.makeItem = itemTemplate.Instantiate;
            m_ListView.bindItem = OnBindItem;

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

        public void UpdateList(IEnumerable<T> entries)
        {
            m_List = entries.ToList();
            m_ListView.itemsSource = m_List;
            m_ListView.RefreshItems();

            Show();
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
    }
}
#endif
