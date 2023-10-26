#if !UC_EXCLUDE_SAMPLES
using System;
using System.ComponentModel;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public abstract class DialogController : IDialogController
    {
        protected VisualElement m_DialogPanel;
        Button m_OkButton;

        protected event Action OkClicked
        {
            add => m_OkButton.clicked += value;
            remove => m_OkButton.clicked -= value;
        }

        public event Action Opened;

        public event Action Closed;

        public virtual void Init(VisualElement dialogPanel, string title)
        {
            m_DialogPanel = dialogPanel;

            var titleLabel = m_DialogPanel.Q<Label>("Title");
            titleLabel.text = title;

            m_OkButton = m_DialogPanel.Q<Button>("OkButton");
        }

        protected virtual void ShowDialog()
        {
            Opened?.Invoke();
            m_DialogPanel.style.display = DisplayStyle.Flex;
        }

        protected virtual void CloseDialog()
        {
            m_DialogPanel.style.display = DisplayStyle.None;
            Closed?.Invoke();
        }
    }
}
#endif
