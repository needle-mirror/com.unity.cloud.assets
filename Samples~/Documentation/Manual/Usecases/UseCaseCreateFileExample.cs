using LogProgress = Unity.Cloud.Documentation.Assets.BaseAssetBehaviour.LogProgress;

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

    public class UseCaseCreateFileExampleUI : IAssetManagementUI
    {
        readonly BaseAssetBehaviour m_Behaviour;

        public UseCaseCreateFileExampleUI(BaseAssetBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseCreateFileExample : IAssetManagementUI
    {
        readonly UseCaseCreateFileExampleBehaviour m_Behaviour;

        public UseCaseCreateFileExample(BaseAssetBehaviour behaviour)
        {
            m_Behaviour = new UseCaseCreateFileExampleBehaviour(behaviour);
        }

        #region Example_UIContent

        static readonly GUILayoutOption k_DefaultButtonSize = GUILayout.Width(60);

        IAsset m_CurrentAsset;
        Vector2 m_DatasetsScrollPosition;
        Dictionary<DatasetId, bool> m_Expanded = new();

        static readonly string[] m_OverrideActions = {"None", "Replace", "Reupload"};
        int m_OverrideFileActionIndex;

        public void OnGUI()
        {
            if (m_Behaviour.CurrentAsset == null) return;

            if (m_CurrentAsset != m_Behaviour.CurrentAsset)
            {
                m_CurrentAsset = m_Behaviour.CurrentAsset;
                _ = m_Behaviour.GetDatasets();
            }

            GUILayout.BeginVertical();

            if (GUILayout.Button("Refresh", k_DefaultButtonSize))
            {
                _ = m_Behaviour.GetDatasets();
            }

            GUILayout.Space(5);

            if (m_Behaviour.Datasets == null)
            {
                GUILayout.Label("Loading datasets...");
                GUILayout.EndVertical();
                return;
            }

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
            m_OverrideFileActionIndex = GUILayout.SelectionGrid(m_OverrideFileActionIndex, m_OverrideActions, 3, GUILayout.Width(240));

            GUILayout.Space(10);
#endif

            m_DatasetsScrollPosition = GUILayout.BeginScrollView(m_DatasetsScrollPosition, GUILayout.ExpandHeight(true));

            foreach (var dataset in datasets)
            {
                DisplayDataset(dataset.Descriptor.DatasetId);

                GUILayout.Space(10);
            }

            GUILayout.EndScrollView();
        }

        void DisplayDataset(DatasetId datasetId)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(m_Behaviour.GetDatasetName(datasetId));

            GUILayout.Space(5);

            TryUploadFolder(datasetId);
            TryUploadFile(datasetId);

            var expanded = m_Expanded.GetValueOrDefault(datasetId);
            if (GUILayout.Button(expanded ? "-" : "+", GUILayout.Width(20)))
            {
                expanded = !expanded;
                m_Expanded[datasetId] = expanded;

                if (!expanded)
                {
                    m_Behaviour.DatasetFiles.Remove(datasetId);
                }
            }

            GUILayout.EndHorizontal();

            if (expanded)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Space(25);

                DisplayFiles(datasetId);

                GUILayout.EndHorizontal();
            }

            if (!string.IsNullOrEmpty(m_SelectedFilePath))
            {
                m_WindowRect = GUILayout.Window(0, m_WindowRect, DisplayWindow, "Select files to link");
            }
        }

        void TryUploadFolder(DatasetId datasetId)
        {
#if UNITY_EDITOR
            if (GUILayout.Button("Upload folder", GUILayout.Width(90)))
            {
                var folderPath = UnityEditor.EditorUtility.OpenFolderPanel("Folder to upload", "Assets", string.Empty);
                if (!string.IsNullOrEmpty(folderPath))
                {
                    _ = m_Behaviour.UploadFolderAsync(datasetId, folderPath, m_OverrideActions[m_OverrideFileActionIndex]);
                }
            }
#endif
        }

        void TryUploadFile(DatasetId datasetId)
        {
#if UNITY_EDITOR
            if (GUILayout.Button("Upload file", GUILayout.Width(90)))
            {
                var filePath = UnityEditor.EditorUtility.OpenFilePanel("File to upload", "Assets", string.Empty);
                var folderPath = filePath[..filePath.LastIndexOf(Path.DirectorySeparatorChar)];
                if (!string.IsNullOrEmpty(filePath))
                {
                    _ = m_OverrideFileActionIndex switch
                    {
                        1 => m_Behaviour.ReplaceFile(datasetId, filePath, folderPath),
                        2 => m_Behaviour.ReuploadFile(datasetId, filePath, folderPath),
                        _ => m_Behaviour.UploadFile(datasetId, filePath, folderPath)
                    };
                }
            }
#endif
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

            var enumerable = files.ToList();
            if (!enumerable.Any())
            {
                GUILayout.Label("No files.");
                return;
            }

            GUILayout.BeginVertical();

            enumerable.Sort((a, b) => string.Compare(a.Descriptor.Path, b.Descriptor.Path, StringComparison.Ordinal));
            foreach (var file in enumerable)
            {
                DisplayFile(datasetId, file.Descriptor.Path);
            }

            GUILayout.EndVertical();
        }

        void DisplayFile(DatasetId datasetId, string filePath)
        {
            if (!m_Behaviour.FileProperties.TryGetValue(filePath, out var fileProperties))
            {
                GUILayout.Label($"Loading properties for {filePath}...");
                return;
            }

            GUILayout.BeginHorizontal();

            var size = fileProperties.SizeBytes < 1000 ? "<1" : MathF.Round(fileProperties.SizeBytes / 1000f).ToString("F0");
            GUILayout.Label($"{filePath} ({size} KB)");

            if (GUILayout.Button("Link to", k_DefaultButtonSize))
            {
                m_WindowRect = new Rect(Screen.width * 0.4f, Screen.height * 0.4f, Screen.width * 0.2f, Screen.height * 0.2f);
                m_SelectedDatasetId = datasetId;
                m_SelectedFilePath = filePath;
                m_SelectedFileProperties = fileProperties;

                var alreadyLinkedDatasetIds = m_SelectedFileProperties.LinkedDatasets.Select(d => d.DatasetId).ToHashSet();
                m_AvailableDatasets = m_Behaviour.Datasets
                    .Select(d => d.Descriptor.DatasetId)
                    .Where(id => !alreadyLinkedDatasetIds.Contains(id))
                    .ToList();
            }

            if (GUILayout.Button("Unlink", k_DefaultButtonSize))
            {
                _ = m_Behaviour.UnlinkFile(datasetId, filePath);
            }

            GUILayout.EndHorizontal();
        }

        Rect m_WindowRect;
        DatasetId m_SelectedDatasetId;
        string m_SelectedFilePath;
        FileProperties m_SelectedFileProperties;
        List<DatasetId> m_AvailableDatasets;

        void DisplayWindow(int windowId)
        {
            GUILayout.BeginVertical();

            GUILayout.Label($"Link {m_SelectedFilePath} to:");

            if (m_AvailableDatasets.Count == 0)
            {
                GUILayout.Label(" ! No datasets to link to !");
            }
            else
            {
                for (var i = 0; i < m_AvailableDatasets.Count; ++i)
                {
                    GUILayout.BeginHorizontal();

                    GUILayout.Label(m_Behaviour.GetDatasetName(m_AvailableDatasets[i]));

                    if (GUILayout.Button("Link", k_DefaultButtonSize))
                    {
                        _ = m_Behaviour.LinkFile(m_AvailableDatasets[i], m_SelectedDatasetId, m_SelectedFilePath);
                        m_AvailableDatasets.RemoveAt(i);

                        // Force a refresh of the dataset files, including the already linked ones of the selected one
                        foreach (var linkedDataset in m_SelectedFileProperties.LinkedDatasets)
                        {
                            m_Behaviour.DatasetFiles.Remove(linkedDataset.DatasetId);
                            m_Expanded.Remove(linkedDataset.DatasetId);
                        }

                        m_Behaviour.DatasetFiles.Remove(m_AvailableDatasets[i]);
                        m_Expanded.Remove(m_AvailableDatasets[i]);

                        GUILayout.EndHorizontal();
                        break;
                    }

                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Close", k_DefaultButtonSize))
            {
                m_SelectedFilePath = null;
                m_SelectedFileProperties = default;
                m_AvailableDatasets = null;
            }

            GUILayout.EndVertical();
        }

        #endregion
    }

    class UseCaseCreateFileExampleBehaviour : UseCaseCreateDatasetExampleBehaviour
    {
        public UseCaseCreateFileExampleBehaviour(BaseAssetBehaviour behaviour)
            : base(behaviour) { }

        #region Example_Behaviour_RefreshFiles

        public Dictionary<DatasetId, List<IFile>> DatasetFiles { get; } = new();
        public Dictionary<string, FileProperties> FileProperties { get; } = new();
        public string CurrentFilePath { get; set; }

        CancellationTokenSource m_FileCancellationSource;

        public async Task GetFilesAsync(DatasetId datasetId)
        {
            var filePath = CurrentFilePath;
            CurrentFilePath = null;

            var token = GetFileCancellationToken();

            var dataset = Datasets.FirstOrDefault(d => d.Descriptor.DatasetId == datasetId);
            if (dataset == null) return;

            if (DatasetFiles.TryGetValue(dataset.Descriptor.DatasetId, out var files))
            {
                files.Clear();
            }
            else
            {
                files = new List<IFile>();
                DatasetFiles[dataset.Descriptor.DatasetId] = files;
            }

            var fileList = dataset.ListFilesAsync(Range.All, token);
            await foreach (var file in fileList)
            {
                files.Add(file);

                if (filePath == file.Descriptor.Path)
                {
                    CurrentFilePath = file.Descriptor.Path;
                }

                var properties = await file.GetPropertiesAsync(token);
                FileProperties[file.Descriptor.Path] = properties;
            }
        }

        CancellationToken GetFileCancellationToken()
        {
            if (m_FileCancellationSource != null)
            {
                m_FileCancellationSource.Cancel();
                m_FileCancellationSource.Dispose();
            }

            m_FileCancellationSource = new CancellationTokenSource();
            return m_FileCancellationSource.Token;
        }

        #endregion

        #region Example_Behaviour_UploadAssetFile

        static string GetRelativePath(string folderPath, string filePath)
        {
            return string.IsNullOrEmpty(folderPath) ? filePath : Path.GetRelativePath(folderPath, filePath);
        }

        public async Task UploadFile(DatasetId datasetId, string filePath, string folderPath = "", bool refreshFiles = true)
        {
            var fileCreation = new FileCreation(GetRelativePath(folderPath, filePath))
            {
                Description = "Documentation example file creation.",
            };

            try
            {
                var progress = new LogProgress(filePath);

                var fileStream = File.OpenRead(filePath);
                var dataset = await CurrentAsset.GetDatasetAsync(datasetId, CancellationToken.None);
                var fileDescriptor = await dataset.UploadFileLiteAsync(fileCreation, fileStream, progress, CancellationToken.None);

                if (refreshFiles)
                {
                    _ = GetFilesAsync(datasetId);
                }

                Debug.Log($"File upload: {fileDescriptor.Path} added and uploaded.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to upload file: {fileCreation.Path}. {e}");
            }
        }

        public async Task ReplaceFile(DatasetId datasetId, string filePath, string folderPath = "", bool refreshFiles = true)
        {
            var path = GetRelativePath(folderPath, filePath);

            try
            {
                var dataset = await CurrentAsset.GetDatasetAsync(datasetId, CancellationToken.None);
                await dataset.RemoveFileAsync(path, CancellationToken.None);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to remove file for replace: {path}. {e}");
                return;
            }

            await UploadFile(datasetId, filePath, folderPath, refreshFiles);
        }

        public async Task ReuploadFile(DatasetId datasetId, string filePath, string folderPath = "", bool refreshFiles = true)
        {
            var path = GetRelativePath(folderPath, filePath);

            IFile file = null;

            try
            {
                var datasetDescriptor = new DatasetDescriptor(CurrentAsset.Descriptor, datasetId);
                var fileDescriptor = new FileDescriptor(datasetDescriptor, path);
                file = PlatformServices.AssetRepository.GetFileAsync(fileDescriptor, CancellationToken.None).Result;
            }
            catch (NotFoundException)
            {
                Debug.LogError($"File not found: {path}.");
            }

            if (file == null)
            {
                await UploadFile(datasetId, filePath, folderPath, refreshFiles);
                return;
            }

            try
            {
                var fileStream = File.OpenRead(filePath);
                await file.UploadAsync(fileStream, new LogProgress(path), CancellationToken.None);

                if (refreshFiles)
                {
                    _ = GetFilesAsync(datasetId);
                }

                Debug.Log($"Re-uploaded file: {path}.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to re-upload file: {path}. {e}");
            }
        }

        #endregion

        #region Example_Behaviour_AddFileReference

        public async Task LinkFile(DatasetId datasetId, DatasetId sourceDatasetId, string filePath)
        {
            try
            {
                var dataset = await CurrentAsset.GetDatasetAsync(datasetId, CancellationToken.None);
                await dataset.AddExistingFileAsync(filePath, sourceDatasetId, CancellationToken.None);
                Debug.Log($"File: {filePath} linked to dataset {datasetId}.");

                // If the dataset files are already loaded, refresh them
                if (DatasetFiles.ContainsKey(datasetId))
                {
                    _ = GetFilesAsync(datasetId);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to link file: {filePath}. {e}");
            }
        }

        #endregion

        #region Example_Behaviour_RemoveFileReference

        public async Task UnlinkFile(DatasetId datasetId, string filePath)
        {
            try
            {
                var dataset = await CurrentAsset.GetDatasetAsync(datasetId, CancellationToken.None);
                await dataset.RemoveFileAsync(filePath, CancellationToken.None);
                Debug.Log($"File: {filePath} unlinked from dataset {datasetId}.");

                // If the dataset files are already loaded, refresh them
                if (DatasetFiles.ContainsKey(datasetId))
                {
                    _ = GetFilesAsync(datasetId);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to unlink file: {filePath}. {e}");
            }
        }

        #endregion

        #region Example_Behaviour_UploadFolder

        public async Task UploadFolderAsync(DatasetId datasetId, string folderPath, string uploadType)
        {
            var parentDirectoryPath = Directory.GetParent(folderPath)?.FullName;
            var files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);

            var tasks = new List<Task>();
            foreach (var file in files)
            {
                switch (uploadType)
                {
                    case "Replace":
                        tasks.Add(ReplaceFile(datasetId, file, parentDirectoryPath, false));
                        break;
                    case "Reupload":
                        tasks.Add(ReuploadFile(datasetId, file, parentDirectoryPath, false));
                        break;
                    default:
                        tasks.Add(UploadFile(datasetId, file, parentDirectoryPath, false));
                        break;
                }
            }

            try
            {
                await Task.WhenAll(tasks);

                _ = GetFilesAsync(datasetId);

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
