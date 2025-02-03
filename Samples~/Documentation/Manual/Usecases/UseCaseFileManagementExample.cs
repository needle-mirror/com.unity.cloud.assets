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

    public class UseCaseFileManagementExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public UseCaseFileManagementExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseFileManagementExample : IAssetManagementUI
    {
        readonly UseCaseFileManagementExampleBehaviour m_Behaviour;

        public UseCaseFileManagementExample(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = new UseCaseFileManagementExampleBehaviour(behaviour);
        }

        #region Example_UIContent

        IAsset m_CurrentAsset;
        Vector2 m_DatasetsScrollPosition;
        Vector2 m_FilesScrollPosition;

        FileUpdate m_FileUpdate;
        string m_TagsString = string.Empty;

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
                m_FileUpdate = null;
                m_Behaviour.CancelTagGeneration();
                _ = m_Behaviour.GetDatasetsAsync();
            }

            GUILayout.BeginVertical();

            DisplayDatasetSelection(m_Behaviour.Datasets.ToArray());

            GUILayout.EndVertical();

            if (m_Behaviour.CurrentDatasetId == null)
            {
                GUILayout.Label("! No dataset selected !");
                return;
            }

            GUILayout.BeginVertical();

            DisplayFileSelection(m_Behaviour.FileProperties.Keys.ToArray());

            GUILayout.EndVertical();

            if (m_Behaviour.CurrentFilePath == null)
            {
                GUILayout.Label("! No file selected !");
                return;
            }

            GUILayout.BeginVertical();

            DisplaySelectedFile();

            GUILayout.EndVertical();
        }

        void DisplayDatasetSelection(IReadOnlyCollection<IDataset> datasets)
        {
            if (GUILayout.Button("Refresh", GUILayout.Width(60)))
            {
                _ = m_Behaviour.GetDatasetsAsync();
                return;
            }

            GUILayout.Space(5);

            if (datasets.Count == 0)
            {
                GUILayout.Label("No datasets.");
                return;
            }

            m_DatasetsScrollPosition = GUILayout.BeginScrollView(m_DatasetsScrollPosition, GUILayout.ExpandHeight(true), GUILayout.Width(Screen.width * 0.15f));

            foreach (var dataset in datasets)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label(m_Behaviour.GetDatasetName(dataset.Descriptor.DatasetId));

                GUI.enabled = dataset.Descriptor.DatasetId != m_Behaviour.CurrentDatasetId;

                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    m_FileUpdate = null;
                    m_Behaviour.CancelTagGeneration();
                    _ = m_Behaviour.SetSelectedDatasetAsync(dataset.Descriptor.DatasetId);
                }

                GUI.enabled = true;

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        void DisplayFileSelection(IReadOnlyCollection<string> filePaths)
        {
            if (GUILayout.Button("Refresh", GUILayout.Width(60)))
            {
                m_FileUpdate = null;
                m_Behaviour.CancelTagGeneration();
                _ = m_Behaviour.GetFilesAsync();
                return;
            }

            GUILayout.Space(5);

            if (filePaths.Count == 0)
            {
                GUILayout.Label("No files.");
                return;
            }

            m_FilesScrollPosition = GUILayout.BeginScrollView(m_FilesScrollPosition, GUILayout.MaxHeight(Screen.height * 0.8f), GUILayout.Width(Screen.width * 0.2f));

            DisplayFiles(filePaths);

            GUILayout.EndScrollView();
        }

        void DisplayFiles(IReadOnlyCollection<string> filePaths)
        {
            // Get a local copy of the list of asset files to avoid concurrent modification exceptions.
            foreach (var filePath in filePaths)
            {
                if (!m_Behaviour.FileProperties.TryGetValue(filePath, out var fileProperties))
                {
                    GUILayout.Label(filePath);
                    continue;
                }

                GUILayout.BeginHorizontal();

                GUILayout.Label($"{filePath}");

                GUI.enabled = filePath != m_Behaviour.CurrentFilePath;

                if (GUILayout.Button("Select", GUILayout.Width(70)))
                {
                    m_Behaviour.CurrentFilePath = filePath;
                    m_FileUpdate = new FileUpdate
                    {
                        Description = fileProperties.Description,
                        Tags = fileProperties.Tags?.ToArray() ?? Array.Empty<string>()
                    };
                    m_TagsString = string.Join(',', m_FileUpdate.Tags);
                    m_Behaviour.CancelTagGeneration();
                }

                GUI.enabled = true;

                if (GUILayout.Button("Download", GUILayout.Width(70)))
                {
                    _ = m_Behaviour.DownloadFileAsync(filePath);
                }

                GUILayout.EndHorizontal();
            }
        }

        void DisplaySelectedFile()
        {
            if (!m_Behaviour.FileProperties.TryGetValue(m_Behaviour.CurrentFilePath, out var properties))
            {
                GUILayout.Label("! File properties not loaded !");
                return;
            }

            GUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label($"{m_Behaviour.CurrentFilePath}");

            GUILayout.Label($"Status: {properties.StatusName}");

            var createdDate = properties.AuthoringInfo?.Created.ToString("d") ?? "unknown";
            GUILayout.Label($"Created on: {createdDate}");

            var modifiedDate = properties.AuthoringInfo?.Updated.ToString("d") ?? "unknown";
            GUILayout.Label($"Last modified on: {modifiedDate}");

            GUILayout.Label($"Size: {properties.SizeBytes} bytes");

            GUILayout.EndVertical();

            GUILayout.Space(5);

            GUILayout.Label("Description:");
            m_FileUpdate.Description = GUILayout.TextField(m_FileUpdate.Description);

            GUILayout.Label("Tags:");
            m_TagsString = GUILayout.TextField(m_TagsString, GUILayout.ExpandWidth(true));
            m_FileUpdate.Tags = m_TagsString.Split(',')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            if (GUILayout.Button("Generate Tags"))
            {
                _ = m_Behaviour.GenerateTagsAsync();
            }

            DisplayGeneratedTags();

            GUILayout.Space(5);

            if (GUILayout.Button("Update"))
            {
                _ = m_Behaviour.UpdateFileAsync(m_FileUpdate);
            }
        }

        void DisplayGeneratedTags()
        {
            if (m_Behaviour.GeneratedTags != null)
            {
                foreach (var tag in m_Behaviour.GeneratedTags)
                {
                    GUILayout.BeginHorizontal();

                    GUI.enabled = !m_FileUpdate.Tags?.Contains(tag.Value) ?? true;

                    if (GUILayout.Button("Add", GUILayout.Width(40)))
                    {
                        if (string.IsNullOrWhiteSpace(m_TagsString))
                        {
                            m_TagsString = tag.Value;
                        }
                        else
                        {
                            m_TagsString += $", {tag.Value}";
                        }
                    }

                    GUILayout.Label($"{tag.Value}, Confidence: {tag.Confidence:F3}");

                    GUI.enabled = true;

                    GUILayout.EndHorizontal();
                }
            }
        }

        #endregion
    }

    class UseCaseFileManagementExampleBehaviour
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public bool IsProjectSelected => m_Behaviour.IsProjectSelected;
        public IAsset CurrentAsset => m_Behaviour.CurrentAsset;

        public UseCaseFileManagementExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_Behaviour_RefreshFiles

        CancellationTokenSource m_DatasetCancellationSource;
        CancellationTokenSource m_FileCancellationSource;

        public List<IDataset> Datasets { get; } = new();
        public DatasetId? CurrentDatasetId { get; private set; }
        Dictionary<DatasetId, string> DatasetNames { get; } = new();

        public Dictionary<string, FileProperties> FileProperties { get; } = new();
        public string CurrentFilePath { get; set; }

        public async Task GetDatasetsAsync()
        {
            var datasetId = CurrentDatasetId;
            CurrentDatasetId = null;

            CleanFileCancellation();
            FileProperties.Clear();

            CleanDatasetCancellation();
            Datasets.Clear();
            DatasetNames.Clear();

            if (CurrentAsset == null) return;

            m_DatasetCancellationSource = new CancellationTokenSource();
            var token = m_DatasetCancellationSource.Token;

            var datasetList = CurrentAsset.ListDatasetsAsync(Range.All, token);
            await foreach (var dataset in datasetList)
            {
                Datasets.Add(dataset);

                if (datasetId == dataset.Descriptor.DatasetId)
                {
                    CurrentDatasetId = dataset.Descriptor.DatasetId;
                }

                var properties = await dataset.GetPropertiesAsync(token);

                if (token.IsCancellationRequested) break;

                DatasetNames[dataset.Descriptor.DatasetId] = properties.Name;
            }
        }

        public string GetDatasetName(DatasetId datasetId)
        {
            return DatasetNames.TryGetValue(datasetId, out var datasetName) ? datasetName : datasetId.ToString();
        }

        public async Task SetSelectedDatasetAsync(DatasetId? datasetId)
        {
            CurrentDatasetId = datasetId;
            await GetFilesAsync();
        }

        public async Task GetFilesAsync()
        {
            var filePath = CurrentFilePath;
            CurrentFilePath = null;

            CleanFileCancellation();
            FileProperties.Clear();

            if (CurrentDatasetId == null) return;

            var dataset = Datasets.FirstOrDefault(d => d.Descriptor.DatasetId == CurrentDatasetId);
            if (dataset == null) return;

            m_FileCancellationSource = new CancellationTokenSource();
            var token = m_FileCancellationSource.Token;

            var fileList = dataset.ListFilesAsync(Range.All, token);
            await foreach (var file in fileList)
            {
                if (filePath == file.Descriptor.Path)
                {
                    CurrentFilePath = file.Descriptor.Path;
                }

                var properties = await file.GetPropertiesAsync(token);
                FileProperties[file.Descriptor.Path] = properties;
            }
        }

        void CleanDatasetCancellation()
        {
            if (m_DatasetCancellationSource != null)
            {
                m_DatasetCancellationSource.Cancel();
                m_DatasetCancellationSource.Dispose();
            }

            m_DatasetCancellationSource = null;
        }

        void CleanFileCancellation()
        {
            if (m_FileCancellationSource != null)
            {
                m_FileCancellationSource.Cancel();
                m_FileCancellationSource.Dispose();
            }

            m_FileCancellationSource = null;
        }

        async Task<IFile> GetFileAsync(string filePath)
        {
            if (CurrentDatasetId == null || string.IsNullOrEmpty(filePath)) return null;

            var dataset = Datasets.FirstOrDefault(d => d.Descriptor.DatasetId == CurrentDatasetId)
                ?? await CurrentAsset.GetDatasetAsync(CurrentDatasetId.Value, CancellationToken.None);

            return await dataset.GetFileAsync(CurrentFilePath, CancellationToken.None);
        }

        #endregion

        #region Example_Behaviour_UpdateAssetFile

        public async Task UpdateFileAsync(IFileUpdate fileUpdate)
        {
            var file = await GetFileAsync(CurrentFilePath);
            if (file == null) return;

            try
            {
                await file.UpdateAsync(fileUpdate, CancellationToken.None);
                await file.RefreshAsync(CancellationToken.None);

                var properties = await file.GetPropertiesAsync(CancellationToken.None);
                FileProperties[CurrentFilePath] = properties;

                Debug.Log("File updated.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to update file. {e}");
                throw;
            }
        }

        #endregion

        #region Example_Behaviour_DownloadAssetFile

        class LogProgress : IProgress<HttpProgress>
        {
            public void Report(HttpProgress value)
            {
                if (!value.DownloadProgress.HasValue) return;

                Debug.Log($"Download progress: {value.DownloadProgress * 100} %");
            }
        }

        public async Task DownloadFileAsync(string filePath)
        {
            const string dialogHeader = "Download file to location:";

            var defaultFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var folder = UnityEditor.EditorUtility.OpenFolderPanel(dialogHeader, defaultFolder, "");

            if (string.IsNullOrEmpty(folder)) return;

            var downloadPath = Path.Combine(folder, filePath);

            try
            {
                // Create the necessary directories
                var directory = Path.GetDirectoryName(downloadPath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var file = await GetFileAsync(filePath);

                await using var destination = File.OpenWrite(downloadPath);

                var progress = new LogProgress();
                await file.DownloadAsync(destination, progress, default);

                Debug.Log($"Asset file downloaded: {filePath}.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to download asset file: {filePath}. {e}");

                if (File.Exists(downloadPath))
                {
                    File.Delete(downloadPath);
                }
            }
        }

        #endregion

        #region Example_Behaviour_GenerateFileTags

        public IEnumerable<GeneratedTag> GeneratedTags { get; private set; }

        CancellationTokenSource TagGenerationCancellationSource;

        public async Task GenerateTagsAsync()
        {
            CancelTagGeneration();

            TagGenerationCancellationSource = new CancellationTokenSource();

            try
            {
                var file = await GetFileAsync(CurrentFilePath);
                GeneratedTags = await file.GenerateSuggestedTagsAsync(TagGenerationCancellationSource.Token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Cancelled tag generation for {CurrentFilePath}.");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public void CancelTagGeneration()
        {
            if (TagGenerationCancellationSource != null)
            {
                TagGenerationCancellationSource.Cancel();
                TagGenerationCancellationSource.Dispose();
            }

            TagGenerationCancellationSource = null;
            GeneratedTags = null;
        }

        #endregion
    }
}
