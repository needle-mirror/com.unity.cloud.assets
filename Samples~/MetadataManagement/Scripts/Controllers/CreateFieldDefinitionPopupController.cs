using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.MetadataManagement
{
    public class CreateFieldDefinitionPopupController : PopupController
    {
        class AcceptedValue : TextField
        {
            int m_Index = -1;

            public AcceptedValue(Action<string, int> onValueChanged)
            {
                this.RegisterValueChangedCallback(evt =>
                {
                    onValueChanged?.Invoke(evt.newValue, m_Index);
                });
            }

            public void SetValueWithoutNotify(string newValue, int index)
            {
                m_Index = index;
                SetValueWithoutNotify(newValue);
            }
        }

        public event Action<IFieldDefinitionCreation> FieldDefinitionCreated;

        readonly TextField m_NameInput;
        readonly TextField m_DisplayNameInput;
        readonly EnumField m_TypeInput;
        readonly Toggle m_MultiSelectionInput;
        readonly ListView m_AcceptedValuesInput;
        readonly List<string> m_AcceptedValues = new();

        readonly Label m_ErrorLabel;

        readonly ValidateFieldDefinitionName m_ValidateFieldDefinitionName;

        public CreateFieldDefinitionPopupController(VisualElement root, ValidateFieldDefinitionName validateFieldDefinitionName)
            : base(root, "CreateFieldPopup")
        {
            m_NameInput = m_PopupWindow.Q<TextField>("Name");
            m_NameInput.RegisterCallback<InputEvent>(OnInputChanged);

            m_DisplayNameInput = m_PopupWindow.Q<TextField>("DisplayName");
            m_DisplayNameInput.RegisterCallback<InputEvent>(OnInputChanged);

            m_TypeInput = m_PopupWindow.Q<EnumField>("Type");
            m_TypeInput.RegisterValueChangedCallback(evt => SetFieldDefinitionType(evt.newValue));

            m_MultiSelectionInput = m_PopupWindow.Q<Toggle>("Multiselection");

            m_AcceptedValuesInput = m_PopupWindow.Q<ListView>("AcceptedValues");
            m_AcceptedValuesInput.itemsSource = m_AcceptedValues;
            m_AcceptedValuesInput.makeItem = () => new AcceptedValue(OnValueChanged);
            m_AcceptedValuesInput.bindItem = (element, i) =>
            {
                var acceptedValue = (AcceptedValue) element;
                acceptedValue.SetValueWithoutNotify(m_AcceptedValues[i] ?? string.Empty, i);
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

            m_AcceptedValuesInput.Rebuild();
            SetFieldDefinitionType(m_TypeInput.value);
            m_ActionButton.SetEnabled(false);

            FieldDefinitionCreated?.Invoke(newField);

            base.OnClicked();
        }

        IFieldDefinitionCreation CreateFieldDefinition(FieldDefinitionType type)
        {
            return type switch
            {
                FieldDefinitionType.Selection => new SelectionFieldDefinitionCreation
                {
                    Key = m_NameInput.value.Trim(),
                    DisplayName = m_DisplayNameInput.value.Trim(),
                    Multiselection = m_MultiSelectionInput.value,
                    AcceptedValues = m_AcceptedValues.Where(x => !string.IsNullOrWhiteSpace(x)).ToList()
                },
                _ => new FieldDefinitionCreation
                {
                    Key = m_NameInput.value.Trim(),
                    DisplayName = m_DisplayNameInput.value.Trim(),
                    Type = (FieldDefinitionType) m_TypeInput.value,
                }
            };
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

        void OnValueChanged(string newValue, int index)
        {
            if (index < 0 || index >= m_AcceptedValues.Count) return;

            m_AcceptedValues[index] = newValue;
        }

        void SetFieldDefinitionType(Enum type)
        {
            var isSelection = (FieldDefinitionType) type == FieldDefinitionType.Selection;
            m_MultiSelectionInput.SetEnabled(isSelection);
            m_AcceptedValuesInput.SetEnabled(isSelection);
        }
    }
}
