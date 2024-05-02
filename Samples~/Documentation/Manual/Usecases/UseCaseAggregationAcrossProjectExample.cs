using Unity.Cloud.Common;

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
    using Unity.Cloud.Assets;
    using UnityEngine;

    public class UseCaseAggregationAcrossProjectsExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public UseCaseAggregationAcrossProjectsExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseAggregationAcrossProjectsExample : IAssetManagementUI
    {
        readonly UseCaseAggregationAcrossProjectsExampleBehaviour m_Behaviour;

        public UseCaseAggregationAcrossProjectsExample(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = new UseCaseAggregationAcrossProjectsExampleBehaviour(behaviour);
        }

        #region Example_UIContent

        readonly string[] m_AggregationFields = Enum.GetNames(typeof(GroupableField));
        int m_SelectedIndex = -1;

        public void OnGUI()
        {
            if (!m_Behaviour.IsOrganizationSelected)
            {
                m_SelectedIndex = -1;
                return;
            }

            if (m_Behaviour.IsProjectSelected) return;

            GUILayout.Space(15f);

            GUILayout.BeginVertical();

            GUI.enabled = m_Behaviour.AvailableProjects.Any();

            GUILayout.Label("Aggregate by: ");
            var selection = GUILayout.SelectionGrid(m_SelectedIndex, m_AggregationFields, 4);
            if (selection != m_SelectedIndex && selection >= 0)
            {
                m_SelectedIndex = selection;
                var aggregationField = (GroupableField) Enum.Parse(typeof(GroupableField), m_AggregationFields[m_SelectedIndex]);
                _ = m_Behaviour.AggregateByField(aggregationField);
            }

            if (GUILayout.Button("Collections"))
            {
                _ = m_Behaviour.AggregateByCollection();
            }

            GUILayout.Label("Aggregation Results:");
            if (m_Behaviour.GroupCounters != null)
            {
                GUILayout.Label($"Total: {m_Behaviour.Total}");
                GUILayout.Label($"Unique: {m_Behaviour.GroupCounters.Keys.Count()}");
                GUILayout.Label($"Values:");
                foreach (var value in m_Behaviour.GroupCounters)
                {
                    GUILayout.Label($"- {value.Key}: {value.Value}");
                }
            }
            else
            {
                GUILayout.Label("Empty.");
            }

            GUI.enabled = true;

            GUILayout.EndVertical();
        }

        #endregion
    }

    class UseCaseAggregationAcrossProjectsExampleBehaviour
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public bool IsOrganizationSelected => m_Behaviour.IsOrganizationSelected;
        public bool IsProjectSelected => m_Behaviour.IsProjectSelected;
        public IEnumerable<IAssetProject> AvailableProjects => m_Behaviour.AvailableProjects;

        public UseCaseAggregationAcrossProjectsExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_Behaviour

        public IReadOnlyDictionary<string, int> GroupCounters { get; private set; }
        public int Total { get; private set; }

        IEnumerable<ProjectDescriptor> AvailableProjectDescriptors => m_Behaviour.AvailableProjects.Select(x => x.Descriptor);

        public async Task AggregateByField(GroupableField groupableField)
        {
            GroupCounters = null;
            GroupCounters = await PlatformServices.AssetRepository.GroupAndCountAssets(AvailableProjectDescriptors)
                .ExecuteAsync(groupableField, CancellationToken.None);
            Total = GroupCounters.Values.Sum();
        }

        public async Task AggregateByCollection()
        {
            GroupCounters = null;
            var collections = await PlatformServices.AssetRepository.GroupAndCountAssets(AvailableProjectDescriptors)
                .GroupByCollectionAndExecuteAsync(CancellationToken.None);
            GroupCounters = collections.ToDictionary(x => x.Key.Path.ToString(), x => x.Value);
            Total = GroupCounters.Values.Sum();
        }

        #endregion
    }
}
