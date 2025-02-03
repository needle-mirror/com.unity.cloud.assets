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

        CreateFieldDefinitionPopupController m_CreatePopup;
        FieldDefinitionPanelController m_FieldDefinitionPanel;

        void Start()
        {
            var uiDocumentRoot = m_UiDocument.rootVisualElement;

            var sampleContainer = uiDocumentRoot.Q("ContentPanel");
            sampleContainer.style.flexDirection = FlexDirection.Column;

            var layout = m_LayoutTemplate.Instantiate();
            sampleContainer.Add(layout);

            var contextMenu = new ContextMenuController(uiDocumentRoot.Q("FieldContextMenu"));
            m_FieldDefinitionPanel = new FieldDefinitionPanelController(uiDocumentRoot.Q("FieldDefinitionInfoPanel"), contextMenu);
            m_FieldDefinitionPanel.UpdateFieldDefinition += OnUpdateFieldDefinition;
            m_FieldDefinitionPanel.DeleteFieldDefinition += OnDeleteFieldDefinition;

            var popupVisual = layout.Q("CreateFieldPopup");
            sampleContainer.Add(popupVisual);

            m_CreatePopup = new CreateFieldDefinitionPopupController(popupVisual, m_FieldDefinitionController.ValidateFieldDefinitionKey);
            m_CreatePopup.FieldDefinitionCreated += OnFieldDefinitionCreated;

            m_FieldDefinitionController.HideContent += HideContent;
            m_FieldDefinitionController.FieldDefinitionSelected += OnFieldDefinitionSelected;
            m_FieldDefinitionController.RegisterContextButton("Create", m_CreatePopup.Show);
            m_FieldDefinitionController.RegisterContextButton("Hide Deleted", m_FieldDefinitionController.HideDeletedFieldDefinitions);
            m_FieldDefinitionController.RegisterContextButton("Show Deleted", m_FieldDefinitionController.ShowDeletedFieldDefinitions, false);
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
                await m_FieldDefinitionController.AssetRepository.CreateFieldDefinitionLiteAsync(m_FieldDefinitionController.SelectedOrganizationId, fieldCreation, CancellationToken.None);
            }
            catch (Exception e)
            {
                e.LogException();
                DialogService.ShowMessage(e, "Creation failed", $"Failed to create field definition of type {fieldCreation.Type} with reason: {e.Message}");
            }

            m_FieldDefinitionController.RefreshList();
        }

        void OnFieldDefinitionSelected(IFieldDefinition selectedFieldDefinition)
        {
            m_FieldDefinitionPanel.SetFieldDefinition(selectedFieldDefinition);
        }

        async void OnUpdateFieldDefinition(string fieldDefinitionKey, Func<IFieldDefinition, Task> fieldDefinitionUpdate)
        {
            if (string.IsNullOrEmpty(fieldDefinitionKey)) return;

            try
            {
                var descriptor = new FieldDefinitionDescriptor(m_FieldDefinitionController.SelectedOrganizationId, fieldDefinitionKey);
                var fieldDefinition = await m_FieldDefinitionController.AssetRepository.GetFieldDefinitionAsync(descriptor, CancellationToken.None);
                await fieldDefinitionUpdate(fieldDefinition);
                m_FieldDefinitionController.RefreshList();

            }
            catch (Exception e)
            {
                e.LogException();
                DialogService.ShowMessage(e, "Update failed", $"Failed to update field definition with reason: {e.Message}");
            }
        }

        async void OnDeleteFieldDefinition(string fieldDefinitionKey)
        {
            if (string.IsNullOrEmpty(fieldDefinitionKey)) return;

            try
            {
                var descriptor = new FieldDefinitionDescriptor(m_FieldDefinitionController.SelectedOrganizationId, fieldDefinitionKey);
                await m_FieldDefinitionController.AssetRepository.DeleteFieldDefinitionAsync(descriptor, CancellationToken.None);
                m_FieldDefinitionController.RefreshList();
            }
            catch (Exception e)
            {
                e.LogException();
                DialogService.ShowMessage(e, "Deletion failed", $"Failed to delete field definition with reason: {e.Message}");
            }
        }
    }
}
