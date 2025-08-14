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
    using UnityEngine;

    public class UseCaseListAssetLibraryFieldDefinitionsExampleUI : IAssetManagementUI
    {
        readonly AssetLibrariesBehaviour m_Behaviour;

        public UseCaseListAssetLibraryFieldDefinitionsExampleUI(AssetLibrariesBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1144 // Remove unused private method
#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseListAssetLibraryFieldDefinitionsExample : IAssetManagementUI
    {
        readonly UseCaseListAssetLibraryFieldDefinitionsExampleBehaviour m_Behaviour;

        public UseCaseListAssetLibraryFieldDefinitionsExample(AssetLibrariesBehaviour behaviour)
        {
            m_Behaviour = new UseCaseListAssetLibraryFieldDefinitionsExampleBehaviour(behaviour);
        }

        #region Example_UIContent

        IAsset m_CurrentAsset;
        List<string> m_SelectionAcceptedValues = new();
        Vector2 m_ListScrollPosition;

        public void OnGUI()
        {
            if (m_Behaviour.CurrentAsset == null) return;

            if (m_CurrentAsset != m_Behaviour.CurrentAsset)
            {
                m_CurrentAsset = m_Behaviour.CurrentAsset;
                _ = m_Behaviour.GetFieldDefinitionsAsync();
                return;
            }

            GUILayout.BeginVertical();

            if (GUILayout.Button("Refresh", GUILayout.Width(60)))
            {
                _ = m_Behaviour.GetFieldDefinitionsAsync();
            }

            GUILayout.Space(15f);

            GUILayout.Label("Fields:");
            ListFieldDefinitions();

            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            if (m_Behaviour.CurrentFieldDefinitionKey != null)
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

            m_ListScrollPosition = GUILayout.BeginScrollView(m_ListScrollPosition, GUILayout.MinWidth(Screen.width * 0.2f), GUILayout.Height(Screen.height * 0.8f));

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

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
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

    class UseCaseListAssetLibraryFieldDefinitionsExampleBehaviour
    {
        readonly AssetLibrariesBehaviour m_Behaviour;

        public IAssetLibrary CurrentAssetLibrary => m_Behaviour.CurrentAssetLibrary;
        public IAsset CurrentAsset => m_Behaviour.CurrentAsset;

        public UseCaseListAssetLibraryFieldDefinitionsExampleBehaviour(AssetLibrariesBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_Behaviour_RefreshMetadata

        public Dictionary<string, FieldDefinitionProperties> FieldDefinitionProperties { get; } = new();
        public string CurrentFieldDefinitionKey { get; private set; }
        public FieldDefinitionUpdate FieldDefinitionUpdate { get; private set; }

        public async Task GetFieldDefinitionsAsync()
        {
            var fieldKey = CurrentFieldDefinitionKey;
            CurrentFieldDefinitionKey = null;
            FieldDefinitionProperties.Clear();

            var metadataQuery = CurrentAsset.Metadata.Query().SelectAll().ExecuteAsync(CancellationToken.None);
            var metadataKeys = new List<string>();
            await foreach (var kvp in metadataQuery)
            {
                metadataKeys.Add(kvp.Key);
            }

            var asyncList = CurrentAssetLibrary.QueryFieldDefinitions(metadataKeys).ExecuteAsync(CancellationToken.None);
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
    }
}
