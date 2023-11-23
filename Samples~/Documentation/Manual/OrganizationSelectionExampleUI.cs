namespace Unity.Cloud.Assets.Documentation
{
    #region Example

    using System;
    using UnityEngine;

    public class OrganizationSelectionExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public OrganizationSelectionExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI()
        {
            if (m_Behaviour.IsOrganizationSelected) return;

            // Refresh the org list
            if (GUILayout.Button("Refresh"))
            {
                _ = m_Behaviour.GetOrganizationsAsync();
                return;
            }

            GUILayout.Space(15f);

            // If an organization is not selected, list those available.
            SelectAnOrganization();
        }

        void SelectAnOrganization()
        {
            GUILayout.BeginVertical();

            GUILayout.Label("Available Organizations:");
            GUILayout.Space(5f);

            var availableOrganizations = m_Behaviour.AvailableOrganizations;
            if (availableOrganizations != null)
            {
                for (var i = 0; i < availableOrganizations.Length; ++i)
                {
                    if (GUILayout.Button(availableOrganizations[i].Name))
                    {
                        m_Behaviour.SetSelectedOrganization(availableOrganizations[i]);
                    }
                }
            }
            else
            {
                GUILayout.Label("Loading...");
            }

            GUILayout.EndVertical();
        }
    }

    #endregion
}
