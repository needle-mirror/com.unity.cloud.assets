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

        IFieldDefinition m_FieldDefinition;

        public event Action<Func<CancellationToken, Task>> UpdateFieldDefinition;

        public FieldDefinitionPanelController(VisualElement rootVisualElement)
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
                UpdateFieldDefinition?.Invoke(UpdateFieldDefinitionAsync);
            });

            Hide();
        }

        async Task UpdateFieldDefinitionAsync(CancellationToken cancellationToken)
        {
            var update = new FieldDefinitionUpdate(m_FieldDefinition)
            {
                DisplayName = ((EditableDisplayName) m_EditableFieldDefinitionValues[0]).DisplayName
            };
            await m_FieldDefinition.UpdateAsync(update, cancellationToken);

            if (m_FieldDefinition.Type == FieldDefinitionType.Selection)
            {
                await m_FieldDefinition
                    .AsSelectionFieldDefinition()
                    .SetSelectionValuesAsync(((EditableSelection) m_EditableFieldDefinitionValues[1]).AcceptedValues, cancellationToken);
            }
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
            m_Origin.text = $"Origin: {m_FieldDefinition.Origin.ToString()}";
            m_AuthoringInfo_Created.text = $"Created: {m_FieldDefinition.AuthoringInfo?.Created.ToString() ?? "unknown"}";
            m_AuthoringInfo_Updated.text = $"Updated: {m_FieldDefinition.AuthoringInfo?.Updated.ToString() ?? "unknown"}";
            m_Type.text = $"Type: {m_FieldDefinition.Type.ToString()}";

            foreach (var editableField in m_EditableFieldDefinitionValues)
            {
                editableField.Initialize(m_FieldDefinition);
            }
        }

        public void Hide()
        {
            SetEditEnabled(false);
            m_RootVisualElement.style.display = DisplayStyle.None;
        }

        public void SetEditEnabled(bool isEditable)
        {
            foreach (var editableField in m_EditableFieldDefinitionValues)
            {
                editableField.SetEditable(isEditable);
            }
            m_Update.style.display = isEditable ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
