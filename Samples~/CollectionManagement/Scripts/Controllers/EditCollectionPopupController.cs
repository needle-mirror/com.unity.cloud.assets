using System;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.CollectionManagement
{
    public class EditCollectionPopupController : PopupController
    {
        readonly TextField m_NameInput;
        readonly TextField m_DescriptionInput;
        readonly Label m_ErrorLabel;

        readonly ValidateCollectionName m_ValidateCollectionName;

        IAssetCollection m_AssetCollection;

        public event Action<IAssetCollectionUpdate> UpdateCollection;

        public EditCollectionPopupController(VisualElement root, ValidateCollectionName validateCollectionName)
            : base(root, "EditCollectionPopup")
        {
            m_NameInput = m_PopupWindow.Q<TextField>("Name");
            m_NameInput.RegisterCallback<InputEvent>(OnInputChanged);
            m_DescriptionInput = m_PopupWindow.Q<TextField>("Description");
            m_ErrorLabel = m_PopupWindow.Q<Label>("ErrorLabel");

            m_ValidateCollectionName = validateCollectionName;
        }

        public void SetAssetCollection(IAssetCollection assetCollection)
        {
            m_AssetCollection = assetCollection;
            m_NameInput.value = m_AssetCollection.Name;
            m_DescriptionInput.value = m_AssetCollection.Description;
        }

        void OnInputChanged(InputEvent _)
        {
            var (isValid, errorMsg) = (true, string.Empty);

            var trimmedName = m_NameInput.value.Trim();
            if (m_AssetCollection.Name != trimmedName)
                (isValid, errorMsg) = m_ValidateCollectionName?.Invoke(trimmedName) ?? (true, string.Empty);

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

            m_ActionButton.SetEnabled(isValid);
        }

        protected override void OnClicked()
        {
            m_ErrorLabel.style.display = DisplayStyle.None;
            m_ErrorLabel.text = "Error: ";

            var update = new AssetCollectionUpdate
            {
                Name = m_NameInput.value.Trim(),
                Description = m_DescriptionInput.value
            };

            m_NameInput.value = string.Empty;
            m_DescriptionInput.value = string.Empty;

            UpdateCollection?.Invoke(update);

            base.OnClicked();
        }
    }
}
