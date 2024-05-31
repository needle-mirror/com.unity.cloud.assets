namespace Unity.Cloud.Documentation.Assets
{
#pragma warning disable S4487 // Unread "private" fields should be removed
#pragma warning disable S1186 // Methods should not be empty

    #region Example_UIClass

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Unity.Cloud.Assets;
    using Unity.Cloud.Common;
    using UnityEngine;

    public class UseCaseFileCreationExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public UseCaseFileCreationExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseFileCreationExample : IAssetManagementUI
    {
        readonly UseCaseFileCreationExampleBehaviour m_Behaviour;

        public UseCaseFileCreationExample(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = new UseCaseFileCreationExampleBehaviour(behaviour);
        }

        #region Example_UIContent

        IAsset m_CurrentAsset;
        Vector2 m_DatasetsScrollPosition;
        Dictionary<DatasetId, bool> m_Expanded = new();

        public void OnGUI()
        {
            if (!m_Behaviour.IsProjectSelected) return;

            if (m_CurrentAsset != m_Behaviour.CurrentAsset)
            {
                m_CurrentAsset = m_Behaviour.CurrentAsset;
                _ = m_Behaviour.GetDatasets();
            }

            if (m_CurrentAsset == null)
            {
                GUILayout.Label(" ! No asset selected !");
                return;
            }

            if (m_Behaviour.Datasets == null)
            {
                GUILayout.Label("Loading datasets...");
                return;
            }

            GUILayout.BeginVertical();

            if (GUILayout.Button("Refresh Datasets", GUILayout.Width(120)))
            {
                _ = m_Behaviour.GetDatasets();
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
                return;
            }

            m_DatasetsScrollPosition = GUILayout.BeginScrollView(m_DatasetsScrollPosition, GUILayout.Height(Screen.height * 0.8f));

            foreach (var dataset in datasets)
            {
                DisplayDataset(dataset);

                GUILayout.Space(10f);
            }

            GUILayout.EndScrollView();
        }

        void DisplayDataset(IDataset dataset)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(dataset.Name);

            GUILayout.Space(5f);

#if UNITY_EDITOR
            if (GUILayout.Button("Upload new file", GUILayout.Width(100)))
            {
                var filePath = UnityEditor.EditorUtility.OpenFilePanel("File to upload", "Assets", string.Empty);
                if (!string.IsNullOrEmpty(filePath))
                    _ = m_Behaviour.UploadFile(dataset, filePath);
            }
#endif

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

                DisplayFiles(dataset);

                GUILayout.EndHorizontal();
            }

            if (m_SelectedFile != null)
            {
                m_WindowRect = GUILayout.Window(0, m_WindowRect, DisplayWindow, "Select files to link");
            }
        }

        void DisplayFiles(IDataset dataset)
        {
            if (!m_Behaviour.DatasetFiles.ContainsKey(dataset.Descriptor.DatasetId))
            {
                _ = m_Behaviour.GetFilesAsync(dataset.Descriptor.DatasetId);
            }

            var files = m_Behaviour.DatasetFiles.GetValueOrDefault(dataset.Descriptor.DatasetId);

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
                DisplayFile(dataset, file);
            }

            GUILayout.EndVertical();
        }

        void DisplayFile(IDataset dataset, IFile file)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label($"{file.Descriptor.Path} ({file.SizeBytes} kb)");

            if (GUILayout.Button("Link to", GUILayout.Width(60)))
            {
                m_WindowRect = new Rect(Screen.width * 0.4f, Screen.height * 0.4f, Screen.width * 0.2f, Screen.height * 0.2f);
                m_SelectedFile = file;
                m_AvailableDatasets = m_Behaviour.Datasets?.Where(f => !m_SelectedFile.LinkedDatasets.Contains(dataset.Descriptor)).ToList();
            }

            if (GUILayout.Button("Unlink", GUILayout.Width(60)))
            {
                _ = m_Behaviour.UnlinkFile(dataset, file);
            }

            GUILayout.EndHorizontal();
        }

        Rect m_WindowRect;
        IFile m_SelectedFile;
        List<IDataset> m_AvailableDatasets;

        void DisplayWindow(int windowId)
        {
            GUILayout.BeginVertical();

            GUILayout.Label($"Link {m_SelectedFile.Descriptor.Path} to:");

            if (m_AvailableDatasets.Count == 0)
            {
                GUILayout.Label(" ! No datasets to link to !");
            }
            else
            {
                for (var i = 0; i < m_AvailableDatasets.Count; ++i)
                {
                    GUILayout.BeginHorizontal();

                    GUILayout.Label(m_AvailableDatasets[i].Name);

                    if (GUILayout.Button("Link", GUILayout.Width(60)))
                    {
                        _ = m_Behaviour.LinkFile(m_AvailableDatasets[i], m_SelectedFile);
                        m_AvailableDatasets.RemoveAt(i);

                        // Force a refresh of the dataset files, including the already linked ones of the selected one
                        foreach (var linkedDatasetId in m_SelectedFile.LinkedDatasets.Select(d => d.DatasetId))
                        {
                            m_Behaviour.DatasetFiles.Remove(linkedDatasetId);
                            m_Expanded.Remove(linkedDatasetId);
                        }
                        m_Behaviour.DatasetFiles.Remove(m_AvailableDatasets[i].Descriptor.DatasetId);
                        m_Expanded.Remove(m_AvailableDatasets[i].Descriptor.DatasetId);

                        GUILayout.EndHorizontal();
                        break;
                    }

                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.Space(10f);

            if (GUILayout.Button("Close", GUILayout.Width(60)))
            {
                m_SelectedFile = null;
                m_AvailableDatasets = null;
            }

            GUILayout.EndVertical();
        }

        #endregion
    }

    class UseCaseFileCreationExampleBehaviour
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public bool IsProjectSelected => m_Behaviour.IsProjectSelected;
        public IAsset CurrentAsset => m_Behaviour.CurrentAsset;

        public UseCaseFileCreationExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_Behaviour_RefreshDatasets

        public List<IDataset> Datasets { get; private set; }

        public async Task GetDatasets()
        {
            Datasets = null;

            if (CurrentAsset == null) return;

            var datasets = new List<IDataset>();
            var asyncList = CurrentAsset.ListDatasetsAsync(Range.All, CancellationToken.None);
            await foreach (var dataset in asyncList)
            {
                datasets.Add(dataset);
            }

            Datasets = datasets;
        }

        #endregion

        #region Example_Behaviour_RefreshFiles

        public Dictionary<DatasetId, IEnumerable<IFile>> DatasetFiles { get; } = new();

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

        #region Example_Behaviour_UploadAssetFile

        class LogProgress : IProgress<HttpProgress>
        {
            public void Report(HttpProgress value)
            {
                if (!value.UploadProgress.HasValue) return;

                Debug.Log($"Upload progress: {value.UploadProgress * 100} %");
            }
        }

        public async Task UploadFile(IDataset dataset, string filePath)
        {
            var fileCreation = new FileCreation(Path.GetFileName(filePath))
            {
                Description = "Documentation example asset file creation.",
                Tags = new List<string> {"Texture", "Gray"}
            };

            try
            {
                var progress = new LogProgress();

                var fileStream = File.OpenRead(filePath);
                var file = await dataset.UploadFileAsync(fileCreation, fileStream, progress, default);

                _ = GetFilesAsync(dataset.Descriptor.DatasetId);

                Debug.Log($"Asset file upload: {file.Descriptor.Path} added and uploaded.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to upload file: {fileCreation.Path}. {e}");
            }
        }

        #endregion

        #region Example_Behaviour_AddFileReference

        public async Task LinkFile(IDataset dataset, IFile file)
        {
            try
            {
                await dataset.AddExistingFileAsync(file.Descriptor.Path, file.Descriptor.DatasetId, CancellationToken.None);
                Debug.Log($"File: {file.Descriptor.Path} linked to dataset {dataset.Descriptor.DatasetId}.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to link asset file: {file.Descriptor.Path}. {e}");
            }
        }

        #endregion

        #region Example_Behaviour_RemoveFileReference

        public async Task UnlinkFile(IDataset dataset, IFile file)
        {
            try
            {
                await dataset.RemoveFileAsync(file.Descriptor.Path, CancellationToken.None);
                Debug.Log($"File: {file.Descriptor.Path} unlinked from dataset {dataset.Descriptor.DatasetId}.");

                _ = GetFilesAsync(dataset.Descriptor.DatasetId);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to unlink asset file: {file.Descriptor.Path}. {e}");
            }
        }

        #endregion
    }
}
