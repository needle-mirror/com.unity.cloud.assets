using System;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public class PopupController
    {
        protected readonly VisualElement m_PopupWindow;
        protected readonly Button m_ActionButton;

        protected PopupController(VisualElement root, string popupName)
        {
            m_PopupWindow = root.Q(popupName);

            var closeButton = m_PopupWindow.Q<Button>("Close");
            closeButton.clicked += Hide;

            m_ActionButton = m_PopupWindow.Q<Button>("Action");
            m_ActionButton.clicked += OnClicked;

            Hide();
        }

        public void Show()
        {
            m_PopupWindow.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            m_PopupWindow.style.display = DisplayStyle.None;
        }

        protected virtual void OnClicked()
        {
            Hide();
        }
    }
}
