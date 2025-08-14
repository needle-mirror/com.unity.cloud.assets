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
    using Unity.Cloud.Common;

    public class UseCaseCreateDatasetExampleUI : IAssetManagementUI
    {
        readonly BaseAssetBehaviour m_Behaviour;

        public UseCaseCreateDatasetExampleUI(BaseAssetBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseCreateDatasetExample : IAssetManagementUI
    {
        readonly UseCaseCreateDatasetExampleBehaviour m_Behaviour;

        public UseCaseCreateDatasetExample(BaseAssetBehaviour behaviour)
        {
            m_Behaviour = new UseCaseCreateDatasetExampleBehaviour(behaviour);
        }

        #region Example_UIContent

        string m_NewDatasetName = "MyDataset";

        IAsset m_CurrentAsset;
        Vector2 m_ListScrollPosition;

        public void OnGUI()
        {
            if (m_Behaviour.CurrentAsset == null) return;

            if (m_CurrentAsset != m_Behaviour.CurrentAsset)
            {
                m_CurrentAsset = m_Behaviour.CurrentAsset;
                _ = m_Behaviour.GetDatasets();
            }

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();

            m_NewDatasetName = GUILayout.TextField(m_NewDatasetName, GUILayout.MinWidth(100f));
            if (GUILayout.Button("Create Dataset"))
            {
                _ = m_Behaviour.CreateDataset(m_NewDatasetName);
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(15f);

            if (GUILayout.Button("Refresh", GUILayout.Width(60)))
            {
                _ = m_Behaviour.GetDatasets();
            }

            DisplayDatasets(m_Behaviour.Datasets?.ToArray() ?? Array.Empty<IDataset>());

            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            DisplaySelectedDataset();

            GUILayout.EndVertical();
        }

        void DisplayDatasets(IReadOnlyList<IDataset> datasets)
        {
            if (datasets.Count == 0)
            {
                GUILayout.Label(" ! No datasets !");
                return;
            }

            m_ListScrollPosition = GUILayout.BeginScrollView(m_ListScrollPosition, GUILayout.ExpandHeight(true));

            foreach (var dataset in datasets)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label(m_Behaviour.DatasetProperties.TryGetValue(dataset.Descriptor.DatasetId, out var properties)
                    ? $"{properties.Name}"
                    : $"{dataset.Descriptor.DatasetId}");

                GUI.enabled = m_Behaviour.CurrentDatasetId != dataset.Descriptor.DatasetId;

                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    m_Behaviour.CurrentDatasetId = dataset.Descriptor.DatasetId;
                }

                GUI.enabled = true;

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        void DisplaySelectedDataset()
        {
            if (m_Behaviour.CurrentDatasetId == null) return;

            if (!m_Behaviour.DatasetProperties.TryGetValue(m_Behaviour.CurrentDatasetId.Value, out var properties))
            {
                GUILayout.Label("Loading properties...");
                return;
            }

            GUILayout.Label($"Description: {properties.Description}");
            GUILayout.Label($"Type: {properties.Type}");
            GUILayout.Label($"Tags: {string.Join(", ", properties.Tags)}");
            GUILayout.Label($"System Tags: {string.Join(", ", properties.SystemTags)}");
            GUILayout.Label($"Status: {properties.StatusName}");
            GUILayout.Label("Is Visible: " + (properties.IsVisible ? "Yes" : "No"));
        }

        #endregion
    }

    class UseCaseCreateDatasetExampleBehaviour
    {
        readonly BaseAssetBehaviour m_Behaviour;

        public IAsset CurrentAsset => m_Behaviour.CurrentAsset;

        public UseCaseCreateDatasetExampleBehaviour(BaseAssetBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_Behaviour_RefreshDatasets

        public List<IDataset> Datasets { get; } = new();
        public DatasetId? CurrentDatasetId { get; set; }
        public Dictionary<DatasetId, DatasetProperties> DatasetProperties { get; } = new();

        CancellationTokenSource m_DatasetCancellationSource;

        public async Task GetDatasets()
        {
            var datasetId = CurrentDatasetId;
            CurrentDatasetId = null;
            Datasets.Clear();
            DatasetProperties.Clear();

            var token = GetDatasetCancellationToken();

            await CurrentAsset.RefreshAsync(token);

            var asyncList = CurrentAsset.ListDatasetsAsync(Range.All, token);
            await foreach (var dataset in asyncList)
            {
                Datasets.Add(dataset);

                if (datasetId == dataset.Descriptor.DatasetId)
                {
                    CurrentDatasetId = datasetId;
                }

                DatasetProperties[dataset.Descriptor.DatasetId] = await dataset.GetPropertiesAsync(token);
            }
        }

        public string GetDatasetName(DatasetId datasetId) => DatasetProperties.TryGetValue(datasetId, out var properties)
            ? properties.Name
            : $"{datasetId}";

        CancellationToken GetDatasetCancellationToken()
        {
            if (m_DatasetCancellationSource != null)
            {
                m_DatasetCancellationSource.Cancel();
                m_DatasetCancellationSource.Dispose();
            }

            m_DatasetCancellationSource = new CancellationTokenSource();
            return m_DatasetCancellationSource.Token;
        }

        #endregion

        #region Example_Behaviour_CreateDataset

        public async Task CreateDataset(string name)
        {
            IDatasetCreation datasetCreation = new DatasetCreation(name)
            {
                Description = "Documentation example asset dataset creation.",
                Tags = new List<string> {"Custom"}
            };

            try
            {
                var dataset = await CurrentAsset.CreateDatasetAsync(datasetCreation, CancellationToken.None);
                var properties = await dataset.GetPropertiesAsync(CancellationToken.None);
                DatasetProperties[dataset.Descriptor.DatasetId] = properties;

                Debug.Log($"Asset dataset creation: {properties.Name} added.");
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
