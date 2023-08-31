#if !UC_EXCLUDE_SAMPLES
using System;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.CollectionManagement
{
    public class EditCollectionPopupController : PopupController
    {
        readonly TextField m_Name;
        readonly TextField m_Description;
        readonly Label m_ErrorLabel;

        IAssetCollection m_AssetCollection;

        public EditCollectionPopupController(VisualElement root, Action action)
            : base(root, "EditCollectionPopup", action)
        {
            m_Name = m_PopupWindow.Q<TextField>("Name");
            m_Description = m_PopupWindow.Q<TextField>("Description");
            m_ErrorLabel = m_PopupWindow.Q<Label>("ErrorLabel");
        }

        public void SetAssetCollection(IAssetCollection assetCollection)
        {
            m_AssetCollection = assetCollection;
            m_Name.value = m_AssetCollection.Name;
            m_Description.value = m_AssetCollection.Description;
        }

        protected override void OnClicked()
        {
            m_ErrorLabel.style.display = DisplayStyle.None;
            m_ErrorLabel.text = "Error: ";

            try
            {
                m_AssetCollection.SetName(m_Name.value);
            }
            catch (Exception)
            {
                m_ErrorLabel.text += "\n\tName is required. ";
                m_ErrorLabel.style.display = DisplayStyle.Flex;
            }

            try
            {
                m_AssetCollection.SetDescription(m_Description.value);
            }
            catch (Exception)
            {
                m_ErrorLabel.text += "\n\tDescription is required. ";
                m_ErrorLabel.style.display = DisplayStyle.Flex;
            }

            if (m_ErrorLabel.style.display == DisplayStyle.None) base.OnClicked();
        }
    }
}
#endif
