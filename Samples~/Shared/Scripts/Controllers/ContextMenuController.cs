using System;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public class ContextMenuController
    {
        readonly VisualElement m_ContextMenu;
        readonly Button m_ContextMenuButton;
        readonly VisualTreeAsset m_TemplateOption;

        public ContextMenuController(VisualElement root)
        {
            m_ContextMenu = root.Q("ContextMenu");
            m_ContextMenu.style.display = DisplayStyle.None;

            m_ContextMenuButton = root.Q<Button>("ContextMenuButton");
            m_ContextMenuButton.clicked += ToggleContextMenu;

            m_TemplateOption = m_ContextMenu.Q<TemplateContainer>()?.templateSource;
        }

        public void RegisterButtonAction(string id, Action clicked, string name = null)
        {
            var button = m_ContextMenu.Q<Button>(id);
            // Create a new button if not found
            if (button == null)
            {
                button = m_TemplateOption == null ? new Button() : m_TemplateOption.Instantiate().Q<Button>();
                button.name = id;
                button.text = string.IsNullOrEmpty(name) ? id : name;
                m_ContextMenu.Add(button);
            }
            // If new name is provided, update the button text
            else if (!string.IsNullOrEmpty(name))
            {
                button.text = name;
            }
            button.clicked += clicked;
            button.clicked += ToggleContextMenu;
        }

        public void UnregisterButtonAction(string id, Action clicked)
        {
            var button = m_ContextMenu.Q<Button>(id);
            button.clicked -= clicked;
        }

        public void SetButtonVisibility(string id, bool show)
        {
            var button = m_ContextMenu.Q<Button>(id);
            button.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void SetEnabled(bool isEnabled)
        {
            Hide();
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
