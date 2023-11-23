namespace Unity.Cloud.Assets.Documentation
{
    #region Example

    using System;
    using System.Linq;
    using Unity.Cloud.Assets;
    using UnityEngine;

    public class UseCaseManageAssetExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;
        readonly string[] m_AssetTypeList;

        IAsset m_CurrentAsset;
        AssetUpdate m_AssetUpdate;

        public UseCaseManageAssetExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
            m_AssetTypeList = AssetTypeExtensions.AssetTypeList().ToArray();
        }

        public void OnGUI()
        {
            if (!m_Behaviour.IsProjectSelected) return;

            GUILayout.Space(15f);

            GUILayout.BeginVertical();

            if (m_Behaviour.CurrentAsset == null)
            {
                GUILayout.Label(" ! No asset selected !");
            }
            else
            {
                if (m_Behaviour.CurrentAsset != m_CurrentAsset)
                {
                    m_CurrentAsset = m_Behaviour.CurrentAsset;
                    m_AssetUpdate = new AssetUpdate(m_CurrentAsset);
                }

                GUILayout.Label("Asset selected:");
                GUILayout.Space(5f);

                DisplayAsset(m_CurrentAsset, m_AssetUpdate);
            }

            GUILayout.EndVertical();
        }

        void DisplayAsset(IAsset asset, IAssetUpdate assetUpdate)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Name:");

            assetUpdate.Name = GUILayout.TextField(assetUpdate.Name, GUILayout.Width(100f));

            GUILayout.Space(5f);

            GUILayout.Label(asset.Status);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Type: ");

            var type = (int) assetUpdate.Type;
            type = GUILayout.SelectionGrid(type, m_AssetTypeList, 4);
            assetUpdate.Type = (AssetType) type;

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Tags: ");
            var tags = string.Join(',', assetUpdate.Tags);
            tags = GUILayout.TextField(tags);
            assetUpdate.Tags = tags.Split(',').ToList();

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Update"))
            {
                _ = m_Behaviour.UpdateAssetAsync(asset, assetUpdate);
            }
        }
    }

    #endregion
}
