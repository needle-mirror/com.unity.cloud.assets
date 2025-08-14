namespace Unity.Cloud.Documentation.Assets
{
#pragma warning disable S4487 // Unread "private" fields should be removed
#pragma warning disable S1186 // Methods should not be empty

    #region Example_UIClass

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Unity.Cloud.Assets;
    using Unity.Cloud.Identity;
    using UnityEngine;

    public class UseCaseFieldDefinitionsModifyAcceptedValuesExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public UseCaseFieldDefinitionsModifyAcceptedValuesExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseFieldDefinitionsModifyAcceptedValuesExample : IAssetManagementUI
    {
        readonly UseCaseFieldDefinitionsModifyAcceptedValuesExampleBehaviour m_Behaviour;

        public UseCaseFieldDefinitionsModifyAcceptedValuesExample(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = new UseCaseFieldDefinitionsModifyAcceptedValuesExampleBehaviour(behaviour);
        }

        #region Example_UIContent

        IOrganization m_CurrentOrganization;
        Vector2 m_FieldsScrollPosition;

        public void OnGUI()
        {
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

            GUILayout.Label("Fields:");
            ListFieldDefinitions();

            GUILayout.EndVertical();

            if (string.IsNullOrEmpty(m_Behaviour.CurrentFieldDefinitionKey))
            {
                GUILayout.Label(" ! No field selected !");
                return;
            }

            GUILayout.BeginVertical();

            DisplayFieldDefinition();

            GUILayout.EndVertical();
        }

        void ListFieldDefinitions()
        {
            if (m_Behaviour.FieldDefinitionsProperties.Count == 0)
            {
                GUILayout.Label(" ! No fields !");
                return;
            }

            m_FieldsScrollPosition = GUILayout.BeginScrollView(m_FieldsScrollPosition, GUILayout.MinWidth(Screen.width * 0.2f), GUILayout.Height(Screen.height * 0.8f));

            foreach (var field in m_Behaviour.FieldDefinitionsProperties)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label(field.Key);

                GUI.enabled = field.Key != m_Behaviour.CurrentFieldDefinitionKey;

                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    m_Behaviour.CurrentFieldDefinitionKey = field.Key;
                }

                GUI.enabled = true;

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        string m_NewValue = string.Empty;

        void DisplayFieldDefinition()
        {
            if (!m_Behaviour.FieldDefinitionsProperties.TryGetValue(m_Behaviour.CurrentFieldDefinitionKey, out var properties))
            {
                GUILayout.Label(" ! Field definition properties not loaded !");
                return;
            }

            GUI.enabled = !m_Behaviour.IsLocked;

            var acceptedValues = properties.AcceptedValues.ToArray();
            foreach (var value in acceptedValues)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label(value);

                if (GUILayout.Button("Remove"))
                {
                    _ = m_Behaviour.RemoveAcceptedValuesAsync(value);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.Space(15f);

            GUILayout.BeginHorizontal();

            m_NewValue = GUILayout.TextField(m_NewValue);

            GUI.enabled = !string.IsNullOrEmpty(m_NewValue) && !acceptedValues.Contains(m_NewValue);
            if (GUILayout.Button("Add"))
            {
                _ = m_Behaviour.AddAcceptedValueAsync(m_NewValue.Split(',').Select(x => x.Trim()).ToArray());
                m_NewValue = string.Empty;
            }

            GUILayout.EndHorizontal();

            GUI.enabled = true;
        }

        #endregion
    }

    class UseCaseFieldDefinitionsModifyAcceptedValuesExampleBehaviour
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public bool IsOrganizationSelected => m_Behaviour.IsOrganizationSelected;
        public IOrganization CurrentOrganization => m_Behaviour.CurrentOrganization;

        public UseCaseFieldDefinitionsModifyAcceptedValuesExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void SetSelectedOrganization(IOrganization organization) => m_Behaviour.SetSelectedOrganization(organization);

        #region Example_Behaviour_RefreshMetadata

        public Dictionary<string, SelectionFieldDefinitionProperties> FieldDefinitionsProperties { get; } = new();
        public string CurrentFieldDefinitionKey { get; set; }

        public bool IsLocked { get; private set; }

        public async Task GetFieldDefinitionsAsync()
        {
            var key = CurrentFieldDefinitionKey;
            CurrentFieldDefinitionKey = null;
            FieldDefinitionsProperties.Clear();

            var searchFilter = new FieldDefinitionSearchFilter();
            searchFilter.Deleted.WhereEquals(false);

            var asyncList = PlatformServices.AssetRepository.QueryFieldDefinitions(CurrentOrganization.Id)
                .SelectWhereMatchesFilter(searchFilter)
                .ExecuteAsync(CancellationToken.None);
            await foreach (var fieldDefinition in asyncList)
            {
                var properties = await fieldDefinition.GetPropertiesAsync(CancellationToken.None);

                if (properties.Type != FieldDefinitionType.Selection) continue;

                FieldDefinitionsProperties.Add(fieldDefinition.Descriptor.FieldKey,
                    await fieldDefinition.AsSelectionFieldDefinition().GetPropertiesAsync(CancellationToken.None));

                if (fieldDefinition.Descriptor.FieldKey == key)
                {
                    CurrentFieldDefinitionKey = key;
                }
            }
        }

        #endregion

        #region Example_Behaviour_ModifyAcceptedValues

        public async Task AddAcceptedValueAsync(params string[] values)
        {
            IsLocked = true;

            var fieldDefinition = await PlatformServices.AssetRepository.GetFieldDefinitionAsync(new FieldDefinitionDescriptor(CurrentOrganization.Id, CurrentFieldDefinitionKey), CancellationToken.None);
            var selectionFieldDefinition = fieldDefinition.AsSelectionFieldDefinition();
            await selectionFieldDefinition.AddSelectionValuesAsync(values, CancellationToken.None);

            Debug.Log("Added accepted values.");

            await selectionFieldDefinition.RefreshAsync(CancellationToken.None);
            var properties = await selectionFieldDefinition.GetPropertiesAsync(CancellationToken.None);
            FieldDefinitionsProperties[CurrentFieldDefinitionKey] = properties;

            IsLocked = false;
        }

        public async Task RemoveAcceptedValuesAsync(params string[] values)
        {
            IsLocked = true;

            var fieldDefinition = await PlatformServices.AssetRepository.GetFieldDefinitionAsync(new FieldDefinitionDescriptor(CurrentOrganization.Id, CurrentFieldDefinitionKey), CancellationToken.None);
            var selectionFieldDefinition = fieldDefinition.AsSelectionFieldDefinition();
            await selectionFieldDefinition.RemoveSelectionValuesAsync(values, CancellationToken.None);

            Debug.Log("Removed accepted values.");

            await selectionFieldDefinition.RefreshAsync(CancellationToken.None);
            var properties = await selectionFieldDefinition.GetPropertiesAsync(CancellationToken.None);
            FieldDefinitionsProperties[CurrentFieldDefinitionKey] = properties;

            IsLocked = false;
        }

        #endregion
    }
}
