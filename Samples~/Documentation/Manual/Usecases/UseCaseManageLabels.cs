namespace Unity.Cloud.Documentation.Assets
{
#pragma warning disable S4487 // Unread "private" fields should be removed
#pragma warning disable S1186 // Methods should not be empty
#pragma warning disable S1144 // Remove unused private method

    #region Example_UIClass

    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Unity.Cloud.Assets;
    using Unity.Cloud.Identity;
    using UnityEngine;
    using UnityColor = UnityEngine.Color;
    using Color = System.Drawing.Color;

    public class UseCaseManageLabelsExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;
        List<string> m_ColorNames;
        string[] m_ColorSelection;

        public UseCaseManageLabelsExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;

            m_ColorNames = Enum.GetNames(typeof(KnownColor)).ToList();
            m_ColorSelection = m_ColorNames
                .Select(n => $"<color=#{Color.FromName(n).ToArgb().ToString("X")[2..]}>\x25A0</color>")
                .ToArray();
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1144 // Remove unused private method
#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseManageLabelsExample : IAssetManagementUI
    {
        readonly UseCaseManageLabelsExampleBehaviour m_Behaviour;
        List<string> m_ColorNames;
        string[] m_ColorSelection;

        public UseCaseManageLabelsExample(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = new UseCaseManageLabelsExampleBehaviour(behaviour);

            m_ColorNames = Enum.GetNames(typeof(KnownColor)).ToList();
            m_ColorSelection = m_ColorNames
                .Select(n => $"<color=#{Color.FromName(n).ToArgb().ToString("X")[2..]}>\x25A0</color>")
                .ToArray();
        }

        #region Example_UIContent

        GUIStyle m_ErrorLabelStyle;
        
        IOrganization m_CurrentOrganization;
        LabelCreation m_LabelCreation = new();
        Vector2 m_LabelsScrollPosition;

        public void OnGUI()
        {
            if (m_ErrorLabelStyle == null)
            {
                m_ErrorLabelStyle = new GUIStyle(GUI.skin.label) {normal = {textColor = UnityColor.red}};
            }

            if (!m_Behaviour.IsOrganizationSelected) return;

            if (m_CurrentOrganization != m_Behaviour.CurrentOrganization)
            {
                m_CurrentOrganization = m_Behaviour.CurrentOrganization;
                _ = m_Behaviour.GetLabels();
                return;
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
                _ = m_Behaviour.GetLabels();
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            if (GUILayout.Button("Create New"))
            {
                m_Behaviour.SetCurrentLabel(null);
                m_LabelCreation = new LabelCreation();
            }

            ListLabels();

            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            if (string.IsNullOrEmpty(m_Behaviour.CurrentLabelName))
            {
                CreateLabel();
            }
            else
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
                m_LabelsScrollPosition = GUILayout.BeginScrollView(m_LabelsScrollPosition, GUILayout.MinWidth(Screen.width * 0.3f), GUILayout.Height(Screen.height * 0.8f));

                var labels = m_Behaviour.Labels.ToArray();
                foreach (var label in labels)
                {
                    DisplayLabel(label.Descriptor.LabelName, () =>
                    {
                        if (GUILayout.Button("Archive", GUILayout.Width(80)))
                        {
                            _ = m_Behaviour.ArchiveLabelAsync(label.Descriptor.LabelName);
                        }
                    });
                }

                var archivedLabels = m_Behaviour.ArchivedLabels.ToArray();
                foreach (var archivedLabel in archivedLabels)
                {
                    DisplayLabel(archivedLabel.Descriptor.LabelName, () =>
                    {
                        if (GUILayout.Button("Unarchive", GUILayout.Width(80)))
                        {
                            _ = m_Behaviour.UnarchiveLabelAsync(archivedLabel.Descriptor.LabelName);
                        }
                    });
                }

                GUILayout.EndScrollView();
            }
        }

        void CreateLabel()
        {
            GUILayout.Label("New Label (* = required):");

            GUILayout.Label("Label name *:");
            m_LabelCreation.Name = GUILayout.TextField(m_LabelCreation.Name ?? string.Empty);

            GUILayout.Label("Description:");
            m_LabelCreation.Description = GUILayout.TextArea(m_LabelCreation.Description ?? string.Empty);

            GUILayout.Label($"Color: {m_LabelCreation.DisplayColor.Name}");
            var index = GUILayout.SelectionGrid(m_ColorNames.IndexOf(m_LabelCreation.DisplayColor.Name), m_ColorSelection, 16);
            if (index >= 0)
            {
                m_LabelCreation.DisplayColor = Color.FromName(m_ColorNames[index]);
            }

            var isEmpty = string.IsNullOrEmpty(m_LabelCreation.Name);
            var isUnique = m_Behaviour.Labels.All(x => x.Descriptor.LabelName != m_LabelCreation.Name);
            isUnique &= m_Behaviour.ArchivedLabels.All(x => x.Descriptor.LabelName != m_LabelCreation.Name);
            var canCreate = !isEmpty && isUnique;

            GUI.enabled = canCreate;
            if (GUILayout.Button("Create"))
            {
                _ = m_Behaviour.CreateLabelAsync(m_LabelCreation, CancellationToken.None);
                m_LabelCreation = new LabelCreation();
            }

            if (!isEmpty && !isUnique)
            {
                GUILayout.Label($"Label {m_LabelCreation.Name} already exists.", m_ErrorLabelStyle);
            }

            GUI.enabled = true;
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

            if (properties.IsSystemLabel)
            {
                GUILayout.Label($"Description: {properties.Description}");
                GUILayout.Label($"Color: {properties.DisplayColor?.Name ?? "None"}");
            }
            else
            {
                GUILayout.Space(5f);

                m_Behaviour.CurrentLabelUpdate.Description = GUILayout.TextArea(m_Behaviour.CurrentLabelUpdate.Description);

                var color = m_Behaviour.CurrentLabelUpdate.DisplayColor?.Name ?? properties.DisplayColor?.Name;
                GUILayout.Label($"Color: {color ?? "None"}");
                var index = m_Behaviour.CurrentLabelUpdate.DisplayColor == null
                    ? m_ColorNames.IndexOf(properties.DisplayColor?.Name)
                    : m_ColorNames.IndexOf(m_Behaviour.CurrentLabelUpdate.DisplayColor.Value.Name);
                index = GUILayout.SelectionGrid(index, m_ColorSelection, 16);
                if (index >= 0)
                {
                    m_Behaviour.CurrentLabelUpdate.DisplayColor = Color.FromName(m_ColorNames[index]);
                }

                GUILayout.Space(5f);
                if (GUILayout.Button("Update"))
                {
                    _ = m_Behaviour.UpdateCurrentLabelAsync();
                }
            }
        }

        #endregion
    }

    class UseCaseManageLabelsExampleBehaviour
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public bool IsOrganizationSelected => m_Behaviour.IsOrganizationSelected;
        public IOrganization CurrentOrganization => m_Behaviour.CurrentOrganization;

        public UseCaseManageLabelsExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void SetSelectedOrganization(IOrganization organization) => m_Behaviour.SetSelectedOrganization(organization);

        #region Example_Behaviour_RefreshLabels

        public List<ILabel> Labels { get; } = new();
        public List<ILabel> ArchivedLabels { get; } = new();
        public Dictionary<string, LabelProperties> LabelProperties { get; } = new();

        public string CurrentLabelName { get; private set; }
        public LabelUpdate CurrentLabelUpdate { get; private set; }

        public async Task GetLabels()
        {
            var currentLabelName = CurrentLabelName;
            CurrentLabelName = null;

            Labels.Clear();
            ArchivedLabels.Clear();

            var filter = new LabelSearchFilter();
            filter.IsArchived.WhereEquals(false);

            var asyncList = PlatformServices.AssetRepository.QueryLabels(CurrentOrganization.Id)
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
            asyncList = PlatformServices.AssetRepository.QueryLabels(CurrentOrganization.Id)
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
            CurrentLabelUpdate = null;

            if (LabelProperties.TryGetValue(labelName, out var properties))
            {
                CurrentLabelUpdate = new LabelUpdate
                {
                    Description = properties.Description,
                    DisplayColor = properties.DisplayColor
                };
            }
        }

        #endregion

        #region Example_Behaviour_CreateLabel

        public async Task CreateLabelAsync(ILabelCreation labelCreation, CancellationToken cancellationToken)
        {
            try
            {
                await PlatformServices.AssetRepository.CreateLabelLiteAsync(CurrentOrganization.Id, labelCreation, cancellationToken);

                await GetLabels();

                Debug.Log($"Label {labelCreation.Name} created.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create label {labelCreation.Name}: {e.Message}");
            }
        }

        #endregion

        #region Example_Behaviour_ArchiveLabel

        public async Task ArchiveLabelAsync(string labelName)
        {
            try
            {
                var labelDescriptor = new LabelDescriptor(CurrentOrganization.Id, labelName);
                var label = await PlatformServices.AssetRepository.GetLabelAsync(labelDescriptor, CancellationToken.None);

                await label.ArchiveAsync(CancellationToken.None);

                Debug.Log($"Label {label.Descriptor.LabelName} archived.");

                await GetLabels();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to archive label {labelName}: {e.Message}");
            }
        }

        public async Task UnarchiveLabelAsync(string labelName)
        {
            try
            {
                var labelDescriptor = new LabelDescriptor(CurrentOrganization.Id, labelName);
                var label = await PlatformServices.AssetRepository.GetLabelAsync(labelDescriptor, CancellationToken.None);

                await label.UnarchiveAsync(CancellationToken.None);

                Debug.Log($"Label {label.Descriptor.LabelName} unarchived.");

                await GetLabels();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to unarchive label {labelName}: {e.Message}");
            }
        }

        #endregion

        #region Example_Behaviour_UpdateLabel

        public async Task UpdateCurrentLabelAsync()
        {
            if (string.IsNullOrEmpty(CurrentLabelName) || CurrentLabelUpdate == null) return;

            try
            {
                var label = Labels.FirstOrDefault(l => l.Descriptor.LabelName == CurrentLabelName)
                            ?? ArchivedLabels.FirstOrDefault(l => l.Descriptor.LabelName == CurrentLabelName);
                if (label == null) return;

                await label.UpdateAsync(CurrentLabelUpdate, CancellationToken.None);
                await label.RefreshAsync(CancellationToken.None);

                LabelProperties[CurrentLabelName] = await label.GetPropertiesAsync(CancellationToken.None);

                Debug.Log($"Label {CurrentLabelName} updated.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to update label {CurrentLabelName}: {e.Message}");
            }
        }

        #endregion
    }
}
