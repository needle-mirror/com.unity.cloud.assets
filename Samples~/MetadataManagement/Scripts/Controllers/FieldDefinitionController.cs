using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.MetadataManagement
{
    public delegate (bool, string) ValidateFieldDefinitionName(string name);

    public class FieldDefinitionController : OrganizationController
    {
        [SerializeField]
        UIDocument m_FieldListUiDocument;

        readonly FieldDefinitionListUi m_FieldDefinitionListUi = new();
        readonly HashSet<string> m_DeletedFieldDefinitionKeys = new();

        ContextMenuController m_ContextMenu;

        public event Action<IFieldDefinition> FieldDefinitionSelected
        {
            add => m_FieldDefinitionListUi.FieldDefinitionSelected += value;
            remove => m_FieldDefinitionListUi.FieldDefinitionSelected -= value;
        }

        ContextMenuController ContextMenu => m_ContextMenu ??= new ContextMenuController(m_FieldListUiDocument.rootVisualElement.Q("LeftPanelContextMenu"));

        protected override void Start()
        {
            base.Start();

            m_FieldDefinitionListUi.Initialize(m_FieldListUiDocument.rootVisualElement, null);
            m_FieldDefinitionListUi.SetName("Field Definitions");
            m_FieldDefinitionListUi.Hide();

            ContextMenu.SetEnabled(false);

            OrganizationSelected += PopulateFieldList;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            OrganizationSelected -= PopulateFieldList;
        }

        public void RefreshList()
        {
            _ = PopulateFieldListAsync(SelectedOrganizationId);
        }

        public void RegisterContextButton(string buttonName, Action buttonAction, bool visible = true)
        {
            ContextMenu.RegisterButtonAction(buttonName, buttonAction);
            ContextMenu.SetButtonVisibility(buttonName, visible);
        }

        public void UnregisterContextButton(string buttonName, Action buttonAction)
        {
            ContextMenu.UnregisterButtonAction(buttonName, buttonAction);
        }

        public void HideDeletedFieldDefinitions()
        {
            m_ContextMenu.SetButtonVisibility("Hide Deleted", false);
            m_ContextMenu.SetButtonVisibility("Show Deleted", true);

            m_FieldDefinitionListUi.Filter = x => !m_DeletedFieldDefinitionKeys.Contains(x.Descriptor.FieldKey);
            m_FieldDefinitionListUi.Populate();
        }

        public void ShowDeletedFieldDefinitions()
        {
            m_ContextMenu.SetButtonVisibility("Hide Deleted", true);
            m_ContextMenu.SetButtonVisibility("Show Deleted", false);

            m_FieldDefinitionListUi.Filter = null;
            m_FieldDefinitionListUi.Populate();
        }

        public (bool, string) ValidateFieldDefinitionKey(string s)
        {
            return m_FieldDefinitionListUi.FieldDefinitions.Any(x => x.Descriptor.FieldKey == s)
                ? (false, "Field name already exists.")
                : (!string.IsNullOrWhiteSpace(s), string.Empty);
        }

        protected override void Hide()
        {
            base.Hide();

            m_FieldDefinitionListUi.Hide();
            m_ContextMenu.Hide();
            m_ContextMenu.SetEnabled(false);
        }

        void PopulateFieldList(OrganizationId organizationId)
        {
            m_FieldDefinitionListUi.ClearSelection();
            _ = PopulateFieldListAsync(organizationId);
        }

        async Task PopulateFieldListAsync(OrganizationId organizationId)
        {
            await m_FieldDefinitionListUi.Populate(AssetRepository, organizationId);
            ContextMenu.SetEnabled(true);

            m_DeletedFieldDefinitionKeys.Clear();
            foreach (var field in m_FieldDefinitionListUi.FieldDefinitions)
            {
                var properties = await field.GetPropertiesAsync(default);
                if (properties.IsDeleted)
                {
                    m_DeletedFieldDefinitionKeys.Add(field.Descriptor.FieldKey);
                }
            }
        }
    }
}
