using System.Text;

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
        GUIStyle m_ErrorLabelStyle;

        public UseCaseManageLabelsExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
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
        GUIStyle m_ErrorLabelStyle;

        public UseCaseManageLabelsExample(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = new UseCaseManageLabelsExampleBehaviour(behaviour);
        }

        #region Example_UIContent

        IOrganization m_CurrentOrganization;
        LabelCreation m_LabelCreation = new();
        Vector2 m_LabelsScrollPosition;
        List<string> m_ColorNames;
        string[] m_ColorSelection;

        public void OnGUI()
        {
            if (m_ErrorLabelStyle == null)
            {
                m_ErrorLabelStyle = new GUIStyle(GUI.skin.label) {normal = {textColor = UnityColor.red}};
            }

            if (m_ColorNames == null)
            {
                m_ColorNames = Enum.GetNames(typeof(KnownColor)).ToList();
                m_ColorSelection = m_ColorNames
                    .Select(n => $"<color=#{Color.FromName(n).ToArgb().ToString("X")[2..]}>\x25A0</color>")
                    .ToArray();
            }

            if (!m_Behaviour.IsOrganizationSelected) return;

            if (m_CurrentOrganization != m_Behaviour.CurrentOrganization)
            {
                m_CurrentOrganization = m_Behaviour.CurrentOrganization;
                m_Behaviour.SetCurrentLabel(null);
                m_Behaviour.Labels = null;
            }

            GUILayout.BeginVertical();

            // Go back to select a different scene.
            if (GUILayout.Button("Back"))
            {
                m_Behaviour.SetSelectedOrganization(null);
                return;
            }

            if (GUILayout.Button("Refresh", GUILayout.Width(60)) || m_Behaviour.Labels == null)
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

            if (m_Behaviour.CurrentLabel == null)
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

                for (var i = 0; i < m_Behaviour.Labels.Count; ++i)
                {
                    DisplayLabel(m_Behaviour.Labels[i], label =>
                    {
                        if (label.IsSystemLabel) return;

                        if (GUILayout.Button("Archive", GUILayout.Width(80)))
                        {
                            _ = label.ArchiveAsync(CancellationToken.None);
                        }
                    });
                }

                for (var i = 0; i < m_Behaviour.ArchivedLabels.Count; ++i)
                {
                    DisplayLabel(m_Behaviour.ArchivedLabels[i], label =>
                    {
                        if (label.IsSystemLabel) return;

                        if (GUILayout.Button("Unarchive", GUILayout.Width(80)))
                        {
                            _ = label.UnarchiveAsync(CancellationToken.None);
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
            var isUnique = m_Behaviour.Labels != null && m_Behaviour.Labels.All(x => x.Descriptor.LabelName != m_LabelCreation.Name);
            isUnique &= m_Behaviour.ArchivedLabels != null && m_Behaviour.ArchivedLabels.All(x => x.Descriptor.LabelName != m_LabelCreation.Name);
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

        void DisplayLabel(ILabel label, Action<ILabel> action)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(label.Descriptor.LabelName);

            if (GUILayout.Button("Select", GUILayout.Width(60)))
            {
                m_Behaviour.SetCurrentLabel(label);
            }

            action?.Invoke(label);

            GUILayout.EndHorizontal();
        }

        void DisplayLabel()
        {
            var label = m_Behaviour.CurrentLabel;

            var isArchived = m_Behaviour.ArchivedLabels.Contains(label);

            GUILayout.Label($"Label: {label.Descriptor.LabelName}" + (isArchived ? " (archived)" : string.Empty));
            GUILayout.Label($"Is system: {label.IsSystemLabel}");
            GUILayout.Label($"Is assignable: {label.IsAssignable}");
            GUILayout.Label($"Created on: {label.AuthoringInfo?.Created:yyyy-M-d dddd}");
            GUILayout.Label($"Updated on: {label.AuthoringInfo?.Updated:yyyy-M-d dddd}");

            if (label.IsSystemLabel)
            {
                GUILayout.Label($"Description: {label.Description}");
                GUILayout.Label($"Color: {label.DisplayColor.Name}");
            }
            else
            {
                GUILayout.Space(5f);

                m_Behaviour.LabelUpdate.Description = GUILayout.TextArea(m_Behaviour.LabelUpdate.Description);

                GUILayout.Label($"Color: {m_Behaviour.LabelUpdate.DisplayColor?.Name ?? label.DisplayColor.Name}");
                var index = m_Behaviour.LabelUpdate.DisplayColor == null
                    ? m_ColorNames.IndexOf(label.DisplayColor.Name)
                    : m_ColorNames.IndexOf(m_Behaviour.LabelUpdate.DisplayColor.Value.Name);
                index = GUILayout.SelectionGrid(index, m_ColorSelection, 16);
                if (index >= 0)
                {
                    m_Behaviour.LabelUpdate.DisplayColor = Color.FromName(m_ColorNames[index]);
                }

                GUILayout.Space(5f);
                if (GUILayout.Button("Update"))
                {
                    _ = m_Behaviour.UpdateLabel();
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

        public List<ILabel> Labels { get; set; }
        public List<ILabel> ArchivedLabels { get; set; }
        public ILabel CurrentLabel { get; private set; }
        public LabelUpdate LabelUpdate { get; private set; }

        public async Task GetLabels()
        {
            Labels = new List<ILabel>();
            ArchivedLabels = new List<ILabel>();
            CurrentLabel = null;

            var filter = new LabelSearchFilter();
            filter.IsArchived.WhereEquals(false);

            var asyncList = PlatformServices.AssetRepository.QueryLabels(CurrentOrganization.Id)
                .SelectWhereMatchesFilter(filter)
                .ExecuteAsync(CancellationToken.None);
            await foreach (var label in asyncList)
            {
                Labels.Add(label);
            }

            filter.IsArchived.WhereEquals(true);
            asyncList = PlatformServices.AssetRepository.QueryLabels(CurrentOrganization.Id)
                .SelectWhereMatchesFilter(filter)
                .ExecuteAsync(CancellationToken.None);
            await foreach (var label in asyncList)
            {
                ArchivedLabels.Add(label);
            }
        }

        public void SetCurrentLabel(ILabel label)
        {
            CurrentLabel = label;
            LabelUpdate = CurrentLabel != null ? new LabelUpdate() : null;
        }

        #endregion

        #region Example_Behaviour_CreateLabel

        public async Task CreateLabelAsync(ILabelCreation labelCreation, CancellationToken cancellationToken)
        {
            try
            {
                await PlatformServices.AssetRepository.CreateLabelAsync(CurrentOrganization.Id, labelCreation, cancellationToken);

                Labels = null;
                ArchivedLabels = null;

                Debug.Log($"Label {labelCreation.Name} created.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create label {labelCreation.Name}: {e.Message}");
            }
        }

        #endregion

        #region Example_Behaviour_ArchiveLabel

        public async Task ArchiveLabel(ILabel label)
        {
            try
            {
                await label.ArchiveAsync(CancellationToken.None);
                await label.RefreshAsync(CancellationToken.None);

                Labels = null;
                ArchivedLabels = null;

                Debug.Log($"Label {label.Name} archived.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to archive label {label.Name}: {e.Message}");
            }
        }

        public async Task UnarchiveLabel(ILabel label)
        {
            try
            {
                await label.UnarchiveAsync(CancellationToken.None);
                await label.RefreshAsync(CancellationToken.None);

                Labels = null;
                ArchivedLabels = null;

                Debug.Log($"Label {label.Name} unarchived.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to unarchive label {label.Name}: {e.Message}");
            }
        }

        #endregion

        #region Example_Behaviour_UpdateLabel

        public async Task UpdateLabel()
        {
            try
            {
                await CurrentLabel.UpdateAsync(LabelUpdate, CancellationToken.None);
                await CurrentLabel.RefreshAsync(CancellationToken.None);

                Debug.Log($"Label {CurrentLabel.Name} updated.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to update label {CurrentLabel.Name}: {e.Message}");
            }
        }

        #endregion
    }
}
