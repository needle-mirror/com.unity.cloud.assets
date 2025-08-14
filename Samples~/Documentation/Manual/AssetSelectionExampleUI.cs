namespace Unity.Cloud.Documentation.Assets
{
    #region Example

    using System;
    using UnityEngine;

    public class AssetSelectionExampleUI : IAssetManagementUI
    {
        readonly BaseAssetBehaviour m_Behaviour;

        Vector2 m_ListScrollPosition;

        public AssetSelectionExampleUI(BaseAssetBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI()
        {
            if (!m_Behaviour.CanSelectAsset) return;

            GUILayout.BeginVertical();

            if (GUILayout.Button("Back"))
            {
                m_Behaviour.ClearParentSelection();
                return;
            }

            GUILayout.Space(15f);

            ListAssets();

            GUILayout.EndVertical();
        }

        void ListAssets()
        {
            GUILayout.Label($"Available Assets ({m_Behaviour.AssetCount}):");
            GUILayout.Space(5f);

            var assets = m_Behaviour.AvailableAssets.ToArray();
            if (assets.Length > 0)
            {
                m_ListScrollPosition = GUILayout.BeginScrollView(m_ListScrollPosition, GUILayout.ExpandHeight(true), GUILayout.Width(250));

                for (var i = 0; i < assets.Length; ++i)
                {
                    GUILayout.BeginHorizontal();
                    
                    var name = m_Behaviour.TryGetAssetProperties(assets[i].Descriptor.AssetVersion, out var properties) ? properties.Name : assets[i].Descriptor.AssetId.ToString();
                    GUILayout.Label(name, GUILayout.Width(150));
                    
                    GUI.enabled = assets[i].Descriptor.AssetId != m_Behaviour.CurrentAsset?.Descriptor.AssetId;
                    
                    if (GUILayout.Button("Select", GUILayout.Width(60)))
                    {
                        _ = m_Behaviour.CurrentAsset = assets[i];
                    }
                    
                    GUILayout.EndHorizontal();

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
