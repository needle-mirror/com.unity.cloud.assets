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
    using Unity.Cloud.Common;
    using UnityEngine;

    public class UseCaseDatasetUpdateExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public UseCaseDatasetUpdateExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseDatasetUpdateExample : IAssetManagementUI
    {
        readonly UseCaseDatasetUpdateExampleBehaviour m_Behaviour;

        public UseCaseDatasetUpdateExample(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = new UseCaseDatasetUpdateExampleBehaviour(behaviour);
        }

        #region Example_UIContent

        IAsset m_CurrentAsset;
        Vector2 m_DatasetListScrollPosition;
        DatasetUpdate m_DatasetUpdate;
        string m_TagsString = string.Empty;

        public void OnGUI()
        {
            if (!m_Behaviour.IsProjectSelected) return;

            if (m_CurrentAsset == null)
            {
                GUILayout.Label(" ! No asset selected !");
                return;
            }

            if (m_CurrentAsset != m_Behaviour.CurrentAsset)
            {
                m_CurrentAsset = m_Behaviour.CurrentAsset;
                _ = m_Behaviour.GetDatasets();
            }

            GUILayout.BeginVertical();

            if (GUILayout.Button("Refresh", GUILayout.Width(60)))
            {
                _ = m_Behaviour.GetDatasets();
            }

            GUILayout.Label("Datasets:");
            DisplayDatasets(m_Behaviour.Datasets.ToArray());

            GUILayout.EndVertical();

            if (m_Behaviour.CurrentDatasetId == null)
            {
                GUILayout.Label(" ! No dataset selected !");
                return;
            }

            GUILayout.BeginVertical();

            DisplayDataset(m_Behaviour.DatasetProperties.GetValueOrDefault(m_Behaviour.CurrentDatasetId.Value));

            GUILayout.EndVertical();
        }

        void DisplayDatasets(IReadOnlyCollection<IDataset> datasets)
        {
            if (datasets.Count == 0)
            {
                GUILayout.Label(" ! No datasets !");
                return;
            }

            m_DatasetListScrollPosition = GUILayout.BeginScrollView(m_DatasetListScrollPosition, GUILayout.Height(Screen.height * 0.8f));

            foreach (var dataset in datasets)
            {
                if (!m_Behaviour.DatasetProperties.TryGetValue(dataset.Descriptor.DatasetId, out var properties))
                {
                    GUILayout.Label(dataset.Descriptor.DatasetId.ToString());
                    continue;
                }

                GUILayout.BeginHorizontal();

                GUILayout.Label($"{properties.Name}");

                GUI.enabled = dataset.Descriptor.DatasetId != m_Behaviour.CurrentDatasetId;

                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    m_Behaviour.CurrentDatasetId = dataset.Descriptor.DatasetId;
                    m_DatasetUpdate = new DatasetUpdate
                    {
                        Name = properties.Name,
                        Description = properties.Description,
                        IsVisible = properties.IsVisible,
                        Tags = properties.Tags?.ToList() ?? new List<string>()
                    };
                    m_TagsString = string.Join(',', m_DatasetUpdate.Tags);
                }

                GUI.enabled = true;

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        void DisplayDataset(DatasetProperties datasetProperties)
        {
            GUILayout.Label("Name:");
            m_DatasetUpdate.Name = GUILayout.TextField(m_DatasetUpdate.Name);

            GUILayout.Label("Description:");
            m_DatasetUpdate.Description = GUILayout.TextArea(m_DatasetUpdate.Description);

            m_DatasetUpdate.IsVisible = GUILayout.Toggle(m_DatasetUpdate.IsVisible ?? false, "Is visible");

            GUILayout.Label("Tags:");
            m_TagsString = GUILayout.TextField(m_TagsString, GUILayout.ExpandWidth(true));
            m_DatasetUpdate.Tags = m_TagsString.Split(',')
                .Select(tag => tag.Trim())
                .Where(tag => !string.IsNullOrEmpty(tag))
                .ToList();

            if (GUILayout.Button("Update"))
            {
                _ = m_Behaviour.UpdateDataset(m_DatasetUpdate);
            }

            GUILayout.Label($"System tags: {string.Join(", ", datasetProperties.SystemTags)}");
        }

        #endregion
    }

    class UseCaseDatasetUpdateExampleBehaviour
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public bool IsProjectSelected => m_Behaviour.IsProjectSelected;
        public IAsset CurrentAsset => m_Behaviour.CurrentAsset;

        public UseCaseDatasetUpdateExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_Behaviour_RefreshDatasets

        public List<IDataset> Datasets { get; } = new();
        public DatasetId? CurrentDatasetId { get; set; }
        public Dictionary<DatasetId, DatasetProperties> DatasetProperties { get; } = new();

        public async Task GetDatasets()
        {
            var datasetId = CurrentDatasetId;
            CurrentDatasetId = null;
            Datasets.Clear();
            DatasetProperties.Clear();

            await CurrentAsset.RefreshAsync(CancellationToken.None);

            var asyncList = CurrentAsset.ListDatasetsAsync(Range.All, CancellationToken.None);
            await foreach (var dataset in asyncList)
            {
                Datasets.Add(dataset);

                if (datasetId == dataset.Descriptor.DatasetId)
                {
                    CurrentDatasetId = datasetId;
                }

                DatasetProperties[dataset.Descriptor.DatasetId] = await dataset.GetPropertiesAsync(CancellationToken.None);
            }
        }

        #endregion

        #region Example_Behaviour_UpdateDataset

        public async Task UpdateDataset(IDatasetUpdate update)
        {
            if (CurrentDatasetId == null) return;

            try
            {
                var datasetId = CurrentDatasetId.Value;

                var dataset = Datasets.FirstOrDefault(x => x.Descriptor.DatasetId == CurrentDatasetId)
                    ?? await CurrentAsset.GetDatasetAsync(datasetId, CancellationToken.None);

                await dataset.UpdateAsync(update, CancellationToken.None);
                await dataset.RefreshAsync(CancellationToken.None);

                DatasetProperties[datasetId] = await dataset.GetPropertiesAsync(CancellationToken.None);

                Debug.Log($"Dataset update succeeded.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to update dataset. {e}");
                throw;
            }
        }

        #endregion
    }
}
