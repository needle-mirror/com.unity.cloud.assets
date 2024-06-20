namespace Unity.Cloud.Documentation.Assets
{
    #region Example

    using Unity.Cloud.Assets;
    using UnityEngine;

    public class UseCaseCreateAssetExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;
        readonly string[] m_AssetTypeList;

        AssetType m_SelectedType = AssetType.Other;

        public UseCaseCreateAssetExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
            m_AssetTypeList = AssetTypeExtensions.AssetTypeList().ToArray();
        }

        public void OnGUI()
        {
            if (!m_Behaviour.IsProjectSelected) return;

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Type: ");

            var type = (int) m_SelectedType;
            type = GUILayout.SelectionGrid(type, m_AssetTypeList, 4);
            if (type != -1)
                m_SelectedType = (AssetType) type;

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Create new asset", GUILayout.Width(150f)))
            {
                _ = m_Behaviour.CreateAssetAsync(m_SelectedType);
            }

            GUILayout.EndVertical();
        }
    }

    #endregion
}
