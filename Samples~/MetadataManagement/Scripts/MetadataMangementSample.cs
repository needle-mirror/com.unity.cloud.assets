using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.MetadataManagement
{
    public class MetadataMangementSample : MonoBehaviour
    {
        [SerializeField]
        UIDocument m_UiDocument;
        [SerializeField]
        FieldDefinitionController m_FieldDefinitionController;
        [SerializeField]
        VisualTreeAsset m_LayoutTemplate;

        MessagePopupController m_MessagePopup;
        CreateFieldDefinitionPopupController m_CreatePopup;
        FieldDefinitionPanelController m_FieldDefinitionPanel;
        ContextMenuController m_InfoPanelContextMenu;

        void Start()
        {
            var uiDocumentRoot = m_UiDocument.rootVisualElement;

            var sampleContainer = uiDocumentRoot.Q("ContentPanel");
            var layout = m_LayoutTemplate.Instantiate();
            layout.style.height = Length.Percent(100);
            layout.style.width = Length.Percent(100);
            sampleContainer.Add(layout);

            m_FieldDefinitionPanel = new FieldDefinitionPanelController(uiDocumentRoot.Q("FieldDefinitionInfoPanel"));
            m_FieldDefinitionPanel.UpdateFieldDefinition += OnUpdateFieldDefinition;

            m_InfoPanelContextMenu = new ContextMenuController(uiDocumentRoot.Q("FieldContextMenu"));
            m_InfoPanelContextMenu.RegisterButtonAction("Edit", () => SetEditEnabled(true));
            m_InfoPanelContextMenu.RegisterButtonAction("StopEdit", () => SetEditEnabled(false), "Stop Editing");
            m_InfoPanelContextMenu.SetButtonVisibility("StopEdit", false);
            m_InfoPanelContextMenu.RegisterButtonAction("Delete", OnDeleteFieldDefinition);

            m_CreatePopup = new CreateFieldDefinitionPopupController(layout, m_FieldDefinitionController.ValidateFieldDefinitionKey);
            m_CreatePopup.FieldDefinitionCreated += OnFieldDefinitionCreated;

            m_FieldDefinitionController.HideContent += HideContent;
            m_FieldDefinitionController.FieldDefinitionSelected += OnFieldDefinitionSelected;
            m_FieldDefinitionController.RegisterContextButton("Create", m_CreatePopup.Show);
            m_FieldDefinitionController.RegisterContextButton("Hide Deleted", m_FieldDefinitionController.HideDeletedFieldDefinitions, true);
            m_FieldDefinitionController.RegisterContextButton("Show Deleted", m_FieldDefinitionController.ShowDeletedFieldDefinitions, false);

            m_MessagePopup = new MessagePopupController(uiDocumentRoot);
        }

        void OnDestroy()
        {
            m_FieldDefinitionController.HideContent -= HideContent;
            m_FieldDefinitionController.FieldDefinitionSelected -= OnFieldDefinitionSelected;

            if (m_CreatePopup != null)
            {
                m_CreatePopup.FieldDefinitionCreated -= OnFieldDefinitionCreated;
                m_FieldDefinitionController.UnregisterContextButton("Create", m_CreatePopup.Show);
            }
        }

        void HideContent()
        {
            m_FieldDefinitionPanel.Hide();
        }

        async void OnFieldDefinitionCreated(IFieldDefinitionCreation fieldCreation)
        {
            try
            {
                await m_FieldDefinitionController.AssetRepository.CreateFieldDefinitionAsync(m_FieldDefinitionController.SelectedOrganizationId, fieldCreation, CancellationToken.None);
            }
            catch (Exception e)
            {
                e.LogException();
                m_MessagePopup.ShowMessage("Failed to create field definition", $"{e.Message}");
            }

            m_FieldDefinitionController.RefreshList();
        }

        void OnFieldDefinitionSelected()
        {
            var selectedFieldDefinition = m_FieldDefinitionController.SelectedFieldDefinition;
            m_InfoPanelContextMenu.SetEnabled(selectedFieldDefinition != null && selectedFieldDefinition.Status != "Deleted");
            m_FieldDefinitionPanel.SetFieldDefinition(selectedFieldDefinition);
        }

        void SetEditEnabled(bool isEditable)
        {
            m_FieldDefinitionPanel.SetEditEnabled(isEditable);
            m_InfoPanelContextMenu.SetButtonVisibility("Edit", !isEditable);
            m_InfoPanelContextMenu.SetButtonVisibility("StopEdit", isEditable);
        }

        async void OnUpdateFieldDefinition(Func<CancellationToken, Task> fieldDefinitionUpdate)
        {
            try
            {
                SetEditEnabled(false);
                await fieldDefinitionUpdate(CancellationToken.None);
                m_FieldDefinitionController.RefreshList();
                OnFieldDefinitionSelected();
            }
            catch (Exception e)
            {
                m_MessagePopup.ShowMessage("Failed to updated field definition", $"{e.Message}");
            }
        }

        async void OnDeleteFieldDefinition()
        {
            try
            {
                await m_FieldDefinitionController.AssetRepository.DeleteFieldDefinitionAsync(m_FieldDefinitionController.SelectedFieldDefinition.Descriptor, CancellationToken.None);
                m_FieldDefinitionController.RefreshList();
                OnFieldDefinitionSelected();
            }
            catch (Exception e)
            {
                m_MessagePopup.ShowMessage("Failed to delete field definition", $"{e.Message}");
            }
        }
    }
}
