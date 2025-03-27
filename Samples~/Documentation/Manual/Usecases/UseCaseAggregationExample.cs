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

        Vector2 m_ScrollPosition;

        public void OnGUI()
        {
            if (!m_Behaviour.IsProjectSelected)
            {
                m_SelectedIndex = -1;
                return;
            }

            GUILayout.Space(15f);

            GUILayout.BeginVertical();

            GUILayout.Label("Aggregate by: ");
            var selection = GUILayout.SelectionGrid(m_SelectedIndex, m_AggregationFields, 4);
            if (selection != m_SelectedIndex && selection >= 0)
            {
                m_SelectedIndex = selection;
                var aggregationField = (GroupableField) Enum.Parse(typeof(GroupableField), m_AggregationFields[m_SelectedIndex]);
                _ = m_Behaviour.AggregateByField(aggregationField);
            }

            GUILayout.Label("Aggregation Results:");
            if (m_Behaviour.Total > 0)
            {
                GUILayout.Label($"Total: {m_Behaviour.Total}");
                GUILayout.Label($"Unique: {m_Behaviour.GroupCounters.Keys.Count()}");
                GUILayout.Label($"Values:");
                
                m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition, GUILayout.ExpandHeight(true));

                foreach (var value in m_Behaviour.GroupCounters)
                {
                    GUILayout.Label($"- {value.Key}: {value.Value}");
                }

                GUILayout.EndScrollView();
            }
            else if (m_Behaviour.Total == 0)
            {
                GUILayout.Label("No results.");
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

        #region Example_Behaviour

        readonly Dictionary<string, int> m_GroupCounters = new();

        public IReadOnlyDictionary<string, int> GroupCounters => m_GroupCounters;
        public int Total { get; private set; }

        public async Task AggregateByField(Groupable groupable)
        {
            m_GroupCounters.Clear();
            Total = -1;

            var asyncEnumerable = CurrentProject.GroupAndCountAssets().ExecuteAsync(groupable, CancellationToken.None);
            await foreach (var group in asyncEnumerable)
            {
                switch (group.Key.Type)
                {
                    case GroupableFieldValueType.CollectionDescriptor:
                        m_GroupCounters.Add("[Collection] " + group.Key.AsCollectionDescriptor().Path, group.Value);
                        break;
                    default:
                        m_GroupCounters.Add($"[{group.Key.Type}] " + group.Key.AsString(), group.Value);
                        break;
                }

                if (Total == -1)
                {
                    Total = 0;
                }

                Total += group.Value;
            }

            if (Total == -1)
            {
                Total = 0;
            }
        }

        #endregion
    }
}
