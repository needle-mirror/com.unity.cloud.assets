namespace Unity.Cloud.Documentation.Assets
{
    #region Example

    using System;
    using UnityEngine;

    public class ProjectSelectionExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;

        Vector2 m_ProjectListScrollPosition;

        public ProjectSelectionExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI()
        {
            if (!m_Behaviour.IsOrganizationSelected || m_Behaviour.IsProjectSelected) return;

            GUILayout.BeginVertical();

            // Go back to select a different scene.
            if (GUILayout.Button("Back"))
            {
                m_Behaviour.SetSelectedOrganization(null);
                return;
            }

            // Refresh the project list
            if (GUILayout.Button("Refresh"))
            {
                _ = m_Behaviour.GetProjectsAsync();
                return;
            }

            GUILayout.EndVertical();

            GUILayout.Space(15f);

            SelectAProject();
        }

        void SelectAProject()
        {
            GUILayout.BeginVertical();

            GUILayout.Label($"{m_Behaviour.CurrentOrganization.Name}");
            GUILayout.Space(15f);

            GUILayout.Label("Available Projects:");
            GUILayout.Space(5f);

            var projects = m_Behaviour.AvailableProjects;
            if (projects.Count > 0)
            {
                m_ProjectListScrollPosition = GUILayout.BeginScrollView(m_ProjectListScrollPosition, GUILayout.Height(Screen.height * 0.8f));

                for (var i = 0; i < projects.Count; ++i)
                {
                    if (GUILayout.Button(projects[i].Name))
                    {
                        m_Behaviour.SetSelectedProject(projects[i]);
                    }
                }

                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("No projects found.");
            }

            GUILayout.EndVertical();
        }
    }

    #endregion
}
