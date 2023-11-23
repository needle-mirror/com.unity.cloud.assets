namespace Unity.Cloud.Assets.Documentation
{
    #region Example

    using System;
    using UnityEngine;

    public class UseCaseCreateAssetExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public UseCaseCreateAssetExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI()
        {
            if (!m_Behaviour.IsProjectSelected) return;

            if (GUILayout.Button("Create new asset", GUILayout.Width(150f)))
            {
                _ = m_Behaviour.CreateAssetAsync();
            }
        }
    }

    #endregion
}
