using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.MetadataManagement
{
    public class CreateFieldDefinitionPopupController : PopupController
    {
        public event Action<IFieldDefinitionCreation> FieldDefinitionCreated;

        readonly TextField m_NameInput;
        readonly TextField m_DisplayNameInput;
        readonly EnumField m_TypeInput;
        readonly RadioButton m_MultiSelectionInput;
        readonly ListView m_AcceptedValuesInput;
        readonly List<string> m_AcceptedValues = new();

        readonly Label m_ErrorLabel;

        readonly ValidateFieldDefinitionName m_ValidateFieldDefinitionName;

        readonly Dictionary<TextField, int> m_TextFieldToIndex = new();

        public CreateFieldDefinitionPopupController(VisualElement root, ValidateFieldDefinitionName validateFieldDefinitionName)
            : base(root, "CreateFieldPopup", null)
        {
            m_NameInput = m_PopupWindow.Q<TextField>("Name");
            m_NameInput.RegisterCallback<InputEvent>(OnInputChanged);

            m_DisplayNameInput = m_PopupWindow.Q<TextField>("DisplayName");
            m_DisplayNameInput.RegisterCallback<InputEvent>(OnInputChanged);

            m_TypeInput = m_PopupWindow.Q<EnumField>("Type");
            m_TypeInput.RegisterValueChangedCallback(evt => SetFieldDefinitionType(evt.newValue));

            m_MultiSelectionInput = m_PopupWindow.Q<RadioButton>("Multiselection");

            m_AcceptedValuesInput = m_PopupWindow.Q<ListView>("AcceptedValues");
            m_AcceptedValuesInput.itemsSource = m_AcceptedValues;
            m_AcceptedValuesInput.makeItem = () => new TextField();
            m_AcceptedValuesInput.bindItem = (element, i) =>
            {
                var textField = (TextField) element;
                textField.value = m_AcceptedValues[i];
                m_TextFieldToIndex[textField] = i;
                textField.RegisterValueChangedCallback(OnValueChanged);
            };
            m_AcceptedValuesInput.unbindItem = (element, _) =>
            {
                var textField = (TextField) element;
                textField.UnregisterValueChangedCallback(OnValueChanged);
                m_TextFieldToIndex.Remove(textField);
            };

            m_ErrorLabel = m_PopupWindow.Q<Label>("ErrorLabel");

            m_ActionButton.SetEnabled(false);
            SetFieldDefinitionType(m_TypeInput.value);

            m_ValidateFieldDefinitionName = validateFieldDefinitionName;
        }

        protected override void OnClicked()
        {
            var newField = CreateFieldDefinition((FieldDefinitionType) m_TypeInput.value);

            m_NameInput.SetValueWithoutNotify(string.Empty);
            m_DisplayNameInput.SetValueWithoutNotify(string.Empty);
            m_TypeInput.SetValueWithoutNotify(default(FieldDefinitionType));
            m_MultiSelectionInput.SetValueWithoutNotify(false);
            m_AcceptedValuesInput.Clear();
            m_AcceptedValues.Clear();

            SetFieldDefinitionType(m_TypeInput.value);

            FieldDefinitionCreated?.Invoke(newField);

            base.OnClicked();
        }

        IFieldDefinitionCreation CreateFieldDefinition(FieldDefinitionType type)
        {
            switch (type)
            {
                case FieldDefinitionType.Selection:
                    return new SelectionFieldDefinitionCreation
                    {
                        Key = m_NameInput.value.Trim(),
                        DisplayName = m_DisplayNameInput.value.Trim(),
                        Multiselection = m_MultiSelectionInput.value,
                        AcceptedValues = m_AcceptedValues.Where(x => !string.IsNullOrWhiteSpace(x)).ToList()
                    };
                default:
                    return new FieldDefinitionCreation
                    {
                        Key = m_NameInput.value.Trim(),
                        DisplayName = m_DisplayNameInput.value.Trim(),
                        Type = (FieldDefinitionType) m_TypeInput.value,
                    };
            }
        }

        void OnInputChanged(InputEvent _)
        {
            m_NameInput.SetValueWithoutNotify(m_NameInput.value.Trim());

            var (isValid, errorMsg) = m_ValidateFieldDefinitionName?.Invoke(m_NameInput.value) ?? (true, string.Empty);

            if (!string.IsNullOrEmpty(errorMsg))
            {
                m_ErrorLabel.style.display = DisplayStyle.Flex;
                m_ErrorLabel.text = errorMsg;
            }
            else
            {
                m_ErrorLabel.style.display = DisplayStyle.None;
                m_ErrorLabel.text = "Error: ";
            }

            m_ActionButton.SetEnabled(!string.IsNullOrWhiteSpace(m_DisplayNameInput.value) && isValid);
        }

        void OnValueChanged(ChangeEvent<string> evt)
        {
            var textField = (TextField) evt.target;
            if (m_TextFieldToIndex.TryGetValue(textField, out var index))
            {
                m_AcceptedValues[index] = evt.newValue;
            }
        }

        void SetFieldDefinitionType(Enum type)
        {
            var isSelection = (FieldDefinitionType)type == FieldDefinitionType.Selection;
            m_MultiSelectionInput.SetEnabled(isSelection);
            m_AcceptedValuesInput.SetEnabled(isSelection);
        }
    }
}
