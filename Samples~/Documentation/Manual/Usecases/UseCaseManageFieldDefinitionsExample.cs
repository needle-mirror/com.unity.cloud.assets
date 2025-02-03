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
                _ = m_Behaviour.GetFieldDefinitionsAsync();
            }

            GUILayout.BeginVertical();

            // Go back to select a different scene.
            if (GUILayout.Button("Back"))
            {
                m_Behaviour.SetSelectedOrganization(null);
                return;
            }

            if (GUILayout.Button("Refresh", GUILayout.Width(60)))
            {
                _ = m_Behaviour.GetFieldDefinitionsAsync();
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            if (GUILayout.Button("Create New"))
            {
                m_Behaviour.SetCurrentFieldDefinition(null);
                m_FieldDefinitionCreation = new FieldDefinitionCreation();
            }

            GUILayout.Label("Fields:");
            ListFieldDefinitions();

            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            if (m_Behaviour.CurrentFieldDefinitionKey == null)
            {
                CreateFieldDefinition();
            }
            else
            {
                DisplayFieldDefinition();
            }

            GUILayout.EndVertical();
        }

        void ListFieldDefinitions()
        {
            if (m_Behaviour.FieldDefinitionProperties.Count == 0)
            {
                GUILayout.Label(" ! No fields !");
                return;
            }

            m_FieldsScrollPosition = GUILayout.BeginScrollView(m_FieldsScrollPosition, GUILayout.MinWidth(Screen.width * 0.2f), GUILayout.Height(Screen.height * 0.8f));

            foreach (var kvp in m_Behaviour.FieldDefinitionProperties)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label(kvp.Key);

                GUI.enabled = kvp.Key != m_Behaviour.CurrentFieldDefinitionKey;

                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    m_Behaviour.SetCurrentFieldDefinition(kvp.Key);
                    if (kvp.Value.Type == FieldDefinitionType.Selection)
                    {
                        m_SelectionAcceptedValues = kvp.Value.AsSelectionFieldDefinitionProperties().AcceptedValues?.ToList() ?? new List<string>();
                    }
                }

                GUI.enabled = true;

                if (kvp.Value.IsDeleted)
                {
                    GUILayout.Space(64);
                }
                else
                {
                    if (GUILayout.Button("Delete", GUILayout.Width(60)))
                    {
                        _ = m_Behaviour.DeleteFieldDefinitionAsync(kvp.Key);
                    }
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
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
            var isUnique = !m_Behaviour.FieldDefinitionProperties.ContainsKey(m_FieldDefinitionCreation.Key);

            GUI.enabled = !isEmpty && isUnique;

            if (GUILayout.Button("Create"))
            {
                _ = m_Behaviour.CreateFieldDefinitionAsync(m_FieldDefinitionCreation);
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
            if (!m_Behaviour.FieldDefinitionProperties.TryGetValue(m_Behaviour.CurrentFieldDefinitionKey, out var properties))
            {
                GUILayout.Label(" ! Field definition properties not loaded !");
                return;
            }

            GUILayout.Label($"Field Definition: {m_Behaviour.CurrentFieldDefinitionKey}");
            GUILayout.Label(properties.IsDeleted ? "Deleted" : "Active");
            GUILayout.Label($"Created on: {properties.AuthoringInfo?.Created:yyyy-M-d dddd}");
            GUILayout.Label($"Updated on: {properties.AuthoringInfo?.Updated:yyyy-M-d dddd}");

            var multiSelectionStatus = string.Empty;
            var acceptedValues = string.Empty;

            if (properties.Type == FieldDefinitionType.Selection)
            {
                var selectionProperties = properties.AsSelectionFieldDefinitionProperties();

                multiSelectionStatus = selectionProperties.Multiselection ? ", Multi" : ", Single";
                acceptedValues = string.Join(',', selectionProperties.AcceptedValues ?? new List<string>());
            }

            GUILayout.Label($"Type: {properties.Type}{multiSelectionStatus}");

            if (properties.IsDeleted)
            {
                GUILayout.Label($"Display name: {properties.DisplayName}");
                if (!string.IsNullOrEmpty(acceptedValues))
                {
                    GUILayout.Label($"Accepted values: {acceptedValues}");
                }

                return;
            }

            GUILayout.Space(5f);

            DisplayUpdateValues(m_Behaviour.FieldDefinitionUpdate);

            if (properties.Type == FieldDefinitionType.Selection)
            {
                DisplaySelectionValues();
            }

            GUILayout.Space(5f);
            if (GUILayout.Button("Update"))
            {
                _ = m_Behaviour.UpdateFieldDefinitionAsync(m_SelectionAcceptedValues);
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

        public Dictionary<string, FieldDefinitionProperties> FieldDefinitionProperties { get; } = new();
        public string CurrentFieldDefinitionKey { get; private set; }
        public FieldDefinitionUpdate FieldDefinitionUpdate { get; private set; }

        public async Task GetFieldDefinitionsAsync()
        {
            var fieldKey = CurrentFieldDefinitionKey;
            CurrentFieldDefinitionKey = null;
            FieldDefinitionProperties.Clear();

            var asyncList = PlatformServices.AssetRepository.ListFieldDefinitionsAsync(CurrentOrganization.Id, Range.All, CancellationToken.None);
            await foreach (var fieldDefinition in asyncList)
            {
                var properties = await fieldDefinition.GetPropertiesAsync(CancellationToken.None);

                FieldDefinitionProperties.Add(fieldDefinition.Descriptor.FieldKey, properties);

                if (fieldDefinition.Descriptor.FieldKey == fieldKey)
                {
                    SetCurrentFieldDefinition(fieldKey);
                }
            }
        }

        public void SetCurrentFieldDefinition(string fieldDefinitionKey)
        {
            CurrentFieldDefinitionKey = fieldDefinitionKey;

            if (!string.IsNullOrEmpty(CurrentFieldDefinitionKey) && FieldDefinitionProperties.TryGetValue(CurrentFieldDefinitionKey, out var properties))
            {
                FieldDefinitionUpdate = new FieldDefinitionUpdate {DisplayName = properties.DisplayName};
            }
            else
            {
                FieldDefinitionUpdate = null;
            }
        }

        #endregion

        #region Example_Behaviour_CreateMetadata

        public async Task CreateFieldDefinitionAsync(IFieldDefinitionCreation fieldDefinitionCreation)
        {
            var fieldDefinitionDescriptor = await PlatformServices.AssetRepository.CreateFieldDefinitionLiteAsync(CurrentOrganization.Id, fieldDefinitionCreation, CancellationToken.None);
            CurrentFieldDefinitionKey = fieldDefinitionDescriptor.FieldKey;

            Debug.Log($"Field definition {fieldDefinitionCreation.Key} created.");

            await GetFieldDefinitionsAsync();
        }

        #endregion

        #region Example_Behaviour_DeleteMetadata

        public async Task DeleteFieldDefinitionAsync(string fieldDefinitionKey)
        {
            await PlatformServices.AssetRepository.DeleteFieldDefinitionAsync(new FieldDefinitionDescriptor(CurrentOrganization.Id, fieldDefinitionKey), CancellationToken.None);

            Debug.Log($"Field definition {fieldDefinitionKey} deleted.");

            await GetFieldDefinitionsAsync();
        }

        #endregion

        #region Example_Behaviour_UpdateMetadata

        public async Task UpdateFieldDefinitionAsync(IEnumerable<string> selectionAcceptedValues)
        {
            if (string.IsNullOrEmpty(CurrentFieldDefinitionKey)) return;

            var fieldDefinition = await PlatformServices.AssetRepository.GetFieldDefinitionAsync(new FieldDefinitionDescriptor(CurrentOrganization.Id, CurrentFieldDefinitionKey), CancellationToken.None);

            await fieldDefinition.UpdateAsync(FieldDefinitionUpdate, CancellationToken.None);

            try
            {
                await fieldDefinition.AsSelectionFieldDefinition().SetSelectionValuesAsync(selectionAcceptedValues, CancellationToken.None);
            }
            catch (Exception)
            {
                // Fail silently
            }

            await fieldDefinition.RefreshAsync(CancellationToken.None);
            var properties = await fieldDefinition.GetPropertiesAsync(CancellationToken.None);
            FieldDefinitionProperties[CurrentFieldDefinitionKey] = properties;

            Debug.Log($"Field definition {CurrentFieldDefinitionKey} updated.");
        }

        #endregion
    }
}
