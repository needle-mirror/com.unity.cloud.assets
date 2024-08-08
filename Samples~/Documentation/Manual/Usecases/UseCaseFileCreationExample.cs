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

        static readonly GUILayoutOption k_DefaultButtonSize = GUILayout.Width(60);

        IAsset m_CurrentAsset;
        Vector2 m_DatasetsScrollPosition;
        Dictionary<DatasetId, bool> m_Expanded = new();

        int m_OverrideFileActionIndex;

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

            if (GUILayout.Button("Refresh", k_DefaultButtonSize))
            {
                _ = m_Behaviour.GetDatasets();
            }

            GUILayout.Space(5);

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

#if UNITY_EDITOR
            GUILayout.Label("File upload options:");
            m_OverrideFileActionIndex = GUILayout.SelectionGrid(m_OverrideFileActionIndex, new[] {"None", "Replace", "Reupload"}, 3, GUILayout.Width(240));

            GUILayout.Space(10);
#endif

            m_DatasetsScrollPosition = GUILayout.BeginScrollView(m_DatasetsScrollPosition, GUILayout.ExpandHeight(true));

            foreach (var dataset in datasets)
            {
                DisplayDataset(dataset);

                GUILayout.Space(10);
            }

            GUILayout.EndScrollView();
        }

        void DisplayDataset(IDataset dataset)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(dataset.Name);

            GUILayout.Space(5);

            TryUploadFolder(dataset);
            TryUploadFile(dataset);

            var expanded = m_Expanded.GetValueOrDefault(dataset.Descriptor.DatasetId);
            if (GUILayout.Button(expanded ? "-" : "+", GUILayout.Width(20)))
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

        void TryUploadFolder(IDataset dataset)
        {
#if UNITY_EDITOR
            if (GUILayout.Button("Upload folder", GUILayout.Width(90)))
            {
                var folderPath = UnityEditor.EditorUtility.OpenFolderPanel("Folder to upload", "Assets", string.Empty);
                if (!string.IsNullOrEmpty(folderPath))
                {
                    _ = m_Behaviour.UploadFolderAsync(dataset, folderPath);
                }
            }
#endif
        }

        void TryUploadFile(IDataset dataset)
        {
#if UNITY_EDITOR
            if (GUILayout.Button("Upload file", GUILayout.Width(90)))
            {
                var filePath = UnityEditor.EditorUtility.OpenFilePanel("File to upload", "Assets", string.Empty);
                if (!string.IsNullOrEmpty(filePath))
                {
                    _ = m_OverrideFileActionIndex switch
                    {
                        1 => m_Behaviour.ReplaceFile(dataset, filePath),
                        2 => m_Behaviour.ReuploadFile(dataset, filePath),
                        _ => m_Behaviour.UploadFile(dataset, filePath)
                    };
                }
            }
#endif
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

            var size = file.SizeBytes < 1000 ? "<1" : MathF.Round(file.SizeBytes / 1000f).ToString("F0");
            GUILayout.Label($"{file.Descriptor.Path} ({size} KB)");

            if (GUILayout.Button("Link to", k_DefaultButtonSize))
            {
                m_WindowRect = new Rect(Screen.width * 0.4f, Screen.height * 0.4f, Screen.width * 0.2f, Screen.height * 0.2f);
                m_SelectedFile = file;
                m_AvailableDatasets = m_Behaviour.Datasets?.Where(f => !m_SelectedFile.LinkedDatasets.Contains(dataset.Descriptor)).ToList();
            }

            if (GUILayout.Button("Unlink", k_DefaultButtonSize))
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

                    if (GUILayout.Button("Link", k_DefaultButtonSize))
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

            GUILayout.Space(10);

            if (GUILayout.Button("Close", k_DefaultButtonSize))
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
            string m_Name;

            public LogProgress(string name)
            {
                m_Name = name;
            }

            public void Report(HttpProgress value)
            {
                if (!value.UploadProgress.HasValue) return;

                Debug.Log($"Upload progress for {m_Name}: {value.UploadProgress * 100} %");
            }
        }

        public async Task UploadFile(IDataset dataset, string filePath, string folderPath = "")
        {
            var fileCreation = new FileCreation(Path.GetRelativePath(folderPath, filePath))
            {
                Description = "Documentation example file creation.",
            };

            try
            {
                var progress = new LogProgress(filePath);

                var fileStream = File.OpenRead(filePath);
                var file = await dataset.UploadFileAsync(fileCreation, fileStream, progress, default);

                _ = GetFilesAsync(dataset.Descriptor.DatasetId);

                Debug.Log($"File upload: {file.Descriptor.Path} added and uploaded.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to upload file: {fileCreation.Path}. {e}");
            }
        }

        public async Task ReplaceFile(IDataset dataset, string filePath, string folderPath = "")
        {
            var path = Path.GetRelativePath(folderPath, filePath);

            if (DatasetFiles.TryGetValue(dataset.Descriptor.DatasetId, out var files))
            {
                var file = files.FirstOrDefault(f => f.Descriptor.Path == Path.GetFileName(path));
                if (file != null)
                {
                    try
                    {
                        await UnlinkFile(dataset, file);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to remove file for replace: {file.Descriptor.Path}. {e}");
                        return;
                    }
                }
            }

            await UploadFile(dataset, filePath, folderPath);
        }

        public async Task ReuploadFile(IDataset dataset, string filePath, string folderPath = "")
        {
            var path = Path.GetRelativePath(folderPath, filePath);

            if (DatasetFiles.TryGetValue(dataset.Descriptor.DatasetId, out var files))
            {
                var file = files.FirstOrDefault(f => f.Descriptor.Path == Path.GetFileName(path));
                if (file != null)
                {
                    try
                    {
                        var logProgress = new LogProgress(path);

                        var fileStream = File.OpenRead(filePath);
                        await file.UploadAsync(fileStream, logProgress, default);

                        _ = GetFilesAsync(dataset.Descriptor.DatasetId);

                        Debug.Log($"Re-uplaoded file: {file.Descriptor.Path}.");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to re-upload file: {file.Descriptor.Path}. {e}");
                    }

                    return;
                }
            }

            await UploadFile(dataset, filePath, folderPath);
        }

        #endregion

        #region Example_Behaviour_AddFileReference

        public async Task LinkFile(IDataset dataset, IFile file)
        {
            try
            {
                await dataset.AddExistingFileAsync(file.Descriptor.Path, file.Descriptor.DatasetId, CancellationToken.None);
                Debug.Log($"File: {file.Descriptor.Path} linked to dataset {dataset.Descriptor.DatasetId}.");

                // If the dataset files are already loaded, refresh them
                if (DatasetFiles.ContainsKey(dataset.Descriptor.DatasetId))
                {
                    _ = GetFilesAsync(dataset.Descriptor.DatasetId);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to link file: {file.Descriptor.Path}. {e}");
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

                // If the dataset files are already loaded, refresh them
                if (DatasetFiles.ContainsKey(dataset.Descriptor.DatasetId))
                {
                    _ = GetFilesAsync(dataset.Descriptor.DatasetId);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to unlink file: {file.Descriptor.Path}. {e}");
            }
        }

        #endregion

        #region Example_Behaviour_UploadFolder

        public async Task UploadFolderAsync(IDataset dataset, string folderPath)
        {
            var parentDirectoryPath = Directory.GetParent(folderPath)?.FullName;
            var files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);

            var tasks = new List<Task>();
            foreach (var file in files)
            {
                tasks.Add(ReuploadFile(dataset, file, parentDirectoryPath));
            }

            try
            {
                await Task.WhenAll(tasks);
                Debug.Log($"Folder: {folderPath} uploaded.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to upload folder: {folderPath}. {e}");
            }
        }

        #endregion
    }
}
