using System;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public class TextInputPopupController : PopupController
    {
        readonly Label m_Title;
        readonly Label m_Message;
        readonly TextField m_Input;

        Action<string> m_OnAccept;

        public TextInputPopupController(VisualElement root)
            : base(root, "TextInputPopup", null)
        {
            m_Title = m_PopupWindow.Q<Label>("Title");
            m_Message = m_PopupWindow.Q<Label>("Message");
            m_Input = m_PopupWindow.Q<TextField>();
            m_Input.RegisterValueChangedCallback(evt => m_ActionButton.SetEnabled(!string.IsNullOrEmpty(evt.newValue)));
        }

        public void ShowMessage(string title, string message, Action<string> onAccept, string defaultValue = null)
        {
            m_Title.text = title;
            m_Message.text = message;
            m_Message.style.display = string.IsNullOrEmpty(message) ? DisplayStyle.None : DisplayStyle.Flex;

            m_Input.SetValueWithoutNotify(defaultValue ?? string.Empty);

            m_OnAccept = onAccept;

            Show();
        }

        protected override void OnClicked()
        {
            m_OnAccept?.Invoke(m_Input.value);

            base.OnClicked();
        }
    }
}
