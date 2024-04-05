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

            GUILayout.EndVertical();

            if (m_Behaviour.CurrentDataset == null)
            {
                GUILayout.Label(" ! No dataset selected !");
                return;
            }

            GUILayout.BeginVertical();

            DisplayDataset();

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
                    GUILayout.BeginHorizontal();

                    var dataset = datasets[i];
                    GUILayout.Label($"{dataset.Name}");

                    if (GUILayout.Button("Select"))
                    {
                        m_Behaviour.SetCurrentDataset(dataset);
                        m_DatasetUpdate = new DatasetUpdate(dataset);
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();
            }
        }

        void DisplayDataset()
        {
            GUILayout.Label("Name:");
            m_DatasetUpdate.Name = GUILayout.TextField(m_DatasetUpdate.Name);

            GUILayout.Label("Description:");
            m_DatasetUpdate.Description = GUILayout.TextArea(m_DatasetUpdate.Description);

            m_DatasetUpdate.IsVisible = GUILayout.Toggle(m_DatasetUpdate.IsVisible ?? false, "Is visible");

            GUILayout.Label("Tags:");
            var tags = GUILayout.TextField(string.Join(",", m_DatasetUpdate.Tags));
            m_DatasetUpdate.Tags = tags.Split(',').Select(tag => tag.Trim()).ToList();

            if (GUILayout.Button("Update"))
            {
                _ = UpdateDataset();
            }
        }

        async Task UpdateDataset()
        {
            await m_Behaviour.UpdateDataset(m_DatasetUpdate);
            m_DatasetUpdate = new DatasetUpdate(m_Behaviour.CurrentDataset);
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

        public List<IDataset> Datasets { get; set; }
        public IDataset CurrentDataset { get; private set; }

        public async Task GetDatasets()
        {
            Datasets = new List<IDataset>();
            CurrentDataset = null;

            _ = CurrentAsset.RefreshAsync(CancellationToken.None);
            var asyncList = CurrentAsset.ListDatasetsAsync(Range.All, CancellationToken.None);
            await foreach (var dataset in asyncList)
            {
                Datasets.Add(dataset);
            }
        }

        public void SetCurrentDataset(IDataset dataset)
        {
            CurrentDataset = dataset;
        }

        #endregion

        #region Example_Behaviour_UpdateDataset

        public async Task UpdateDataset(IDatasetUpdate update)
        {
            try
            {
                await CurrentDataset.UpdateAsync(update, CancellationToken.None);
                await CurrentDataset.RefreshAsync(CancellationToken.None);
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
