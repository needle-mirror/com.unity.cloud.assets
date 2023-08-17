#if !UC_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public abstract class ListUi<T, U> where T : ListController<U>, new()
    {
        protected delegate Task OnEntryRetrieved(U entry);

        VisualElement m_Container;
        VisualElement m_DisplayMessageContainer;
        Label m_DisplayMessage;

        protected List<U> m_Entries;

        protected readonly T m_ListController = new();

        protected abstract string VisualElementName { get; }

        protected abstract string EmptyListMessage { get; }

        public void Initialize(VisualElement uiDocumentRoot, VisualTreeAsset listItemTemplate)
        {
            m_Container = uiDocumentRoot.Q<VisualElement>(VisualElementName);
            m_DisplayMessageContainer = m_Container.Q<VisualElement>("DisplayMessageContainer");
            m_DisplayMessage = m_Container.Q<Label>("DisplayMessage");
            var listView = m_Container.Q<ListView>("List");

            m_ListController.Initialize(listView, listItemTemplate, OnSelectionChange);
        }

        public virtual void AddAllItem() { }

        public void Show()
        {
            m_Container.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            m_Container.style.display = DisplayStyle.None;
        }

        protected async Task UpdateList(IAsyncEnumerable<U> asyncEntries, CancellationToken token, OnEntryRetrieved onEntryRetrieved = null)
        {
            var startTime = DateTime.UtcNow;

            var entries = new List<U>();
            await foreach (var entry in asyncEntries.WithCancellation(token))
            {
                entries.Add(entry);
                if (onEntryRetrieved != null) await onEntryRetrieved(entry);

                if (DateTime.UtcNow - startTime > TimeSpan.FromSeconds(0.6f))
                {
                    startTime = DateTime.UtcNow;
                    UpdateList(entries);
                }
            }

            if (!token.IsCancellationRequested)
            {
                UpdateList(entries);
            }
        }

        protected void UpdateList(IEnumerable<U> entries)
        {
            var entryArray = entries as U[] ?? entries.ToArray();
            if (entryArray.Any())
            {
                m_DisplayMessageContainer.style.display = DisplayStyle.None;

                m_Entries = entryArray.ToList();
                m_ListController.UpdateList(entryArray);
            }
            else
            {
                SetDisplayMessage(EmptyListMessage);
            }
        }

        protected void SetDisplayMessage(string message)
        {
            m_ListController.Hide();
            m_DisplayMessageContainer.style.display = DisplayStyle.Flex;
            m_DisplayMessage.text = message;
        }

        protected abstract void OnSelectionChange(IEnumerable<object> selectedItems);
    }
}
#endif
