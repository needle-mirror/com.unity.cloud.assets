namespace Unity.Cloud.Documentation.Assets
{
#pragma warning disable S4487 // Unread "private" fields should be removed
#pragma warning disable S1186 // Methods should not be empty

    #region Example_UIClass

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using UnityEngine;
    using Unity.Cloud.Assets;
    using Unity.Cloud.Common;
    using Unity.Cloud.Identity;

    public class UseCaseCreateAssetExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;
        readonly string[] m_AssetTypeList;

        public UseCaseCreateAssetExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
            m_AssetTypeList = AssetTypeExtensions.AssetTypeList().ToArray();
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseCreateAssetExample : IAssetManagementUI
    {
        readonly UseCaseCreateAssetExampleBehaviour m_Behaviour;
        readonly string[] m_AssetTypeList;

        public UseCaseCreateAssetExample(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = new UseCaseCreateAssetExampleBehaviour(behaviour);
            m_AssetTypeList = AssetTypeExtensions.AssetTypeList().ToArray();
        }

        #region Example_UIContent

        OrganizationId m_OrganizationId;
        AssetType m_SelectedType = AssetType.Other;
        int m_SelectedStatusFlow;

        public void OnGUI()
        {
            if (!m_Behaviour.IsProjectSelected) return;

            if (m_OrganizationId != m_Behaviour.CurrentOrganization.Id)
            {
                m_OrganizationId = m_Behaviour.CurrentOrganization.Id;
                _ = m_Behaviour.GetOrganizationStatusFlows();
            }

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Type");
            var type = (int) m_SelectedType;
            type = GUILayout.SelectionGrid(type, m_AssetTypeList, 3, GUILayout.ExpandWidth(true));
            if (type != -1)
                m_SelectedType = (AssetType) type;

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Status Flow");

            if (m_Behaviour.AvailableStatusFlows == null)
            {
                GUILayout.Label("Loading...");
            }
            else
            {
                var statusFlowNames = m_Behaviour.AvailableStatusFlows.Select(x => x.Name).ToArray();
                m_SelectedStatusFlow = GUILayout.SelectionGrid(m_SelectedStatusFlow, statusFlowNames, 3, GUILayout.ExpandWidth(true));
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Create", GUILayout.Width(60)))
            {
                var statusFlowId = string.Empty;
                if (m_Behaviour.AvailableStatusFlows != null && m_SelectedStatusFlow >= 0 && m_SelectedStatusFlow < m_Behaviour.AvailableStatusFlows.Count)
                {
                    statusFlowId = m_Behaviour.AvailableStatusFlows[m_SelectedStatusFlow].Descriptor.StatusFlowId;
                }

                _ = m_Behaviour.CreateAssetAsync(m_SelectedType, statusFlowId);
            }

            GUILayout.EndVertical();
        }

        #endregion
    }

    class UseCaseCreateAssetExampleBehaviour
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public IOrganization CurrentOrganization => m_Behaviour.CurrentOrganization;
        public bool IsProjectSelected => m_Behaviour.IsProjectSelected;
        IAssetProject CurrentProject => m_Behaviour.CurrentProject;

        public UseCaseCreateAssetExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        async Task GetAssetsAsync(AssetDescriptor selectedAsset)
        {
            await Task.Delay(1000);
            await m_Behaviour.GetAssetsAsync(CurrentProject.QueryAssets(), selectedAsset);
        }

        #region Example_Behaviour_CreateAsset

        public List<IStatusFlow> AvailableStatusFlows { get; private set; }

        public async Task GetOrganizationStatusFlows()
        {
            AvailableStatusFlows = null;

            try
            {
                var statusFlowsAsync = PlatformServices.AssetRepository.ListStatusFlowsAsync(CurrentOrganization.Id, Range.All, CancellationToken.None);
                AvailableStatusFlows = new List<IStatusFlow>();
                await foreach (var statusFlow in statusFlowsAsync)
                {
                    AvailableStatusFlows.Add(statusFlow);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to get organization status flows. {e}");
                throw;
            }
        }

        public async Task CreateAssetAsync(AssetType assetType, string statusFlowId)
        {
            var assetCreation = new AssetCreation("GrayTexture_0")
            {
                Description = "Documentation example asset creation.",
                Type = assetType,
                StatusFlowDescriptor = string.IsNullOrEmpty(statusFlowId) ? null : new StatusFlowDescriptor(CurrentOrganization.Id, statusFlowId)
            };

            try
            {
                var assetDescriptor = await CurrentProject.CreateAssetLiteAsync(assetCreation, CancellationToken.None);
                await GetAssetsAsync(assetDescriptor);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create asset. {e}");
                throw;
            }
        }

        #endregion
    }
}
