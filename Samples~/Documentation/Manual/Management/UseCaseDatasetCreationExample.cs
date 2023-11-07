using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Cloud.Assets.Documentation.Management
{
    public class UseCaseDatasetCreationExample
    {
        readonly UseCaseDatasetCreationExampleBehaviour m_Behaviour = new();

        public void DisplayExample(IAsset asset)
        {
            m_Behaviour.Initialize(asset);
            AssetActions();
        }

        #region Example_UI

        string m_NewDatasetName = "MyDataset";

        protected virtual void AssetActions()
        {
            if (m_Behaviour.CurrentAsset == null)
            {
                GUILayout.Label(" ! No asset selected !");
                return;
            }

            if (GUILayout.Button("Refresh") || m_Behaviour.Datasets == null)
            {
                _ = m_Behaviour.GetDatasets();
            }

            GUILayout.Label("Asset datasets:");
            DisplayDatasets(m_Behaviour.Datasets?.ToArray() ?? Array.Empty<IDataset>());

            GUILayout.BeginHorizontal();
            m_NewDatasetName = GUILayout.TextField(m_NewDatasetName, GUILayout.MinWidth(100f));
            if (GUILayout.Button("Create Dataset"))
            {
                _ = m_Behaviour.CreateDataset(m_NewDatasetName);
            }

            GUILayout.EndHorizontal();
        }

        static void DisplayDatasets(IReadOnlyList<IDataset> datasets)
        {
            if (datasets.Count == 0)
            {
                GUILayout.Label(" ! No datasets !");
            }
            else
            {
                for (var i = 0; i < datasets.Count; ++i)
                {
                    var dataset = datasets[i];
                    GUILayout.Label($"{dataset.Name}, {dataset.Status} - {(dataset.IsVisible ? "visible" : "hidden")}");
                }
            }
        }

        #endregion
    }

    class UseCaseDatasetCreationExampleBehaviour
    {
        // Member names should match with the names of the get-started behaviour snippets.
        public IAsset CurrentAsset;

        public void Initialize(IAsset asset)
        {
            if (asset != CurrentAsset)
            {
                Datasets = null;
            }
            CurrentAsset = asset;
        }

        #region Example_Behaviour_RefreshDatasets

        public List<IDataset> Datasets { get; private set; }

        public async Task GetDatasets()
        {
            Datasets = new List<IDataset>();

            var asyncList = CurrentAsset.ListDatasetsAsync(Range.All, CancellationToken.None);
            await foreach (var dataset in asyncList)
            {
                Datasets.Add(dataset);
            }
        }

        #endregion

        #region Example_Behaviour_CreateDataset

        public async Task CreateDataset(string name)
        {
            var datasetCreation = new DatasetCreation(name)
            {
                Description = "Documentation example asset dataset creation.",
                Tags = new List<string> {"Custom"}
            };

            var cancellationTokenSrc = new CancellationTokenSource();

            try
            {
                var dataset = await CurrentAsset.CreateDatasetAsync(datasetCreation, cancellationTokenSrc.Token);
                Datasets.Add(dataset);

                Debug.Log($"Asset dataset creation: {dataset.Name} added.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create dataset. {e}");
                throw;
            }
        }

        #endregion
    }
}
