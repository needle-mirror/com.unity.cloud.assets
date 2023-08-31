#if !UC_EXCLUDE_SAMPLES
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.CollectionManagement
{
    public class ContextMenuController
    {
        readonly VisualElement m_ContextMenu;
        readonly Button m_ContextMenuButton;

        public ContextMenuController(VisualElement root)
        {
            m_ContextMenu = root.Q("ContextMenu");
            m_ContextMenu.style.display = DisplayStyle.None;

            m_ContextMenuButton = root.Q<Button>("ContextMenuButton");
            m_ContextMenuButton.clicked += ToggleContextMenu;
        }

        public void RegisterButtonAction(string name, Action clicked)
        {
            var button = m_ContextMenu.Q<Button>(name);
            button.clicked += clicked;
            button.clicked += ToggleContextMenu;
        }

        public void UnregisterButtonAction(string name, Action clicked)
        {
            var button = m_ContextMenu.Q<Button>(name);
            button.clicked -= clicked;
        }

        public void SetButtonVisibility(string name, bool show)
        {
            var button = m_ContextMenu.Q<Button>(name);
            button.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void SetEnabled(bool isEnabled)
        {
            m_ContextMenuButton.style.display = isEnabled ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public virtual void Hide()
        {
            m_ContextMenu.style.display = DisplayStyle.None;
        }

        void ToggleContextMenu()
        {
            m_ContextMenu.style.display = m_ContextMenu.style.display == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
#endif
