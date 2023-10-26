#if !UC_EXCLUDE_SAMPLES
using System;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.CollectionManagement
{
    public class CreateCollectionPopupController : PopupController
    {
        public event Action<IAssetCollectionCreation> CollectionCreated;

        readonly TextField m_NameInput;
        readonly TextField m_DescriptionInput;
        readonly TextField m_ParentPathInput;
        readonly Label m_ErrorLabel;

        readonly ValidateCollectionName m_ValidateCollectionName;

        public CreateCollectionPopupController(VisualElement root, ValidateCollectionName validateCollectionName)
            : base(root, "CreateCollectionPopup", null)
        {
            m_NameInput = m_PopupWindow.Q<TextField>("Name");
            m_NameInput.RegisterCallback<InputEvent>(OnInputChanged);
            m_DescriptionInput = m_PopupWindow.Q<TextField>("Description");
            m_DescriptionInput.RegisterCallback<InputEvent>(OnInputChanged);
            m_ParentPathInput = m_PopupWindow.Q<TextField>("ParentPath");
            m_ErrorLabel = m_PopupWindow.Q<Label>("ErrorLabel");

            m_ActionButton.SetEnabled(false);

            m_ValidateCollectionName = validateCollectionName;
        }

        void OnInputChanged(InputEvent _)
        {
            var (isValid, errorMsg) = m_ValidateCollectionName?.Invoke(m_NameInput.value) ?? (true, string.Empty);

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

            m_ActionButton.SetEnabled(!string.IsNullOrWhiteSpace(m_DescriptionInput.value) && isValid);
        }

        protected override void OnClicked()
        {
            var newCollection = new AssetCollectionCreation(m_NameInput.value, m_DescriptionInput.value);
            var parentPath = m_ParentPathInput.value;
            if (!string.IsNullOrEmpty(parentPath.Trim()))
                newCollection.ParentPath = new CollectionPath(parentPath);

            m_NameInput.value = string.Empty;
            m_DescriptionInput.value = string.Empty;
            m_ParentPathInput.value = string.Empty;

            CollectionCreated?.Invoke(newCollection);

            base.OnClicked();
        }
    }
}
#endif
