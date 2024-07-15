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

        IDataset m_CurrentDataset;
        IFile m_CurrentFile;
        FileUpdate m_FileUpdate;
        IEnumerable<GeneratedTag> m_GeneratedTags;

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
                m_CurrentDataset = null;
                m_CurrentFile = null;
                m_FileUpdate = null;
                m_Behaviour.CancelTagGeneration();
                m_GeneratedTags = null;
                _ = m_Behaviour.GetDataSetsAsync();
            }

            if (m_Behaviour.Datasets == null)
            {
                GUILayout.Label("Loading datasets...");
                return;
            }

            GUILayout.BeginVertical();

            DisplayDatasetSelection();

            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            DisplayFileSelection();

            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            DisplaySelectedFile();

            GUILayout.EndVertical();
        }

        void DisplayDatasetSelection()
        {
            if (GUILayout.Button("Refresh", GUILayout.Width(60)))
            {
                m_CurrentDataset = null;
                m_CurrentFile = null;
                _ = m_Behaviour.GetDataSetsAsync();
                return;
            }

            GUILayout.Space(5);

            m_DatasetsScrollPosition = GUILayout.BeginScrollView(m_DatasetsScrollPosition, GUILayout.ExpandHeight(true), GUILayout.Width(Screen.width * 0.15f));

            DisplayDatasets(m_Behaviour.Datasets.ToArray());

            GUILayout.EndScrollView();
        }

        void DisplayDatasets(IReadOnlyCollection<IDataset> datasets)
        {
            if (datasets.Count == 0)
            {
                GUILayout.Label("No datasets.");
                return;
            }

            // Get a local copy of the list of asset files to avoid concurrent modification exceptions.
            foreach (var dataset in datasets)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label($"{dataset.Name}");

                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    m_CurrentDataset = dataset;
                    _ = ListFiles();
                }

                GUILayout.EndHorizontal();
            }
        }

        void DisplayFileSelection()
        {
            if (m_CurrentDataset == null)
            {
                GUILayout.Label("! No dataset selected !");
                return;
            }

            if (m_Behaviour.Files == null)
            {
                GUILayout.Label("Loading files...");
                return;
            }

            if (GUILayout.Button("Refresh", GUILayout.Width(60)))
            {
                m_CurrentFile = null;
                _ = ListFiles();
                return;
            }

            GUILayout.Space(5);

            m_FilesScrollPosition = GUILayout.BeginScrollView(m_FilesScrollPosition, GUILayout.MaxHeight(Screen.height * 0.8f), GUILayout.Width(Screen.width * 0.2f));

            DisplayFiles(m_Behaviour.Files.ToArray());

            GUILayout.EndScrollView();
        }

        void DisplayFiles(IReadOnlyCollection<IFile> files)
        {
            if (files.Count == 0)
            {
                GUILayout.Label("No files.");
                return;
            }

            // Get a local copy of the list of asset files to avoid concurrent modification exceptions.
            foreach (var assetFile in files)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label($"{assetFile.Descriptor.Path}");

                if (GUILayout.Button("Select", GUILayout.Width(70)))
                {
                    m_CurrentFile = assetFile;
                    m_FileUpdate = new FileUpdate(assetFile);

                    m_Behaviour.CancelTagGeneration();
                    m_GeneratedTags = null;
                }

                if (GUILayout.Button("Download", GUILayout.Width(70)))
                {
                    _ = m_Behaviour.DownloadFileAsync(assetFile);
                }

                GUILayout.EndHorizontal();
            }
        }

        void DisplaySelectedFile()
        {
            if (m_Behaviour.Files == null || !m_Behaviour.Files.Any()) return;

            if (m_CurrentFile == null)
            {
                GUILayout.Label("! No file selected !");
                return;
            }

            GUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label($"{m_CurrentFile.Descriptor.Path}");

            GUILayout.Label($"Status: {m_CurrentFile.Status}");

            var createdDate = m_CurrentFile.AuthoringInfo?.Created.ToString("d") ?? "unknown";
            GUILayout.Label($"Created on: {createdDate}");

            var modifiedDate = m_CurrentFile.AuthoringInfo?.Updated.ToString("d") ?? "unknown";
            GUILayout.Label($"Last modified on: {modifiedDate}");

            GUILayout.Label($"Size: {m_CurrentFile.SizeBytes} bytes");

            GUILayout.EndVertical();

            GUILayout.Space(5);

            GUILayout.Label("Description:");
            m_FileUpdate.Description = GUILayout.TextField(m_FileUpdate.Description);

            GUILayout.Label("Tags:");
            var tags = string.Join(", ", m_FileUpdate.Tags ?? Array.Empty<string>());
            m_FileUpdate.Tags = GUILayout.TextField(tags).Split(',')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            if (GUILayout.Button("Generate Tags"))
            {
                _ = GenerateTagsAsync();
            }

            DisplayGeneratedTags();

            GUILayout.Space(5);

            if (GUILayout.Button("Update"))
            {
                _ = m_Behaviour.UpdateFileAsync(m_CurrentFile, m_FileUpdate);
            }
        }

        async Task ListFiles()
        {
            m_CurrentFile = null;
            m_FileUpdate = null;
            m_Behaviour.CancelTagGeneration();
            m_GeneratedTags = null;
            await m_Behaviour.GetFilesAsync(m_CurrentDataset);
        }

        async Task GenerateTagsAsync()
        {
            m_GeneratedTags = null;
            m_GeneratedTags = await m_Behaviour.GenerateTagsAsync(m_CurrentFile);
        }

        void DisplayGeneratedTags()
        {
            if (m_GeneratedTags != null)
            {
                foreach (var tag in m_GeneratedTags)
                {
                    GUILayout.BeginHorizontal();

                    GUI.enabled = !m_FileUpdate.Tags?.Contains(tag.Value) ?? true;

                    if (GUILayout.Button("Add", GUILayout.Width(40)))
                    {
                        m_FileUpdate.Tags ??= Array.Empty<string>();
                        m_FileUpdate.Tags = m_FileUpdate.Tags.Append(tag.Value).ToArray();
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

        public IEnumerable<IDataset> Datasets { get; private set; }

        public IEnumerable<IFile> Files { get; set; }

        public async Task GetDataSetsAsync()
        {
            CleanFileCancellation();
            Files = null;

            CleanDatasetCancellation();
            Datasets = null;

            if (CurrentAsset == null) return;

            m_DatasetCancellationSource = new CancellationTokenSource();
            var token = m_DatasetCancellationSource.Token;

            var datasets = new List<IDataset>();
            var datasetList = CurrentAsset.ListDatasetsAsync(Range.All, token);
            await foreach (var dataset in datasetList)
            {
                if (token.IsCancellationRequested) break;

                datasets.Add(dataset);
            }

            if (token.IsCancellationRequested) return;

            Datasets = datasets;
            CleanDatasetCancellation();
        }

        public async Task GetFilesAsync(IDataset dataset)
        {
            CleanFileCancellation();

            Files = null;

            if (dataset == null) return;

            m_FileCancellationSource = new CancellationTokenSource();
            var token = m_FileCancellationSource.Token;

            var files = new List<IFile>();
            var fileList = dataset.ListFilesAsync(Range.All, token);
            await foreach (var file in fileList)
            {
                files.Add(file);
            }

            Files = files;
            CleanFileCancellation();
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

        #endregion

        #region Example_Behaviour_UpdateAssetFile

        public async Task UpdateFileAsync(IFile assetFile, IFileUpdate fileUpdate)
        {
            await assetFile.UpdateAsync(fileUpdate, CancellationToken.None);
            Debug.Log("File updated.");
            await assetFile.RefreshAsync(CancellationToken.None);
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

        public async Task DownloadFileAsync(IFile assetFile)
        {
            const string dialogHeader = "Download file to location:";

            var defaultFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var folder = UnityEditor.EditorUtility.OpenFolderPanel(dialogHeader, defaultFolder, "");

            if (string.IsNullOrEmpty(folder)) return;

            var filePath = Path.Combine(folder, assetFile.Descriptor.Path);

            try
            {
                // Create the necessary directories
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await using var destination = File.OpenWrite(filePath);

                var progress = new LogProgress();
                await assetFile.DownloadAsync(destination, progress, default);

                Debug.Log($"Asset file downloaded: {assetFile.Descriptor.Path}.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to download asset file: {assetFile.Descriptor.Path}. {e}");

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        #endregion

        #region Example_Behaviour_GenerateFileTags

        CancellationTokenSource TagGenerationCancellationSource;

        public async Task<IEnumerable<GeneratedTag>> GenerateTagsAsync(IFile file)
        {
            CancelTagGeneration();

            TagGenerationCancellationSource = new CancellationTokenSource();

            try
            {
                return await file.GenerateSuggestedTagsAsync(TagGenerationCancellationSource.Token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Cancelled tag generation for {file.Descriptor.Path}.");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return null;
        }

        public void CancelTagGeneration()
        {
            if (TagGenerationCancellationSource != null)
            {
                TagGenerationCancellationSource.Cancel();
                TagGenerationCancellationSource.Dispose();
            }

            TagGenerationCancellationSource = null;
        }

        #endregion
    }
}
