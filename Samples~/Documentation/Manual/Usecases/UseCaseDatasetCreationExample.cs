namespace Unity.Cloud.Documentation.Assets
{
#pragma warning disable S4487 // Unread "private" fields should be removed
#pragma warning disable S1186 // Methods should not be empty

    #region Example_UIClass

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using UnityEngine;
    using Unity.Cloud.Assets;

    public class UseCaseDatasetCreationExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public UseCaseDatasetCreationExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseDatasetCreationExample : IAssetManagementUI
    {
        readonly UseCaseDatasetCreationExampleBehaviour m_Behaviour;

        public UseCaseDatasetCreationExample(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = new UseCaseDatasetCreationExampleBehaviour(behaviour);
        }

        #region Example_UIContent

        string m_NewDatasetName = "MyDataset";

        IAsset m_CurrentAsset;
        Vector2 m_DatasetListScrollPosition;

        public void OnGUI()
        {
            if (!m_Behaviour.IsProjectSelected) return;

            if (m_CurrentAsset != m_Behaviour.CurrentAsset)
            {
                m_CurrentAsset = m_Behaviour.CurrentAsset;
                m_Behaviour.Datasets = null;
            }

            if (m_CurrentAsset == null)
            {
                GUILayout.Label(" ! No asset selected !");
                return;
            }

            GUILayout.BeginVertical();

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

            GUILayout.EndVertical();
        }

        void DisplayDatasets(IReadOnlyList<IDataset> datasets)
        {
            if (datasets.Count == 0)
            {
                GUILayout.Label(" ! No datasets !");
            }
            else
            {
                m_DatasetListScrollPosition = GUILayout.BeginScrollView(m_DatasetListScrollPosition, GUILayout.Height(Screen.height * 0.8f));

                for (var i = 0; i < datasets.Count; ++i)
                {
                    var dataset = datasets[i];
                    GUILayout.Label($"{dataset.Name}, {dataset.Status} - {(dataset.IsVisible ? "visible" : "hidden")}");
                }

                GUILayout.EndScrollView();
            }
        }

        #endregion
    }

    class UseCaseDatasetCreationExampleBehaviour
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public bool IsProjectSelected => m_Behaviour.IsProjectSelected;
        public IAsset CurrentAsset => m_Behaviour.CurrentAsset;

        public UseCaseDatasetCreationExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_Behaviour_RefreshDatasets

        public List<IDataset> Datasets { get; set; }

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
