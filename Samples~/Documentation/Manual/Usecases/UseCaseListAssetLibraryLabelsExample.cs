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
    using Unity.Cloud.Common;
    using UnityEngine;

    public class UseCaseListAssetLibraryLabelsExampleUI : IAssetManagementUI
    {
        readonly AssetLibrariesBehaviour m_Behaviour;

        public UseCaseListAssetLibraryLabelsExampleUI(AssetLibrariesBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1144 // Remove unused private method
#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseListAssetLibraryLabelsExample : IAssetManagementUI
    {
        readonly UseCaseListAssetLibraryLabelsExampleBehaviour m_Behaviour;

        public UseCaseListAssetLibraryLabelsExample(AssetLibrariesBehaviour behaviour)
        {
            m_Behaviour = new UseCaseListAssetLibraryLabelsExampleBehaviour(behaviour);
        }

        #region Example_UIContent
        
        IAssetLibrary m_CurrentLibrary;
        Vector2 m_ListScrollPosition;

        public void OnGUI()
        {
            if (!m_Behaviour.IsAssetLibrarySelected) return;

            if (m_CurrentLibrary != m_Behaviour.CurrentAssetLibrary)
            {
                m_CurrentLibrary = m_Behaviour.CurrentAssetLibrary;
                _ = m_Behaviour.GetLabels();
                return;
            }

            GUILayout.BeginVertical();

            GUILayout.Label($"Library: {m_Behaviour.GetAssetLibraryName(m_Behaviour.CurrentAssetLibrary.Id)}");

            // Go back to select a different scene.
            if (GUILayout.Button("Back"))
            {
                m_Behaviour.SetSelectedAssetLibrary(null);
                return;
            }

            if (GUILayout.Button("Refresh", GUILayout.Width(60)))
            {
                _ = m_Behaviour.GetLabels();
                return;
            }

            ListLabels();

            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            if (!string.IsNullOrEmpty(m_Behaviour.CurrentLabelName))
            {
                DisplayLabel();
            }

            GUILayout.EndVertical();
        }

        void ListLabels()
        {
            if (m_Behaviour.Labels == null || m_Behaviour.ArchivedLabels == null) return;

            if (m_Behaviour.Labels.Count == 0 && m_Behaviour.ArchivedLabels.Count == 0)
            {
                GUILayout.Label(" ! No labels !");
            }
            else
            {
                m_ListScrollPosition = GUILayout.BeginScrollView(m_ListScrollPosition, GUILayout.MinWidth(Screen.width * 0.3f), GUILayout.Height(Screen.height * 0.8f));

                var labels = m_Behaviour.Labels.ToArray();
                foreach (var label in labels)
                {
                    DisplayLabel(label.Descriptor.LabelName, () => { });
                }

                var archivedLabels = m_Behaviour.ArchivedLabels.ToArray();
                foreach (var archivedLabel in archivedLabels)
                {
                    DisplayLabel(archivedLabel.Descriptor.LabelName, () => { });
                }

                GUILayout.EndScrollView();
            }
        }

        void DisplayLabel(string labelName, Action action)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(labelName);

            GUI.enabled = labelName != m_Behaviour.CurrentLabelName;

            if (GUILayout.Button("Select", GUILayout.Width(60)))
            {
                m_Behaviour.SetCurrentLabel(labelName);
            }

            GUI.enabled = true;

            if (m_Behaviour.LabelProperties.TryGetValue(labelName, out var properties))
            {
                if (!properties.IsSystemLabel)
                {
                    action?.Invoke();
                }
            }

            GUILayout.EndHorizontal();
        }

        void DisplayLabel()
        {
            if (!m_Behaviour.LabelProperties.TryGetValue(m_Behaviour.CurrentLabelName, out var properties))
            {
                GUILayout.Label(" ! Label properties not loaded !");
                return;
            }

            var isArchived = m_Behaviour.ArchivedLabels.Any(l => l.Descriptor.LabelName == m_Behaviour.CurrentLabelName);

            GUILayout.Label($"Label: {m_Behaviour.CurrentLabelName}" + (isArchived ? " (archived)" : string.Empty));
            GUILayout.Label($"Is system: {properties.IsSystemLabel}");
            GUILayout.Label($"Is assignable: {properties.IsAssignable}");
            GUILayout.Label($"Created on: {properties.AuthoringInfo?.Created:yyyy-M-d dddd}");
            GUILayout.Label($"Updated on: {properties.AuthoringInfo?.Updated:yyyy-M-d dddd}");
            GUILayout.Label($"Description: {properties.Description}");
            GUILayout.Label($"Color: {properties.DisplayColor?.Name ?? "None"}");
        }

        #endregion
    }

    class UseCaseListAssetLibraryLabelsExampleBehaviour
    {
        readonly AssetLibrariesBehaviour m_Behaviour;

        public bool IsAssetLibrarySelected => m_Behaviour.IsAssetLibrarySelected;
        public IAssetLibrary CurrentAssetLibrary => m_Behaviour.CurrentAssetLibrary;
        public string GetAssetLibraryName(AssetLibraryId libraryId) => m_Behaviour.GetAssetLibraryName(libraryId);

        public UseCaseListAssetLibraryLabelsExampleBehaviour(AssetLibrariesBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void SetSelectedAssetLibrary(IAssetLibrary library) => m_Behaviour.SetSelectedAssetLibrary(library);

        #region Example_Behaviour_RefreshLabels

        public List<ILabel> Labels { get; } = new();
        public List<ILabel> ArchivedLabels { get; } = new();
        public Dictionary<string, LabelProperties> LabelProperties { get; } = new();

        public string CurrentLabelName { get; private set; }

        public async Task GetLabels()
        {
            var currentLabelName = CurrentLabelName;
            CurrentLabelName = null;

            Labels.Clear();
            ArchivedLabels.Clear();

            var filter = new LabelSearchFilter();
            filter.IsArchived.WhereEquals(false);

            var asyncList = CurrentAssetLibrary.QueryLabels()
                .SelectWhereMatchesFilter(filter)
                .ExecuteAsync(CancellationToken.None);
            await foreach (var label in asyncList)
            {
                Labels.Add(label);

                if (label.Descriptor.LabelName == currentLabelName)
                {
                    SetCurrentLabel(currentLabelName);
                }

                LabelProperties[label.Descriptor.LabelName] = await label.GetPropertiesAsync(CancellationToken.None);
            }

            filter.IsArchived.WhereEquals(true);
            asyncList = CurrentAssetLibrary.QueryLabels()
                .SelectWhereMatchesFilter(filter)
                .ExecuteAsync(CancellationToken.None);
            await foreach (var label in asyncList)
            {
                ArchivedLabels.Add(label);

                if (label.Descriptor.LabelName == currentLabelName)
                {
                    SetCurrentLabel(currentLabelName);
                }

                LabelProperties[label.Descriptor.LabelName] = await label.GetPropertiesAsync(CancellationToken.None);
            }
        }

        public void SetCurrentLabel(string labelName)
        {
            CurrentLabelName = labelName;
        }

        #endregion
    }
}
