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

    public class UseCaseAggregationExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public UseCaseAggregationExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseAggregationExample : IAssetManagementUI
    {
        readonly UseCaseAggregationExampleBehaviour m_Behaviour;

        public UseCaseAggregationExample(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = new UseCaseAggregationExampleBehaviour(behaviour);
        }

        #region Example_UIContent

        readonly string[] m_AggregationFields = Enum.GetNames(typeof(GroupableField));
        int m_SelectedIndex = -1;

        public void OnGUI()
        {
            if (!m_Behaviour.IsProjectSelected) return;

            GUILayout.BeginVertical();

            // Go back to select a different scene.
            if (GUILayout.Button("Back"))
            {
                m_Behaviour.SetSelectedProject(null);
                return;
            }

            GUILayout.EndVertical();

            GUILayout.Space(15f);

            GUILayout.BeginVertical();

            GUILayout.Label("Aggregate by: ");
            var selection = GUILayout.SelectionGrid(m_SelectedIndex, m_AggregationFields, 4);
            if (selection != m_SelectedIndex && selection >= 0)
            {
                m_SelectedIndex = selection;
                var aggregationField = (GroupableField)Enum.Parse(typeof(GroupableField), m_AggregationFields[m_SelectedIndex]);
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

            GUILayout.EndVertical();
        }

        #endregion
    }

    class UseCaseAggregationExampleBehaviour
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public bool IsProjectSelected => m_Behaviour.IsProjectSelected;
        IAssetProject CurrentProject => m_Behaviour.CurrentProject;

        public UseCaseAggregationExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void SetSelectedProject(IAssetProject project) => m_Behaviour.SetSelectedProject(project);

        #region Example_Behaviour

        public IReadOnlyDictionary<string, int> GroupCounters { get; private set; }
        public int Total { get; private set; }

        public async Task AggregateByField(GroupableField groupableField)
        {
            GroupCounters = null;
            GroupCounters = await CurrentProject.GroupAndCountAssets().ExecuteAsync(groupableField, CancellationToken.None);
            Total = GroupCounters.Values.Sum();
        }

        public async Task AggregateByCollection()
        {
            GroupCounters = null;
            var collections = await CurrentProject.GroupAndCountAssets().GroupByCollectionAndExecuteAsync(CancellationToken.None);
            GroupCounters = collections.ToDictionary(x => x.Key.Path.ToString(), x => x.Value);
            Total = GroupCounters.Values.Sum();
        }

        #endregion
    }
}
