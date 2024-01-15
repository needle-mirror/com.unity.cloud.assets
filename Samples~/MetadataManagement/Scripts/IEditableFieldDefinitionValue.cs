using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cloud.Assets;
using UnityEngine;
using UnityEngine.UIElements;

namespace Samples.MetadataManagement.Scripts
{
    public interface IEditableFieldDefinitionValue
    {
        void Initialize(IFieldDefinition fieldDefinition);
        void SetEditable(bool editable);
    }

    public class EditableDisplayName : IEditableFieldDefinitionValue
    {
        readonly Label m_DisplayName;
        readonly TextField m_EditableDisplayName;

        public string DisplayName => m_EditableDisplayName.value;

        public EditableDisplayName(VisualElement rootVisualElement)
        {
            m_DisplayName = rootVisualElement.Q<Label>();
            m_EditableDisplayName = rootVisualElement.Q<TextField>();

            SetEditable(false);
        }

        public void Initialize(IFieldDefinition fieldDefinition)
        {
            var deletedTag = fieldDefinition.Status == "Deleted" ? " (Deleted)" : "";
            m_DisplayName.text = $"{fieldDefinition.DisplayName}{deletedTag}";
            m_EditableDisplayName.value = fieldDefinition.DisplayName;
        }

        public void SetEditable(bool editable)
        {
            m_DisplayName.style.display = editable ? DisplayStyle.None : DisplayStyle.Flex;
            m_EditableDisplayName.style.display = editable ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    public class EditableSelection : IEditableFieldDefinitionValue
    {
        readonly Label m_Multiselection;
        readonly VisualElement m_AcceptedValuesRoot;
        readonly Label m_AcceptedValues;
        readonly ListView m_EditableAcceptedValues;

        public List<string> AcceptedValues { get; } = new();

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

        public void Initialize(IFieldDefinition fieldDefinition)
        {
            if (fieldDefinition is not ISelectionFieldDefinition selectionFieldDefinition)
            {
                m_Multiselection.style.display = DisplayStyle.None;
                m_AcceptedValuesRoot.style.display = DisplayStyle.None;
                return;
            }

            AcceptedValues.Clear();
            AcceptedValues.AddRange(selectionFieldDefinition.AcceptedValues);

            m_Multiselection.style.display = DisplayStyle.Flex;
            m_AcceptedValuesRoot.style.display = DisplayStyle.Flex;

            m_Multiselection.text = selectionFieldDefinition.Multiselection ? "Multi selection" : "Single selection";

            m_AcceptedValues.text = $"Accepted Values: {string.Join(", ", selectionFieldDefinition.AcceptedValues)}";
            m_EditableAcceptedValues.itemsSource = AcceptedValues;
            m_EditableAcceptedValues.Rebuild();
            m_EditableAcceptedValues.itemsAdded += OnItemsAdded;
            m_EditableAcceptedValues.itemsRemoved += OnItemsRemoved;
            m_EditableAcceptedValues.itemIndexChanged += OnItemIndexChanged;
        }

        public void SetEditable(bool editable)
        {
            m_AcceptedValues.style.display = editable ? DisplayStyle.None : DisplayStyle.Flex;
            m_EditableAcceptedValues.style.display = editable ? DisplayStyle.Flex : DisplayStyle.None;
        }

        void BindItem(VisualElement element, int i)
        {
            var textField = (TextField) element;
            textField.SetValueWithoutNotify(AcceptedValues[i]);
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
            var index = AcceptedValues.IndexOf(evt.previousValue);
            if (index >= 0)
            {
                AcceptedValues[index] = evt.newValue;
            }
        }

        void OnItemsRemoved(IEnumerable<int> obj)
        {
            Debug.LogWarning(string.Join(", ", AcceptedValues));
        }

        void OnItemsAdded(IEnumerable<int> obj)
        {
            foreach (var index in obj)
            {
                AcceptedValues[index] = string.Empty;
            }
            Debug.LogWarning(string.Join(", ", AcceptedValues));
        }

        void OnItemIndexChanged(int arg1, int arg2)
        {
            Debug.LogWarning(string.Join(", ", AcceptedValues));
        }
    }
}
