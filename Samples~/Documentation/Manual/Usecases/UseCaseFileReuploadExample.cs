using System.Linq;
using UnityEditor;

namespace Unity.Cloud.Documentation.Assets
{
#pragma warning disable S4487 // Unread "private" fields should be removed
#pragma warning disable S1186 // Methods should not be empty

    #region Example_UIClass

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Unity.Cloud.Assets;
    using Unity.Cloud.Common;
    using UnityEngine;

    public class UseCaseFileReuploadExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public UseCaseFileReuploadExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseFileReuploadExample : IAssetManagementUI
    {
        readonly UseCaseFileReuploadExampleBehaviour m_Behaviour;

        public UseCaseFileReuploadExample(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = new UseCaseFileReuploadExampleBehaviour(behaviour);
        }

        #region Example_UIContent

        IAsset m_CurrentAsset;

        Dictionary<DatasetId, bool> m_Expanded = new();

        public void OnGUI()
        {
            if (!m_Behaviour.IsProjectSelected) return;

            if (m_Behaviour.CurrentAsset == null)
            {
                GUILayout.Label(" ! No asset selected !");
                return;
            }

            if (m_CurrentAsset != m_Behaviour.CurrentAsset)
            {
                m_CurrentAsset = m_Behaviour.CurrentAsset;
                m_Behaviour.DatasetFiles.Clear();
                _ = m_Behaviour.GetDataSetsAsync();
            }

            if (m_Behaviour.Datasets == null)
            {
                GUILayout.Label("Loading datasets...");
                return;
            }

            GUILayout.BeginVertical();

            if (GUILayout.Button("Refresh Datasets"))
            {
                _ = m_Behaviour.GetDataSetsAsync();
            }

            GUILayout.Space(5f);

            DisplayDatasets(m_Behaviour.Datasets.ToArray());

            GUILayout.EndVertical();
        }

        void DisplayDatasets(IReadOnlyCollection<IDataset> datasets)
        {
            if (datasets.Count == 0)
            {
                GUILayout.Label("No datasets.");
            }

            foreach (var dataset in datasets)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label($"{dataset.Name}");

                var expanded = m_Expanded.GetValueOrDefault(dataset.Descriptor.DatasetId);
                if (GUILayout.Button(expanded ? "-" : "+", GUILayout.Width(20f)))
                {
                    expanded = !expanded;
                    m_Expanded[dataset.Descriptor.DatasetId] = expanded;

                    if (!expanded)
                    {
                        m_Behaviour.DatasetFiles.Remove(dataset.Descriptor.DatasetId);
                    }
                }

                GUILayout.EndHorizontal();

                if (expanded)
                {
                    GUILayout.BeginHorizontal();

                    GUILayout.Space(25);

                    DisplayFiles(dataset.Descriptor.DatasetId);

                    GUILayout.EndHorizontal();
                }
            }
        }

        void DisplayFiles(DatasetId datasetId)
        {
            if (!m_Behaviour.DatasetFiles.ContainsKey(datasetId))
            {
                _ = m_Behaviour.GetFilesAsync(datasetId);
            }

            var files = m_Behaviour.DatasetFiles.GetValueOrDefault(datasetId);

            if (files == null)
            {
                GUILayout.Label("Loading files...");
                return;
            }

            var enumerable = files.ToArray();
            if (!enumerable.Any())
            {
                GUILayout.Label("No files.");
                return;
            }

            GUILayout.BeginVertical();

            foreach (var file in enumerable)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label($"{file.Descriptor.Path}");

                GUILayout.Space(5f);

                if (GUILayout.Button("Upload new content", GUILayout.Width(150)))
                {
                    var path = EditorUtility.OpenFilePanel("Choose a file to upload.", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "");
                    if (!string.IsNullOrEmpty(path))
                    {
                        var memoryStream = new MemoryStream(File.ReadAllBytes(path));
                        _ = m_Behaviour.ReplaceFileAsync(file, memoryStream);
                    }
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }

        #endregion
    }

    class UseCaseFileReuploadExampleBehaviour
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public bool IsProjectSelected => m_Behaviour.IsProjectSelected;
        public IAsset CurrentAsset => m_Behaviour.CurrentAsset;

        public UseCaseFileReuploadExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_Behaviour_RefreshFiles

        public IEnumerable<IDataset> Datasets { get; private set; }
        public Dictionary<DatasetId, IEnumerable<IFile>> DatasetFiles { get; } = new();

        public async Task GetDataSetsAsync()
        {
            Datasets = null;

            if (CurrentAsset == null) return;

            var datasets = new List<IDataset>();
            var datasetList = CurrentAsset.ListDatasetsAsync(Range.All, CancellationToken.None);
            await foreach (var dataset in datasetList)
            {
                datasets.Add(dataset);
            }

            Datasets = datasets;
        }

        public async Task GetFilesAsync(DatasetId datasetId)
        {
            DatasetFiles.Remove(datasetId);

            var dataset = Datasets?.FirstOrDefault(d => d.Descriptor.DatasetId == datasetId);
            if (dataset == null) return;

            DatasetFiles[datasetId] = null;

            var files = new List<IFile>();
            var fileList = dataset.ListFilesAsync(Range.All, CancellationToken.None);
            await foreach (var file in fileList)
            {
                files.Add(file);
            }

            DatasetFiles[datasetId] = files;
        }

        #endregion

        #region Example_Behaviour_UploadFile

        CancellationTokenSource m_CancellationTokenSource;

        public bool CanCancel => m_CancellationTokenSource is {IsCancellationRequested: false};

        class LogProgress : IProgress<HttpProgress>
        {
            public void Report(HttpProgress value)
            {
                if (!value.UploadProgress.HasValue) return;

                Debug.Log($"Upload progress: {value.UploadProgress * 100} %");
            }
        }

        public async Task ReplaceFileAsync(IFile file, MemoryStream memoryStream)
        {
            await file.UploadAsync(memoryStream, new LogProgress(), GetCancellationToken());
        }

        public void Cancel()
        {
            if (m_CancellationTokenSource != null)
            {
                m_CancellationTokenSource.Cancel();
                m_CancellationTokenSource.Dispose();
            }

            m_CancellationTokenSource = null;
        }

        CancellationToken GetCancellationToken()
        {
            Cancel();

            m_CancellationTokenSource = new CancellationTokenSource();
            return m_CancellationTokenSource.Token;
        }

        #endregion
    }
}
