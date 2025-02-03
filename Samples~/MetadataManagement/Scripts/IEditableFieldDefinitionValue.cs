using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.MetadataManagement
{
    public interface IEditableFieldDefinitionValue
    {
        void Initialize(FieldDefinitionProperties fieldDefinitionProperties);
        void SetEditable(bool editable);
        Task OnUpdateAsync(IFieldDefinition fieldDefinition, CancellationToken cancellationToken);
    }

    public class EditableDisplayName : IEditableFieldDefinitionValue
    {
        readonly Label m_DisplayName;
        readonly TextField m_EditableDisplayName;

        public EditableDisplayName(VisualElement rootVisualElement)
        {
            m_DisplayName = rootVisualElement.Q<Label>();
            m_EditableDisplayName = rootVisualElement.Q<TextField>();

            SetEditable(false);
        }

        public void Initialize(FieldDefinitionProperties fieldDefinitionProperties)
        {
            var deletedTag = fieldDefinitionProperties.IsDeleted ? " (Deleted)" : "";
            m_DisplayName.text = $"{fieldDefinitionProperties.DisplayName}{deletedTag}";
            m_EditableDisplayName.value = fieldDefinitionProperties.DisplayName;
        }

        public void SetEditable(bool editable)
        {
            m_DisplayName.style.display = editable ? DisplayStyle.None : DisplayStyle.Flex;
            m_EditableDisplayName.style.display = editable ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public Task OnUpdateAsync(IFieldDefinition fieldDefinition, CancellationToken cancellationToken)
        {
            var update = new FieldDefinitionUpdate
            {
                DisplayName = m_EditableDisplayName.value
            };
            return fieldDefinition.UpdateAsync(update, cancellationToken);
        }
    }

    public class EditableSelection : IEditableFieldDefinitionValue
    {
        readonly Label m_Multiselection;
        readonly VisualElement m_AcceptedValuesRoot;
        readonly Label m_AcceptedValues;
        readonly ListView m_EditableAcceptedValues;

        readonly List<string> m_AcceptedValuesList = new();

        bool m_IsSelection;

        public EditableSelection(VisualElement rootVisualElement)
        {
            m_Multiselection = rootVisualElement.Q<Label>("Multiselection");

            m_AcceptedValuesRoot = rootVisualElement.Q("AcceptedValues");
            m_AcceptedValues = m_AcceptedValuesRoot.Q<Label>();
            m_EditableAcceptedValues = m_AcceptedValuesRoot.Q<ListView>();
            m_EditableAcceptedValues.makeItem = () => new TextField();
            m_EditableAcceptedValues.bindItem = BindItem;
            m_EditableAcceptedValues.unbindItem = UnbindItem;

            SetEditable(false);
        }

        public void Initialize(FieldDefinitionProperties fieldDefinitionProperties)
        {
            m_IsSelection = fieldDefinitionProperties.Type == FieldDefinitionType.Selection;
            if (!m_IsSelection)
            {
                m_Multiselection.style.display = DisplayStyle.None;
                m_AcceptedValuesRoot.style.display = DisplayStyle.None;
                return;
            }

            var selectionProperties = fieldDefinitionProperties.AsSelectionFieldDefinitionProperties();

            m_AcceptedValuesList.Clear();
            m_AcceptedValuesList.AddRange(selectionProperties.AcceptedValues);

            m_Multiselection.style.display = DisplayStyle.Flex;
            m_AcceptedValuesRoot.style.display = DisplayStyle.Flex;

            m_Multiselection.text = selectionProperties.Multiselection ? "Multi selection" : "Single selection";

            m_AcceptedValues.text = $"Accepted Values: {string.Join(", ", selectionProperties.AcceptedValues)}";
            m_EditableAcceptedValues.itemsSource = m_AcceptedValuesList;
            m_EditableAcceptedValues.Rebuild();
            m_EditableAcceptedValues.itemsAdded += OnItemsAdded;
        }

        public void SetEditable(bool editable)
        {
            m_AcceptedValues.style.display = editable ? DisplayStyle.None : DisplayStyle.Flex;
            m_EditableAcceptedValues.style.display = editable ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public Task OnUpdateAsync(IFieldDefinition fieldDefinition, CancellationToken cancellationToken)
        {
            if (!m_IsSelection)
            {
                return Task.CompletedTask;
            }

            var selectionFieldDefinition = fieldDefinition.AsSelectionFieldDefinition();
            return selectionFieldDefinition.SetSelectionValuesAsync(m_AcceptedValuesList, cancellationToken);
        }

        void BindItem(VisualElement element, int i)
        {
            var textField = (TextField) element;
            textField.SetValueWithoutNotify(m_AcceptedValuesList[i]);
            textField.RegisterValueChangedCallback(OnValueChanged);
        }

        void UnbindItem(VisualElement element, int i)
        {
            var textField = (TextField) element;
            textField.SetValueWithoutNotify(string.Empty);
            textField.UnregisterValueChangedCallback(OnValueChanged);
        }

        void OnValueChanged(ChangeEvent<string> evt)
        {
            var index = m_AcceptedValuesList.IndexOf(evt.previousValue);
            if (index >= 0)
            {
                m_AcceptedValuesList[index] = evt.newValue;
            }
        }

        void OnItemsAdded(IEnumerable<int> obj)
        {
            foreach (var index in obj)
            {
                m_AcceptedValuesList[index] = string.Empty;
            }
        }
    }
}
