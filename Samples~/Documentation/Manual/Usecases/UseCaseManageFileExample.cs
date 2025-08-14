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

    public class UseCaseManageFileExampleUI : IAssetManagementUI
    {
        readonly BaseAssetBehaviour m_Behaviour;

        public UseCaseManageFileExampleUI(BaseAssetBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseManageFileExample : IAssetManagementUI
    {
        readonly UseCaseManageFileExampleBehaviour m_Behaviour;

        public UseCaseManageFileExample(BaseAssetBehaviour behaviour)
        {
            m_Behaviour = new UseCaseManageFileExampleBehaviour(behaviour);
        }

        #region Example_UIContent

        IAsset m_CurrentAsset;
        Vector2 m_DatasetsScrollPosition;
        Vector2 m_ListScrollPosition;

        FileUpdate m_FileUpdate;
        string m_TagsString = string.Empty;

        public void OnGUI()
        {
            if (m_Behaviour.CurrentAsset == null) return;

            if (m_CurrentAsset != m_Behaviour.CurrentAsset)
            {
                m_CurrentAsset = m_Behaviour.CurrentAsset;
                m_FileUpdate = null;
                m_Behaviour.CancelTagGeneration();
                _ = m_Behaviour.GetDatasets();
            }

            GUILayout.BeginVertical();

            // Get a local copy of the list of datasets to avoid concurrent modification exceptions.
            DisplayDatasetSelection(m_Behaviour.Datasets.ToArray());

            GUILayout.EndVertical();

            if (m_Behaviour.CurrentDatasetId == null) return;

            GUILayout.BeginVertical();

            // Get a local copy of the list of files to avoid concurrent modification exceptions.
            IEnumerable<IFile> files = m_Behaviour.DatasetFiles.TryGetValue(m_Behaviour.CurrentDatasetId.Value, out var fileList)
                ? fileList
                : Array.Empty<IFile>();
            DisplayFileSelection(files.Select(x => x.Descriptor.Path).ToArray());

            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(250));

            DisplaySelectedFile();

            GUILayout.EndVertical();
        }

        void DisplayDatasetSelection(IReadOnlyCollection<IDataset> datasets)
        {
            if (GUILayout.Button("Refresh", GUILayout.Width(60)))
            {
                _ = m_Behaviour.GetDatasets();
            }

            GUILayout.Space(5);

            if (datasets.Count == 0)
            {
                GUILayout.Label("No datasets.");
                return;
            }

            m_DatasetsScrollPosition = GUILayout.BeginScrollView(m_DatasetsScrollPosition, GUILayout.ExpandHeight(true), GUILayout.Width(250));

            foreach (var dataset in datasets)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label(m_Behaviour.GetDatasetName(dataset.Descriptor.DatasetId), GUILayout.Width(150));

                GUI.enabled = dataset.Descriptor.DatasetId != m_Behaviour.CurrentDatasetId;

                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    m_FileUpdate = null;
                    m_Behaviour.CancelTagGeneration();
                    m_Behaviour.CurrentDatasetId = dataset.Descriptor.DatasetId;
                    _ = m_Behaviour.GetFilesAsync(dataset.Descriptor.DatasetId);
                }

                GUI.enabled = true;

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        void DisplayFileSelection(IReadOnlyCollection<string> filePaths)
        {
            if (GUILayout.Button("Refresh", GUILayout.Width(60)) && m_Behaviour.CurrentDatasetId.HasValue)
            {
                m_FileUpdate = null;
                m_Behaviour.CancelTagGeneration();
                _ = m_Behaviour.GetFilesAsync(m_Behaviour.CurrentDatasetId.Value);
                return;
            }

            GUILayout.Space(15f);

            if (filePaths.Count == 0)
            {
                GUILayout.Label("! No files !");
                return;
            }

            m_ListScrollPosition = GUILayout.BeginScrollView(m_ListScrollPosition, GUILayout.ExpandHeight(true), GUILayout.Width(330));

            DisplayFiles(filePaths);

            GUILayout.EndScrollView();
        }

        void DisplayFiles(IReadOnlyCollection<string> filePaths)
        {
            foreach (var filePath in filePaths)
            {
                if (!m_Behaviour.FileProperties.TryGetValue(filePath, out var fileProperties))
                {
                    GUILayout.Label(filePath);
                    continue;
                }

                GUILayout.BeginHorizontal();

                GUILayout.Label($"{filePath}", GUILayout.Width(150));

                GUI.enabled = filePath != m_Behaviour.CurrentFilePath;

                if (GUILayout.Button("Select", GUILayout.Width(70)))
                {
                    m_FileUpdate = null;
                    m_Behaviour.CurrentFilePath = filePath;
                    m_TagsString = string.Join(',', fileProperties.Tags ?? Array.Empty<string>());
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
            if (m_Behaviour.CurrentFilePath == null) return;

            if (!m_Behaviour.FileProperties.TryGetValue(m_Behaviour.CurrentFilePath, out var properties))
            {
                GUILayout.Label("Loading properties...");
                return;
            }

            GUILayout.Label($"Status: {properties.StatusName}");

            var createdDate = properties.AuthoringInfo?.Created.ToString("d") ?? "unknown";
            GUILayout.Label($"Created on: {createdDate}");

            var modifiedDate = properties.AuthoringInfo?.Updated.ToString("d") ?? "unknown";
            GUILayout.Label($"Last modified on: {modifiedDate}");

            GUILayout.Label($"Size: {properties.SizeBytes} bytes");

            GUILayout.Label("Description");
            var description = GUILayout.TextField(m_FileUpdate?.Description ?? properties.Description);
            if (m_FileUpdate != null || properties.Description != description)
            {
                m_FileUpdate ??= new FileUpdate();
                m_FileUpdate.Description = description;
            }

            GUILayout.Label("Tags (comma separated)");
            var tags = GUILayout.TextArea(m_TagsString);
            if (m_TagsString != tags)
            {
                m_TagsString = tags;
                OnTagStringUpdated();
            }

            if (GUILayout.Button("Generate Tags"))
            {
                _ = m_Behaviour.GenerateTagsAsync();
            }

            DisplayGeneratedTags(m_FileUpdate?.Tags ?? properties.Tags ?? Array.Empty<string>());

            GUILayout.Space(15f);

            GUI.enabled = m_FileUpdate != null;
            
            if (GUILayout.Button("Update"))
            {
                _ = m_Behaviour.UpdateFileAsync(m_FileUpdate);
                m_FileUpdate = null;
            }
            
            GUI.enabled = true;
        }

        void DisplayGeneratedTags(IEnumerable<string> existingTags)
        {
            if (m_Behaviour.GeneratedTags != null)
            {
                foreach (var tag in m_Behaviour.GeneratedTags)
                {
                    GUILayout.BeginHorizontal();

                    GUI.enabled = !existingTags.Contains(tag.Value);

                    if (GUILayout.Button("+", GUILayout.Width(20)))
                    {
                        if (string.IsNullOrWhiteSpace(m_TagsString))
                        {
                            m_TagsString = tag.Value;
                        }
                        else
                        {
                            m_TagsString += $", {tag.Value}";
                        }

                        OnTagStringUpdated();
                    }

                    GUILayout.Label($"{tag.Value}, Confidence: {tag.Confidence:F3}");

                    GUI.enabled = true;

                    GUILayout.EndHorizontal();
                }
            }
        }

        void OnTagStringUpdated()
        {
            m_FileUpdate ??= new FileUpdate();
            m_FileUpdate.Tags = m_TagsString.Split(',')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();
        }

        #endregion
    }

    class UseCaseManageFileExampleBehaviour : UseCaseCreateFileExampleBehaviour
    {
        public UseCaseManageFileExampleBehaviour(BaseAssetBehaviour behaviour)
            : base(behaviour) { }

        #region Example_Behaviour_UpdateAssetFile

        public async Task UpdateFileAsync(IFileUpdate fileUpdate)
        {
            var file = DatasetFiles.Where(x => x.Key == CurrentDatasetId)
                .SelectMany(x => x.Value)
                .FirstOrDefault(x => x.Descriptor.Path == CurrentFilePath);
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

        public async Task DownloadFileAsync(string filePath)
        {
#if UNITY_EDITOR
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

                var file = DatasetFiles.Where(x => x.Key == CurrentDatasetId)
                    .SelectMany(x => x.Value)
                    .FirstOrDefault(x => x.Descriptor.Path == CurrentFilePath);
                if (file == null) return;

                await using var destination = File.OpenWrite(downloadPath);

                await file.DownloadAsync(destination, new LogProgress(downloadPath), default);

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
#else
            Debug.Log("Feature only supported in Editor.");
#endif
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
                var file = DatasetFiles.Where(x => x.Key == CurrentDatasetId)
                    .SelectMany(x => x.Value)
                    .FirstOrDefault(x => x.Descriptor.Path == CurrentFilePath);
                if (file == null) return;
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