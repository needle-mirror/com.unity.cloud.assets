#if !UC_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public abstract class ListController<T>
    {
        T m_AllItem;
        protected List<T> m_List = new ();
        protected ListView m_ListView;

        public T AllItem
        {
            get => m_AllItem;
            set
            {
                if (!Equals(m_AllItem, value))
                {
                    m_AllItem = value;

                    if (m_List != null)
                        UpdateList(m_List);
                }
            }
        }

        public IEnumerable<T> List => m_List;
        protected virtual SelectionType SelectionType => SelectionType.Single;

        public void Initialize(ListView listView, VisualTreeAsset itemTemplate, Action<IEnumerable<object>> onSelectionChange)
        {
            m_ListView = listView;

            m_ListView.makeItem = itemTemplate.Instantiate;
            m_ListView.bindItem = OnBindItem;

            m_ListView.selectionType = SelectionType;
            m_ListView.onSelectionChange += onSelectionChange;
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
            if(m_ListView == null)
                return;

            if (m_ListView.selectedItem != null)
                m_ListView.ClearSelection();

            if (AllItem != null)
            {
                m_List.Clear();
                m_List.Add(AllItem);
                m_List.AddRange(entries.ToList());
            }
            else
            {
                m_List = entries.ToList();
            }

            m_ListView.itemsSource = m_List;
            m_ListView.RefreshItems();// Ensure UI is updated after itemsSource changed

            Show();
        }

        protected abstract void OnBindItem(VisualElement element, int i);
    }
}
#endif
