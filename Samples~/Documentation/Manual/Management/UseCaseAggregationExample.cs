using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Cloud.Assets.Documentation.Management
{
    public class UseCaseAggregationExample
    {
        readonly UseCaseAggregationExampleBehaviour m_Behaviour = new();

        public void DisplayExample(IAssetProject project)
        {
            m_Behaviour.Initialize(project);
            AssetActions();
        }

        #region Example_UI
        string m_CustomAggregationField;

        protected virtual void AssetActions()
        {
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
        }

        #endregion
    }

    class UseCaseAggregationExampleBehaviour
    {
        // Member names should match with the names of the get-started behaviour snippets.
        IAssetProject m_CurrentProject;

        public void Initialize(IAssetProject project)
        {
            m_CurrentProject = project;
        }

        #region Example_Behaviour

        public Aggregation Aggregation { get; private set; }

        public async Task AggregateByField(string aggregationField)
        {
            var assetSearchFilter = new AssetSearchFilter();
            var aggregationParameters = new AggregationParameters(aggregationField);

            var cancellationTokenSrc = new CancellationTokenSource();
            Aggregation = await m_CurrentProject.CountAssetsAsync(assetSearchFilter, aggregationParameters, cancellationTokenSrc.Token);
        }

        #endregion
    }
}
