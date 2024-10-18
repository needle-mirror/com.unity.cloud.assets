namespace Unity.Cloud.Documentation.Assets
{
#pragma warning disable S4487 // Unread "private" fields should be removed
#pragma warning disable S1186 // Methods should not be empty
#pragma warning disable S1144 // Remove unused private method

    #region Example_UIClass

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Unity.Cloud.Assets;
    using Unity.Cloud.Identity;
    using UnityEngine;

    public class UseCaseManageFieldDefinitionsExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;
        readonly string[] m_FieldTypeList;
        GUIStyle m_ErrorLabelStyle;

        public UseCaseManageFieldDefinitionsExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
            m_FieldTypeList = Enum.GetNames(typeof(FieldDefinitionType));
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1144 // Remove unused private method
#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseManageFieldDefinitionsExample : IAssetManagementUI
    {
        readonly UseCaseManageFieldDefinitionsExampleBehaviour m_Behaviour;
        readonly string[] m_FieldTypeList;
        GUIStyle m_ErrorLabelStyle;

        public UseCaseManageFieldDefinitionsExample(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = new UseCaseManageFieldDefinitionsExampleBehaviour(behaviour);
            m_FieldTypeList = Enum.GetNames(typeof(FieldDefinitionType));
        }

        #region Example_UIContent

        IOrganization m_CurrentOrganization;
        FieldDefinitionCreation m_FieldDefinitionCreation = new();
        List<string> m_SelectionAcceptedValues = new();
        Vector2 m_FieldsScrollPosition;

        public void OnGUI()
        {
            if (m_ErrorLabelStyle == null)
            {
                m_ErrorLabelStyle = new GUIStyle(GUI.skin.label) {normal = {textColor = Color.red}};
            }

            if (!m_Behaviour.IsOrganizationSelected) return;

            if (m_CurrentOrganization != m_Behaviour.CurrentOrganization)
            {
                m_CurrentOrganization = m_Behaviour.CurrentOrganization;
                m_Behaviour.SetCurrentFieldDefinition(null);
                m_Behaviour.FieldDefinitions = null;
            }

            GUILayout.BeginVertical();

            // Go back to select a different scene.
            if (GUILayout.Button("Back"))
            {
                m_Behaviour.SetSelectedOrganization(null);
                return;
            }

            if (GUILayout.Button("Refresh", GUILayout.Width(60)) || m_Behaviour.FieldDefinitions == null)
            {
                _ = m_Behaviour.GetFieldDefinitions();
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            if (GUILayout.Button("Create New"))
            {
                m_Behaviour.SetCurrentFieldDefinition(null);
                m_FieldDefinitionCreation = new FieldDefinitionCreation();
            }

            GUILayout.Label("Fields:");
            ListFieldDefinitions(m_Behaviour.FieldDefinitions?.ToArray() ?? Array.Empty<IFieldDefinition>());

            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            if (m_Behaviour.CurrentFieldDefinition == null)
            {
                CreateFieldDefinition();
            }
            else
            {
                DisplayFieldDefinition();
            }

            GUILayout.EndVertical();
        }

        void ListFieldDefinitions(IReadOnlyList<IFieldDefinition> fields)
        {
            if (fields.Count == 0)
            {
                GUILayout.Label(" ! No fields !");
            }
            else
            {
                m_FieldsScrollPosition = GUILayout.BeginScrollView(m_FieldsScrollPosition, GUILayout.MinWidth(Screen.width * 0.2f), GUILayout.Height(Screen.height * 0.8f));

                for (var i = 0; i < fields.Count; ++i)
                {
                    GUILayout.BeginHorizontal();

                    GUILayout.Label(fields[i].Descriptor.FieldKey);

                    if (GUILayout.Button("Select", GUILayout.Width(60)))
                    {
                        m_Behaviour.SetCurrentFieldDefinition(fields[i]);
                        if (m_Behaviour.CurrentFieldDefinition.Type == FieldDefinitionType.Selection)
                        {
                            m_SelectionAcceptedValues = fields[i].AsSelectionFieldDefinition().AcceptedValues?.ToList() ?? new List<string>();
                        }
                    }

                    if (fields[i].IsDeleted)
                    {
                        GUILayout.Space(64);
                    }
                    else
                    {
                        if (GUILayout.Button("Delete", GUILayout.Width(60)))
                        {
                            _ = m_Behaviour.DeleteFieldDefinition(fields[i]);
                        }
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();
            }
        }

        void CreateFieldDefinition()
        {
            GUILayout.Label("New Field Definition (* = required):");

            GUILayout.Label("Field Key *:");
            m_FieldDefinitionCreation.Key = GUILayout.TextField(m_FieldDefinitionCreation.Key).Trim();

            GUILayout.Label("Display Name *:");
            m_FieldDefinitionCreation.DisplayName = GUILayout.TextField(m_FieldDefinitionCreation.DisplayName);

            GUILayout.Label("Type *:");
            var type = (int) m_FieldDefinitionCreation.Type;
            type = GUILayout.SelectionGrid(type, m_FieldTypeList, 3);
            m_FieldDefinitionCreation.Type = Enum.Parse<FieldDefinitionType>(m_FieldTypeList[type], true);

            var isEmpty = string.IsNullOrEmpty(m_FieldDefinitionCreation.Key) || string.IsNullOrEmpty(m_FieldDefinitionCreation.DisplayName);
            var isUnique = m_Behaviour.FieldDefinitions != null && m_Behaviour.FieldDefinitions.All(x => x.Descriptor.FieldKey != m_FieldDefinitionCreation.Key);

            GUI.enabled = !isEmpty && isUnique;

            if (GUILayout.Button("Create"))
            {
                _ = m_Behaviour.CreateFieldDefinitionAsync(m_FieldDefinitionCreation, CancellationToken.None);
                m_FieldDefinitionCreation = new FieldDefinitionCreation();
            }

            if (!isEmpty && !isUnique)
            {
                GUILayout.Label($"Field {m_FieldDefinitionCreation.Key} already exists.", m_ErrorLabelStyle);
            }

            GUI.enabled = true;
        }

        void DisplayFieldDefinition()
        {
            var field = m_Behaviour.CurrentFieldDefinition;

            GUILayout.Label($"Field Definition: {field.Descriptor.FieldKey}");
            GUILayout.Label(field.IsDeleted ? "Deleted" : "Active");
            GUILayout.Label($"Created on: {field.AuthoringInfo?.Created:yyyy-M-d dddd}");
            GUILayout.Label($"Updated on: {field.AuthoringInfo?.Updated:yyyy-M-d dddd}");

            var multiSelectionStatus = string.Empty;
            var acceptedValues = string.Empty;

            if (field.Type == FieldDefinitionType.Selection)
            {
                var selectionFieldDefinition = field.AsSelectionFieldDefinition();

                multiSelectionStatus = selectionFieldDefinition.Multiselection ? ", Multi" : ", Single";
                acceptedValues = string.Join(',', selectionFieldDefinition.AcceptedValues ?? new List<string>());
            }

            GUILayout.Label($"Type: {field.Type}{multiSelectionStatus}");

            if (field.IsDeleted)
            {
                GUILayout.Label($"Display name: {field.DisplayName}");
                if (!string.IsNullOrEmpty(acceptedValues))
                {
                    GUILayout.Label($"Accepted values: {acceptedValues}");
                }

                return;
            }

            GUILayout.Space(5f);

            DisplayUpdateValues(m_Behaviour.FieldDefinitionUpdate);

            if (m_Behaviour.CurrentFieldDefinition.Type == FieldDefinitionType.Selection)
            {
                DisplaySelectionValues();
            }

            GUILayout.Space(5f);
            if (GUILayout.Button("Update"))
            {
                _ = m_Behaviour.UpdateFieldDefinition(m_SelectionAcceptedValues);
            }
        }

        static void DisplayUpdateValues(FieldDefinitionUpdate update)
        {
            GUILayout.Label("Display name:");
            update.DisplayName = GUILayout.TextField(update.DisplayName);
        }

        void DisplaySelectionValues()
        {
            GUILayout.Space(5f);
            GUILayout.Label("Accepted Values:");

            var value = string.Join(',', m_SelectionAcceptedValues);
            var newValue = GUILayout.TextField(value);
            if (value != newValue)
            {
                m_SelectionAcceptedValues = newValue.Split(',').Select(x => x.Trim()).ToList();
            }
        }

        #endregion
    }

    class UseCaseManageFieldDefinitionsExampleBehaviour
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public bool IsOrganizationSelected => m_Behaviour.IsOrganizationSelected;
        public IOrganization CurrentOrganization => m_Behaviour.CurrentOrganization;

        public UseCaseManageFieldDefinitionsExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void SetSelectedOrganization(IOrganization organization) => m_Behaviour.SetSelectedOrganization(organization);

        #region Example_Behaviour_RefreshMetadata

        public List<IFieldDefinition> FieldDefinitions { get; set; }
        public IFieldDefinition CurrentFieldDefinition { get; private set; }
        public FieldDefinitionUpdate FieldDefinitionUpdate { get; private set; }

        public async Task GetFieldDefinitions()
        {
            FieldDefinitions = new List<IFieldDefinition>();
            CurrentFieldDefinition = null;

            var asyncList = PlatformServices.AssetRepository.QueryFieldDefinitions(CurrentOrganization.Id)
                .ExecuteAsync(CancellationToken.None);
            await foreach (var fieldDefinition in asyncList)
            {
                FieldDefinitions.Add(fieldDefinition);
            }
        }

        public void SetCurrentFieldDefinition(IFieldDefinition fieldDefinition)
        {
            CurrentFieldDefinition = fieldDefinition;
            FieldDefinitionUpdate = CurrentFieldDefinition != null ? new FieldDefinitionUpdate(CurrentFieldDefinition) : null;
        }

        #endregion

        #region Example_Behaviour_CreateMetadata

        public async Task CreateFieldDefinitionAsync(IFieldDefinitionCreation fieldDefinitionCreation, CancellationToken cancellationToken)
        {
            await PlatformServices.AssetRepository.CreateFieldDefinitionAsync(CurrentOrganization.Id, fieldDefinitionCreation, cancellationToken);
            FieldDefinitions = null;
            Debug.Log($"Field definition {fieldDefinitionCreation.Key} created.");
        }

        #endregion

        #region Example_Behaviour_DeleteMetadata

        public async Task DeleteFieldDefinition(IFieldDefinition fieldDefinition)
        {
            await PlatformServices.AssetRepository.DeleteFieldDefinitionAsync(fieldDefinition.Descriptor, CancellationToken.None);
            FieldDefinitions = null;
            if (fieldDefinition == CurrentFieldDefinition)
            {
                SetCurrentFieldDefinition(null);
            }

            Debug.Log($"Field definition {fieldDefinition.Descriptor.FieldKey} deleted.");
        }

        #endregion

        #region Example_Behaviour_UpdateMetadata

        public async Task UpdateFieldDefinition(IEnumerable<string> selectionAcceptedValues)
        {
            await CurrentFieldDefinition.UpdateAsync(FieldDefinitionUpdate, CancellationToken.None);

            if (CurrentFieldDefinition.Type == FieldDefinitionType.Selection)
            {
                await CurrentFieldDefinition.AsSelectionFieldDefinition().SetSelectionValuesAsync(selectionAcceptedValues, CancellationToken.None);
            }

            Debug.Log($"Field definition {CurrentFieldDefinition.Descriptor.FieldKey} updated.");
        }

        #endregion
    }
}
