using System;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public class MessagePopupController : PopupController
    {
        readonly Label m_Title;
        readonly Label m_Message;
        readonly Button m_CancelButton;

        Action m_OnAccept;
        Action m_OnCancel;

        public MessagePopupController(VisualElement root)
            : base(root, "MessagePopup")
        {
            m_Title = m_PopupWindow.Q<Label>("Title");
            m_Message = m_PopupWindow.Q<Label>("Message");
            m_CancelButton = m_PopupWindow.Q<Button>("Cancel");
            if (m_CancelButton != null)
                m_CancelButton.clicked += OnCancel;
        }

        public void ShowMessage(string title, string message)
        {
            m_Title.text = title;
            m_Message.text = message;

            m_OnAccept = null;
            m_OnCancel = null;

            m_CancelButton.style.display = DisplayStyle.None;

            Show();
        }

        public void ShowMessage(string title, string message, Action onAccept, Action onCancel)
        {
            m_Title.text = title;
            m_Message.text = message;

            m_OnAccept = onAccept;
            m_OnCancel = onCancel;

            m_CancelButton.style.display = DisplayStyle.Flex;

            Show();
        }

        protected override void OnClicked()
        {
            m_OnAccept?.Invoke();

            base.OnClicked();
        }

        void OnCancel()
        {
            m_OnCancel?.Invoke();
            Hide();
        }

    }
}
