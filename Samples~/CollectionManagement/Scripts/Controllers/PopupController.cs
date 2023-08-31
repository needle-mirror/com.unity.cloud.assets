#if !UC_EXCLUDE_SAMPLES
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.CollectionManagement
{
    public class PopupController
    {
        protected readonly VisualElement m_PopupWindow;
        protected readonly Button m_ActionButton;
        readonly Action m_Action;

        public PopupController(VisualElement root, string popupName, Action action)
        {
            m_PopupWindow = root.Q(popupName);
            m_Action = action;

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
            m_Action?.Invoke();
            Hide();
        }
    }
}
#endif
