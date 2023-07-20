using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Cloud.Assets.Documentation.Discovery
{
    public class UseCaseAggregationExample
    {
        readonly UseCaseAggregationExampleBehaviour m_Behaviour = new();

        public void DisplayExample(IOrganization organization, IProject project, IAssetProvider assetProvider)
        {
            m_Behaviour.Initialize(organization, project, assetProvider);
            AssetActions();
        }

        #region Example_UI

        protected virtual void AssetActions()
        {
            if (GUILayout.Button("Aggregate by Type"))
            {
                _ = m_Behaviour.AggregateByField(nameof(IAsset.Type));
            }

            if (GUILayout.Button("Aggregate by Tag"))
            {
                _ = m_Behaviour.AggregateByField(nameof(IAsset.Tags));
            }

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
        // Local struct reflects what calls to the static PlatformServices would look like.
        struct PlatformServicesProxy
        {
            public IAssetProvider AssetProvider;
        }

        // Member names should match with the names of the get-started behaviour snippets.
        PlatformServicesProxy PlatformServices;
        IOrganization m_CurrentOrganization;
        IProject m_CurrentProject;

        public void Initialize(IOrganization organization, IProject project, IAssetProvider assetProvider)
        {
            m_CurrentOrganization = organization;
            m_CurrentProject = project;
            PlatformServices.AssetProvider = assetProvider;
        }

        #region Example_Behaviour

        public Aggregation Aggregation { get; private set; }

        public async Task AggregateByField(string aggregationField)
        {
            var assetSearchFilter = new AssetSearchFilter(m_CurrentOrganization, m_CurrentProject);
            var aggregationParameters = new AggregationParameters(aggregationField);

            var cancellationTokenSrc = new CancellationTokenSource();
            Aggregation = await PlatformServices.AssetProvider.AggregateAsync(assetSearchFilter, aggregationParameters, cancellationTokenSrc.Token);
        }

        #endregion
    }
}
