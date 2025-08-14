namespace Unity.Cloud.Documentation.Assets
{
#pragma warning disable S4487 // Unread "private" fields should be removed
#pragma warning disable S1186 // Methods should not be empty

    #region Example_UIClass

    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Unity.Cloud.Assets;
    using Unity.Cloud.Common;
    using UnityEngine;

    public class UseCaseManageAssetStatusExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public UseCaseManageAssetStatusExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseManageAssetStatusExample : IAssetManagementUI
    {
        readonly UseCaseManageAssetStatusExampleBehaviour m_Behaviour;

        public UseCaseManageAssetStatusExample(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = new UseCaseManageAssetStatusExampleBehaviour(behaviour);
        }

        #region Example_UIContent

        IAsset m_CurrentAsset;

        public void OnGUI()
        {
            if (!m_Behaviour.IsProjectSelected) return;

            if (m_Behaviour.CurrentAsset == null)
            {
                GUILayout.Label(" ! No asset selected !");
                return;
            }

            if (m_CurrentAsset != m_Behaviour.CurrentAsset)
            {
                m_CurrentAsset = m_Behaviour.CurrentAsset;
                _ = m_Behaviour.GetReachableStatuses();
            }

            if (!m_Behaviour.TryGetAssetProperties(m_CurrentAsset.Descriptor.AssetVersion, out var properties))
            {
                GUILayout.Label(" ! Asset properties not loaded !");
                return;
            }

            GUILayout.BeginVertical();

            GUILayout.Label($"Current Status: {properties.StatusName}");

            GUILayout.Space(5f);

            if (m_Behaviour.ReachableStatuses == null)
            {
                GUILayout.Label("Reachable Statuses: Loading...");
            }
            else
            {
                GUILayout.Label("Reachable Statuses:");
                foreach (var status in m_Behaviour.ReachableStatuses)
                {
                    if (GUILayout.Button(status))
                    {
                        _ = m_Behaviour.UpdateStatusAsync(status);
                    }
                }
            }

            GUILayout.EndVertical();
        }

        #endregion
    }

    class UseCaseManageAssetStatusExampleBehaviour
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public bool IsProjectSelected => m_Behaviour.IsProjectSelected;
        public IAsset CurrentAsset => m_Behaviour.CurrentAsset;
        public bool TryGetAssetProperties(AssetVersion assetVersion, out AssetProperties properties) => m_Behaviour.TryGetAssetProperties(assetVersion, out properties);

        public UseCaseManageAssetStatusExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_Behaviour_GetReachableStatuses

        public string[] ReachableStatuses { get; private set; }

        public async Task GetReachableStatuses()
        {
            ReachableStatuses = null;
            ReachableStatuses = await CurrentAsset.GetReachableStatusNamesAsync(default);
        }

        #endregion

        #region Example_Behaviour_UpdateStatus

        public async Task UpdateStatusAsync(string reachableStatus)
        {
            await CurrentAsset.UpdateStatusAsync(reachableStatus, default);
            await GetReachableStatuses();
        }

        #endregion
    }
}
