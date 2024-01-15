using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public abstract class ListUi<T, U> where T : ListController<U>, new()
    {
        public event Action ListUpdated;

        protected delegate Task OnEntryRetrieved(U entry);

        VisualElement m_Container;
        VisualElement m_DisplayMessageContainer;
        Label m_DisplayMessage;

        protected List<U> m_Entries;

        protected readonly T m_ListController = new();

        protected abstract string VisualElementName { get; }

        protected abstract string EmptyListMessage { get; }

        public virtual void Initialize(VisualElement uiDocumentRoot, VisualTreeAsset listItemTemplate)
        {
            m_Container = uiDocumentRoot.Q<VisualElement>(VisualElementName);
            m_DisplayMessageContainer = m_Container.Q<VisualElement>("DisplayMessageContainer");
            m_DisplayMessage = m_Container.Q<Label>("DisplayMessage");
            var listView = m_Container.Q<ListView>();

            m_ListController.Initialize(listView, listItemTemplate, OnSelectionChange);
        }

        public void SetName(string name)
        {
            m_Container.Q<Label>("ListLabel").text = name;
        }

        public void Show()
        {
            m_Container.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            m_Container.style.display = DisplayStyle.None;
        }

        public void ClearSelection()
        {
            m_ListController.ClearSelection();
        }

        protected async Task UpdateList(IEnumerable<U> existingEntries, IAsyncEnumerable<U> asyncEntries, CancellationToken token, OnEntryRetrieved onEntryRetrieved = null)
        {
            if (token.IsCancellationRequested) return;

            var startTime = DateTime.UtcNow;

            m_ListController.ClearList();

            var entries = new List<U>();
            if (existingEntries != null)
            {
                entries.AddRange(existingEntries);
                UpdateList(entries);
            }

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
            m_Entries = entryArray.ToList();

            if (entryArray.Any())
            {
                m_DisplayMessageContainer.style.display = DisplayStyle.None;

                m_ListController.UpdateList(entryArray);
            }
            else
            {
                SetDisplayMessage(EmptyListMessage);
            }

            ListUpdated?.Invoke();
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
