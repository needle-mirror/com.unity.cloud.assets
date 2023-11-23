using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Cloud.Assets.Documentation
{
#pragma warning disable S4487 // Unread "private" fields should be removed
#pragma warning disable S1186 // Methods should not be empty

    #region Example_UIClass

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

        string m_CustomAggregationField;

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

            if (GUILayout.Button("Aggregate by Type"))
            {
                _ = m_Behaviour.AggregateByField(AssetTypeSearchCriteria.SearchKey);
            }

            if (GUILayout.Button("Aggregate by Tag"))
            {
                _ = m_Behaviour.AggregateByField(nameof(IAsset.Tags));
            }

            if (GUILayout.Button("Aggregate by Status"))
            {
                _ = m_Behaviour.AggregateByField(nameof(IAsset.Status));
            }

            GUILayout.BeginHorizontal();
            m_CustomAggregationField = GUILayout.TextField(m_CustomAggregationField, GUILayout.MinWidth(120f));
            if (GUILayout.Button("Aggregate"))
            {
                _ = m_Behaviour.AggregateByField(m_CustomAggregationField);
            }

            GUILayout.EndHorizontal();

            GUILayout.Label("Aggregation Results:");
            if (m_Behaviour.Aggregation != null)
            {
                GUILayout.Label($"Total: {m_Behaviour.Aggregation.Total}");
                GUILayout.Label($"Unique: {m_Behaviour.Aggregation.Unique}");
                GUILayout.Label($"Values:");
                foreach (var value in m_Behaviour.Aggregation.Values)
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
        public IAssetProject CurrentProject => m_Behaviour.CurrentProject;

        public UseCaseAggregationExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void SetSelectedProject(IAssetProject project) => m_Behaviour.SetSelectedProject(project);

        #region Example_Behaviour

        public Aggregation Aggregation { get; private set; }

        public async Task AggregateByField(string aggregationField)
        {
            var assetSearchFilter = new AssetSearchFilter();
            var aggregationParameters = new AggregationParameters(aggregationField);

            var cancellationTokenSrc = new CancellationTokenSource();
            Aggregation = await CurrentProject.CountAssetsAsync(assetSearchFilter, aggregationParameters, cancellationTokenSrc.Token);
        }

        #endregion
    }
}
