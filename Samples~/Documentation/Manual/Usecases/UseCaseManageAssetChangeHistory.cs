using Unity.Cloud.Common;

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

    public class UseCaseManageAssetUpdateHistoryExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public UseCaseManageAssetUpdateHistoryExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseManageAssetUpdateHistoryExample : IAssetManagementUI
    {
        readonly UseCaseManageAssetUpdateHistoryExampleBehaviour m_Behaviour;

        public UseCaseManageAssetUpdateHistoryExample(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = new UseCaseManageAssetUpdateHistoryExampleBehaviour(behaviour);
        }

        #region Example_UIContent

        IAsset m_CurrentAsset;
        bool m_IncludeChildren;

        public void OnGUI()
        {
            if (!m_Behaviour.CanSelectAsset) return;

            if (m_Behaviour.CurrentAsset == null)
            {
                GUILayout.Label(" ! No asset selected !");
                return;
            }

            if (m_CurrentAsset != m_Behaviour.CurrentAsset)
            {
                m_CurrentAsset = m_Behaviour.CurrentAsset;
                m_Behaviour.SelectedUpdateHistoryIndex = -1;
                _ = m_Behaviour.GetUpdateHistory(m_IncludeChildren);
            }

            GUILayout.BeginVertical();

            var includeChildren = GUILayout.Toggle(m_IncludeChildren, "Include datasets and files");
            if (includeChildren != m_IncludeChildren)
            {
                m_IncludeChildren = includeChildren;
                _ = m_Behaviour.GetUpdateHistory(m_IncludeChildren);
            }

            if (GUILayout.Button("Refresh"))
            {
                _ = m_Behaviour.GetUpdateHistory(m_IncludeChildren);
            }

            ListUpdateHistory();

            GUILayout.EndVertical();

            GUILayout.Space(15);

            DisplaySelectedUpdateHistory();
        }

        void ListUpdateHistory()
        {
            if (m_Behaviour.UpdateHistories == null)
            {
                GUILayout.Label("Update History: Loading...");
            }
            else
            {
                GUILayout.Label("Update History:");
                var updateHistories = m_Behaviour.UpdateHistories.ToArray();
                for (var i = 0; i < updateHistories.Length; ++i)
                {
                    GUILayout.BeginHorizontal();

                    GUILayout.Label(updateHistories[i].SequenceNumber.ToString());

                    GUI.enabled = m_Behaviour.SelectedUpdateHistoryIndex != i;

                    if (GUILayout.Button("Select", GUILayout.Width(60)))
                    {
                        m_Behaviour.SelectedUpdateHistoryIndex = i;
                    }

                    GUI.enabled = true;

                    GUI.enabled = !(i == 0 && updateHistories[i].SequenceNumber == updateHistories.Length - 1);

                    if (GUILayout.Button("Roll back", GUILayout.Width(60)))
                    {
                        _ = m_Behaviour.UpdateAsync(updateHistories[i].SequenceNumber);
                    }

                    GUI.enabled = true;

                    GUILayout.EndHorizontal();
                }
            }
        }

        void DisplaySelectedUpdateHistory()
        {
            if (m_Behaviour.SelectedUpdateHistoryIndex < 0 || m_Behaviour.UpdateHistories == null ||
                m_Behaviour.SelectedUpdateHistoryIndex >= m_Behaviour.UpdateHistories.Count())
                return;

            GUILayout.BeginVertical();

            var updateHistory = m_Behaviour.UpdateHistories.ElementAt(m_Behaviour.SelectedUpdateHistoryIndex);
            GUILayout.Label($"Selected Update History: {updateHistory.SequenceNumber}");
            GUILayout.Label($"  Updated By: {updateHistory.UpdatedBy}");
            GUILayout.Label($"  Updated: {updateHistory.Updated}");
            GUILayout.Label($"  Name: {updateHistory.Name}");
            GUILayout.Label($"  Description: {updateHistory.Description}");
            GUILayout.Label($"  Type: {updateHistory.Type}");
            GUILayout.Label($"  Tags: {string.Join(", ", updateHistory.Tags)}");
            GUILayout.Label($"  Preview File Path: {updateHistory.PreviewFilePath}");
            if (updateHistory.Metadata.Count == 0)
            {
                GUILayout.Label("  Metadata: None");
            }
            else
            {
                GUILayout.Label("  Metadata:");
                foreach (var kvp in updateHistory.Metadata)
                {
                    GUILayout.Label($"    {kvp.Key}: {kvp.Value}");
                }
            }

            if (updateHistory.ChildDatasetUpdateHistoryDescriptor.HasValue)
            {
                var descriptor = updateHistory.ChildDatasetUpdateHistoryDescriptor.Value;
                GUILayout.Label($"    Dataset Id: {descriptor.DatasetId} [{descriptor.SequenceNumber}]");

                var hasHistory = m_Behaviour.TryGetChildUpdateHistory(descriptor, out var datasetHistory);
                if (hasHistory)
                {
                    GUILayout.Label($"    Updated: {datasetHistory.Updated}");
                    GUILayout.Label($"    Updated By: {datasetHistory.UpdatedBy}");
                    GUILayout.Label($"    Name: {datasetHistory.Name}");
                    GUILayout.Label($"    Description: {datasetHistory.Description}");
                    GUILayout.Label($"    Type: {datasetHistory.Type}");
                    GUILayout.Label($"    Is Visible: {datasetHistory.IsVisible}");
                    GUILayout.Label($"    Tags: {string.Join(", ", datasetHistory.Tags)}");
                    GUILayout.Label($"    File Order: {string.Join(", ", datasetHistory.FileOrder)}");

                    GUILayout.Space(15f);
                }
                else if (GUILayout.Button("Load Dataset Update History"))
                {
                    _ = m_Behaviour.LoadChildUpdateHistory(descriptor);
                }

                if (GUILayout.Button("Roll back Dataset to here"))
                {
                    _ = m_Behaviour.UpdateAsync(descriptor);
                }
            }

            if (updateHistory.ChildFileUpdateHistoryDescriptor.HasValue)
            {
                var descriptor = updateHistory.ChildFileUpdateHistoryDescriptor.Value;
                GUILayout.Label($"    File Path: {descriptor.FilePath} [{descriptor.SequenceNumber}]");

                var hasHistory = m_Behaviour.TryGetChildUpdateHistory(descriptor, out var fileHistory);
                if (hasHistory)
                {
                    GUILayout.Label($"    Updated: {fileHistory.Updated}");
                    GUILayout.Label($"    Updated By: {fileHistory.UpdatedBy}");
                    GUILayout.Label($"    Description: {fileHistory.Description}");
                    GUILayout.Label($"    Tags: {string.Join(", ", fileHistory.Tags)}");

                    GUILayout.Space(15f);
                }
                else if (GUILayout.Button("Load File Update History"))
                {
                    _ = m_Behaviour.LoadChildUpdateHistory(descriptor);
                }

                if (GUILayout.Button("Roll back File to here"))
                {
                    _ = m_Behaviour.UpdateAsync(descriptor);
                }
            }

            GUILayout.EndVertical();
        }

        #endregion
    }

    class UseCaseManageAssetUpdateHistoryExampleBehaviour
    {
        readonly BaseAssetBehaviour m_Behaviour;

        public bool CanSelectAsset => m_Behaviour.CanSelectAsset;
        public IAsset CurrentAsset => m_Behaviour.CurrentAsset;

        public UseCaseManageAssetUpdateHistoryExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_Behaviour_GetHistory

        public IEnumerable<AssetUpdateHistory> UpdateHistories { get; private set; }
        public int SelectedUpdateHistoryIndex { get; set; } = -1;

        public async Task GetUpdateHistory(bool includeChildren)
        {
            UpdateHistories = null;

            var updateHistories = new List<AssetUpdateHistory>();

            var searchFilter = new AssetUpdateHistorySearchFilter();
            searchFilter.IncludeDatasetsAndFiles.WhereEquals(includeChildren);

            var query = CurrentAsset.QueryUpdateHistory()
                .SelectWhereMatchesFilter(searchFilter)
                .LimitTo(Range.All);
            await foreach (var entry in query.ExecuteAsync(CancellationToken.None))
            {
                updateHistories.Add(entry);
            }

            UpdateHistories = updateHistories;
        }

        #endregion

        #region Example_Behaviour_GetChildHistory

        readonly Dictionary<DatasetId, List<DatasetUpdateHistory>> m_DatasetUpdateHistory = new();
        readonly Dictionary<string, List<FileUpdateHistory>> m_FileUpdateHistory = new();

        public bool TryGetChildUpdateHistory(DatasetUpdateHistoryDescriptor historyDescriptor, out DatasetUpdateHistory datasetUpdateHistory)
        {
            if (m_DatasetUpdateHistory.TryGetValue(historyDescriptor.DatasetDescriptor.DatasetId, out var histories))
            {
                if (histories.Exists(x => x.SequenceNumber == historyDescriptor.SequenceNumber))
                {
                    datasetUpdateHistory = histories.Find(x => x.SequenceNumber == historyDescriptor.SequenceNumber);
                    return true;
                }
            }

            datasetUpdateHistory = default;
            return false;
        }

        public bool TryGetChildUpdateHistory(FileUpdateHistoryDescriptor historyDescriptor, out FileUpdateHistory fileUpdateHistory)
        {
            if (!string.IsNullOrEmpty(historyDescriptor.FilePath) && m_FileUpdateHistory.TryGetValue(historyDescriptor.FilePath, out var histories))
            {
                if (histories.Exists(x => x.SequenceNumber == historyDescriptor.SequenceNumber))
                {
                    fileUpdateHistory = histories.Find(x => x.SequenceNumber == historyDescriptor.SequenceNumber);
                    return true;
                }
            }

            fileUpdateHistory = default;
            return false;
        }

        public async Task LoadChildUpdateHistory(DatasetUpdateHistoryDescriptor historyDescriptor)
        {
            if (!m_DatasetUpdateHistory.TryGetValue(historyDescriptor.DatasetDescriptor.DatasetId, out var histories))
            {
                histories = new List<DatasetUpdateHistory>();
                m_DatasetUpdateHistory[historyDescriptor.DatasetDescriptor.DatasetId] = histories;
            }

            var history = await CurrentAsset.GetUpdateHistoryAsync(historyDescriptor, CancellationToken.None);
            histories.Add(history);
        }

        public async Task LoadChildUpdateHistory(FileUpdateHistoryDescriptor historyDescriptor)
        {
            if (!m_FileUpdateHistory.TryGetValue(historyDescriptor.FilePath, out var histories))
            {
                histories = new List<FileUpdateHistory>();
                m_FileUpdateHistory[historyDescriptor.FilePath] = histories;
            }

            var history = await CurrentAsset.GetUpdateHistoryAsync(historyDescriptor, CancellationToken.None);
            histories.Add(history);
        }

        #endregion

        #region Example_Behaviour_RollbackHistory

        public async Task UpdateAsync(int sequenceNumber)
        {
            await CurrentAsset.UpdateAsync(sequenceNumber, CancellationToken.None);

            Debug.Log($"Asset updated to history {sequenceNumber}");
        }

        public async Task UpdateAsync(DatasetUpdateHistoryDescriptor historyDescriptor)
        {
            var dataset = await CurrentAsset.GetDatasetAsync(historyDescriptor.DatasetId, CancellationToken.None);
            await dataset.UpdateAsync(historyDescriptor.SequenceNumber, CancellationToken.None);

            Debug.Log($"Dataset {historyDescriptor.DatasetId} updated to history {historyDescriptor.SequenceNumber}");
        }

        public async Task UpdateAsync(FileUpdateHistoryDescriptor historyDescriptor)
        {
            var dataset = await CurrentAsset.GetDatasetAsync(historyDescriptor.DatasetId, CancellationToken.None);
            var file = await dataset.GetFileAsync(historyDescriptor.FilePath, CancellationToken.None);
            await file.UpdateAsync(historyDescriptor.SequenceNumber, CancellationToken.None);

            Debug.Log($"File {historyDescriptor.FilePath} updated to history {historyDescriptor.SequenceNumber}");
        }

        #endregion

        #region Example_Behaviour_GetHistory_Dataset

        public async Task<IEnumerable<DatasetUpdateHistory>> GetDatasetUpdateHistory(IDataset dataset)
        {
            var updateHistories = new List<DatasetUpdateHistory>();

            var query = dataset.QueryUpdateHistory()
                .LimitTo(Range.All);
            await foreach (var entry in query.ExecuteAsync(CancellationToken.None))
            {
                updateHistories.Add(entry);
            }

            return updateHistories;
        }

        #endregion

        #region Example_Behaviour_GetHistory_File

        public async Task<IEnumerable<FileUpdateHistory>> GetFileUpdateHistory(IFile file)
        {
            var updateHistories = new List<FileUpdateHistory>();

            var query = file.QueryUpdateHistory()
                .LimitTo(Range.All);
            await foreach (var entry in query.ExecuteAsync(CancellationToken.None))
            {
                updateHistories.Add(entry);
            }

            return updateHistories;
        }

        #endregion
    }
}
