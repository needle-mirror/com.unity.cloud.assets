using System;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public static class DialogService
    {
        static MessagePopupController m_MessagePopupController;
        static TextInputPopupController m_TextInputPopupController;

        public static void Initialize(VisualElement root)
        {
            m_MessagePopupController = new MessagePopupController(root);
            m_TextInputPopupController = new TextInputPopupController(root);
        }

        public static void ShowMessage(string title, string message)
        {
            m_MessagePopupController?.ShowMessage(title, message);
        }

        public static void ShowMessage(string title, Action<string> onAccept, string defaultValue = null)
        {
            m_TextInputPopupController?.ShowMessage(title, string.Empty, onAccept, defaultValue);
        }

        public static void ShowMessage(string title, string message, Action<string> onAccept, string defaultValue = null)
        {
            m_TextInputPopupController?.ShowMessage(title, message, onAccept, defaultValue);
        }

        public static void Hide()
        {
            m_MessagePopupController?.Hide();
            m_TextInputPopupController?.Hide();
        }
    }
}
