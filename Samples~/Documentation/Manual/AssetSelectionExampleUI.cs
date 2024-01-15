namespace Unity.Cloud.Documentation.Assets
{
    #region Example

    using System;
    using UnityEngine;

    public class AssetSelectionExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;

        Vector2 m_AssetListScrollPosition;

        public AssetSelectionExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI()
        {
            if (!m_Behaviour.IsProjectSelected) return;

            // Go back to select a different scene.
            if (GUILayout.Button("Back"))
            {
                m_Behaviour.SetSelectedProject(null);
                return;
            }

            GUILayout.Space(15f);

            GUILayout.BeginVertical();

            GUILayout.Label($"{m_Behaviour.CurrentOrganization.Name} >> {m_Behaviour.CurrentProject.Name}");
            GUILayout.Space(15f);

            SelectAnAsset();

            GUILayout.EndVertical();
        }

        void SelectAnAsset()
        {
            var width = Screen.width * 0.25f;

            GUILayout.Label("Available Assets:");
            GUILayout.Space(5f);

            var assets = m_Behaviour.AvailableAssets;
            if (assets.Count > 0)
            {
                m_AssetListScrollPosition = GUILayout.BeginScrollView(m_AssetListScrollPosition, GUILayout.Height(Screen.height * 0.8f));

                for (var i = 0; i < assets.Count; ++i)
                {
                    if (GUILayout.Button(assets[i].Name, GUILayout.Width(width)))
                    {
                        m_Behaviour.CurrentAsset = assets[i];
                        Debug.Log($"Selected: {assets[i].Descriptor.AssetId}");
                    }
                }

                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("No assets found.");
            }
        }
    }

    #endregion
}
