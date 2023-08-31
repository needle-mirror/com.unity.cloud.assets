#if !UC_EXCLUDE_SAMPLES
using System;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.CollectionManagement
{
    public class CreateCollectionPopupController : PopupController
    {
        public event Action<IAssetCollection> CollectionCreated;

        readonly TextField m_NameInputField;
        readonly TextField m_DescriptionInputField;

        public CreateCollectionPopupController(VisualElement root)
            : base(root, "CreateCollectionPopup", null)
        {
            m_NameInputField = m_PopupWindow.Q<TextField>("Name");
            m_NameInputField.RegisterCallback<InputEvent>(OnInputChanged);
            m_DescriptionInputField = m_PopupWindow.Q<TextField>("Description");
            m_DescriptionInputField.RegisterCallback<InputEvent>(OnInputChanged);

            m_ActionButton.SetEnabled(false);
        }

        void OnInputChanged(InputEvent _)
        {
            var emptyString = string.IsNullOrWhiteSpace(m_NameInputField.value);
            emptyString |= string.IsNullOrWhiteSpace(m_DescriptionInputField.value);

            m_ActionButton.SetEnabled(!emptyString);
        }

        protected override void OnClicked()
        {
            var newCollection = new AssetCollection(m_NameInputField.value, m_DescriptionInputField.value);
            CollectionCreated?.Invoke(newCollection);

            base.OnClicked();
        }
    }
}
#endif
