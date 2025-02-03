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

            GUILayout.Label($"{m_Behaviour.CurrentOrganization.Name} >> {m_Behaviour.GetProjectName(m_Behaviour.CurrentProject.Descriptor.ProjectId)}");
            GUILayout.Space(15f);

            SelectAnAsset();

            GUILayout.EndVertical();
        }

        void SelectAnAsset()
        {
            GUILayout.Label($"Available Assets ({m_Behaviour.AvailableAssets.Count}):");
            GUILayout.Space(5f);

            var assets = m_Behaviour.AvailableAssets.ToArray();
            if (assets.Length > 0)
            {
                m_AssetListScrollPosition = GUILayout.BeginScrollView(m_AssetListScrollPosition, GUILayout.ExpandHeight(true), GUILayout.Width(250));

                for (var i = 0; i < assets.Length; ++i)
                {
                    var assetId = assets[i].Descriptor.AssetId;

                    GUI.enabled = assetId != m_Behaviour.CurrentAsset?.Descriptor.AssetId;

                    var name = m_Behaviour.AssetProperties.TryGetValue(assetId, out var properties) ? properties.Name : assetId.ToString();

                    if (GUILayout.Button(name))
                    {
                        _ = m_Behaviour.CurrentAsset = assets[i];
                    }

                    GUI.enabled = true;
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
