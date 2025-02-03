using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.MetadataManagement
{
    public class FieldDefinitionPanelController
    {
        readonly VisualElement m_RootVisualElement;

        readonly Label m_Key;
        readonly Label m_Origin;
        readonly Label m_AuthoringInfo_Created;
        readonly Label m_AuthoringInfo_Updated;
        readonly Label m_Type;
        readonly List<IEditableFieldDefinitionValue> m_EditableFieldDefinitionValues = new();

        readonly Button m_Update;

        readonly ContextMenuController m_InfoPanelContextMenu;

        IFieldDefinition m_FieldDefinition;

        public event Action<string, Func<IFieldDefinition, Task>> UpdateFieldDefinition;
        public event Action<string> DeleteFieldDefinition;

        public FieldDefinitionPanelController(VisualElement rootVisualElement, ContextMenuController contextMenuController)
        {
            m_RootVisualElement = rootVisualElement;

            m_Key = rootVisualElement.Q<Label>("Key");

            m_Origin = rootVisualElement.Q<Label>("Origin");
            m_AuthoringInfo_Created = rootVisualElement.Q<Label>("AuthoringInfo-Created");
            m_AuthoringInfo_Updated = rootVisualElement.Q<Label>("AuthoringInfo-Updated");
            m_Type = rootVisualElement.Q<Label>("Type");

            m_EditableFieldDefinitionValues.Add(new EditableDisplayName(rootVisualElement.Q("DisplayName")));
            m_EditableFieldDefinitionValues.Add(new EditableSelection(rootVisualElement));

            m_Update = rootVisualElement.Q<Button>("Update");
            m_Update.RegisterCallback<ClickEvent>(_ =>
            {
                if (m_FieldDefinition == null) return;
                SetEditEnabled(false);
                UpdateFieldDefinition?.Invoke(m_FieldDefinition.Descriptor.FieldKey, UpdateFieldDefinitionAsync);
            });

            m_InfoPanelContextMenu = contextMenuController;
            m_InfoPanelContextMenu.RegisterButtonAction("Edit", () => SetEditEnabled(true));
            m_InfoPanelContextMenu.RegisterButtonAction("StopEdit", () => SetEditEnabled(false), "Stop Editing");
            m_InfoPanelContextMenu.SetButtonVisibility("StopEdit", false);
            m_InfoPanelContextMenu.RegisterButtonAction("Delete", () =>
            {
                if (m_FieldDefinition == null) return;
                DeleteFieldDefinition?.Invoke(m_FieldDefinition.Descriptor.FieldKey);
            });

            Hide();
        }

        public void SetFieldDefinition(IFieldDefinition fieldDefinition)
        {
            m_FieldDefinition = fieldDefinition;

            if (m_FieldDefinition == null)
            {
                Hide();
                return;
            }

            m_RootVisualElement.style.display = DisplayStyle.Flex;

            m_Key.text = $"{m_FieldDefinition.Descriptor.FieldKey}";

            _ = PopulateAsync(fieldDefinition);
        }

        public void Hide()
        {
            SetEditEnabled(false);
            m_RootVisualElement.style.display = DisplayStyle.None;
        }

        void SetEditEnabled(bool isEditable)
        {
            m_InfoPanelContextMenu.SetButtonVisibility("Edit", !isEditable);
            m_InfoPanelContextMenu.SetButtonVisibility("StopEdit", isEditable);

            foreach (var editableField in m_EditableFieldDefinitionValues)
            {
                editableField.SetEditable(isEditable);
            }

            m_Update.style.display = isEditable ? DisplayStyle.Flex : DisplayStyle.None;
        }

        async Task PopulateAsync(IFieldDefinition fieldDefinition)
        {
            var properties = await fieldDefinition.GetPropertiesAsync(CancellationToken.None);

            m_InfoPanelContextMenu.SetEnabled(!properties.IsDeleted);

            m_Origin.text = $"Origin: {properties.Origin.ToString()}";
            m_AuthoringInfo_Created.text = $"Created: {properties.AuthoringInfo?.Created.ToString() ?? "unknown"}";
            m_AuthoringInfo_Updated.text = $"Updated: {properties.AuthoringInfo?.Updated.ToString() ?? "unknown"}";
            m_Type.text = $"Type: {properties.Type.ToString()}";

            foreach (var editableField in m_EditableFieldDefinitionValues)
            {
                editableField.Initialize(properties);
            }
        }

        async Task UpdateFieldDefinitionAsync(IFieldDefinition fieldDefinition)
        {
            var tasks = new List<Task>();

            foreach (var editableField in m_EditableFieldDefinitionValues)
            {
                tasks.Add(editableField.OnUpdateAsync(fieldDefinition, CancellationToken.None));
            }

            await Task.WhenAll(tasks);
        }
    }
}
