using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetManager
{
    public class FileController
    {
        class FileUploadProgress : IProgress<HttpProgress>
        {
            readonly ProgressBar m_ProgressBar;
            string m_FileLabel;

            public FileUploadProgress(ProgressBar progressBar)
            {
                m_ProgressBar = progressBar;
            }

            public void SetFileLabel(string fileLabel)
            {
                m_FileLabel = fileLabel;
                m_ProgressBar.value = 0;
                m_ProgressBar.title = $"Starting upload of file {m_FileLabel}...";
            }

            public void Report(HttpProgress value)
            {
                m_ProgressBar.value = value.UploadProgress.HasValue ? value.UploadProgress.Value * 100 : 0;
                if (m_ProgressBar.value <= 0) return;

                m_ProgressBar.title = $"Uploading file {m_FileLabel}... {m_ProgressBar.value:0}%";
            }
        }

        VisualElement m_FileUpload;

        ScrollView m_FileScrollView;

        string m_LastOpenedFolder;

        readonly List<string> m_FilesToCreate = new();
        readonly List<IFile> m_FilesToDelete = new();
        Button m_FileUploadButton;
        List<VisualElement> m_FileDeleteIcons = new();

        CancellationTokenSource m_GetFilesCancellationTokenSource;

        public event Action<IEnumerable<string>> FilesAdded;
        public event Action<IEnumerable<string>> FilesRemoved;

        public void Init(VisualElement datasetPanel)
        {
            m_FileScrollView = datasetPanel.Q<ScrollView>("DatasetFileScrollView");
            m_FileUpload = datasetPanel.Q("FileUpload");
            m_FileUploadButton = datasetPanel.Q<Button>("FileUploadButton");

            // Call backs----------------------------------
            m_FileUploadButton.RegisterCallback<ClickEvent>(BrowseFile);
        }

        public void Cleanup()
        {
            m_FileUploadButton.UnregisterCallback<ClickEvent>(BrowseFile);
        }

        public void Clear()
        {
            m_FilesToCreate.Clear();
            m_FilesToDelete.Clear();
            m_FileScrollView.Clear();
            m_FileDeleteIcons.Clear();
        }

        public void Show()
        {
            m_FileUpload.visible = true;
        }

        public void Hide()
        {
            m_FileUpload.visible = false;
        }

        public void SetFileUploadEnabled(bool state)
        {
            m_FileUploadButton.SetEnabled(state);
            m_FileDeleteIcons.ForEach(button => button.SetEnabled(state));
        }

        public async Task ListExistingFiles(IDataset dataset, bool canUpdate)
        {
            m_FileUpload.style.display = canUpdate ? DisplayStyle.Flex : DisplayStyle.None;

            if (m_GetFilesCancellationTokenSource != null)
            {
                m_GetFilesCancellationTokenSource.Cancel();
                m_GetFilesCancellationTokenSource.Dispose();
            }

            m_GetFilesCancellationTokenSource = new CancellationTokenSource();

            if (canUpdate)
            {
                Show();
            }
            else
            {
                Hide();
            }

            await foreach (var file in dataset.ListFilesAsync(Range.All, m_GetFilesCancellationTokenSource.Token))
            {
                AddFileRow(file, canUpdate);
            }
        }

        public async Task RemoveFiles()
        {
            foreach (var file in m_FilesToDelete)
            {
                await RemoveFileAsync(file);
            }
        }

        public async Task<bool> UploadFiles(IDataset sourceDataset, ProgressBar progressBar = null)
        {
            FileUploadProgress progress = null;
            if (progressBar != null)
            {
                progress = new FileUploadProgress(progressBar);
            }

            var fileNb = 1;
            var fileCount = m_FilesToCreate.Count;
            for (var i = m_FilesToCreate.Count - 1; i >= 0; --i)
            {
                var file = m_FilesToCreate[i];
                progress?.SetFileLabel($"{Path.GetFileName(file)} ({fileNb++} of {fileCount})");
                var assetFile = await UploadAssetFileAsync(sourceDataset, file, progress);
                if (assetFile == null) return false;
                m_FilesToCreate.RemoveAt(i);
            }

            return true;
        }

        public bool TryGetNameAndType(out string name, out AssetType type)
        {
            name = "";
            type = AssetType.Other;
            if (m_FilesToCreate.Count == 0) return false;

            // Find the first file whose type can be recognized
            foreach (var filePath in m_FilesToCreate)
            {
                var tempType = GetAssetType(filePath);
                if (tempType.HasValue)
                {
                    type = tempType.Value;
                    name = Path.GetFileNameWithoutExtension(filePath);
                    return true;
                }
            }

            // Otherwise fallback to the values of the first file
            name = Path.GetFileNameWithoutExtension(m_FilesToCreate[0]);
            type = AssetType.Other;
            return true;
        }

        static async Task RemoveFileAsync(IFile file)
        {
            try
            {
                var datasets = file.GetLinkedDatasetsAsync(Range.All, default);
                var taskList = new List<Task>();
                await foreach (var dataset in datasets)
                {
                    taskList.Add(dataset.RemoveFileAsync(file.Descriptor.Path, default));
                }

                await Task.WhenAll(taskList);
            }
            catch (OperationCanceledException oe)
            {
                oe.LogException();
                DialogService.ShowMessage("Error", $"Failed to remove file: {file.Descriptor.Path}. Request cenceled.");
            }
            catch (Exception e)
            {
                e.LogException();
                DialogService.ShowMessage("Error", $"Failed to remove file: {file.Descriptor.Path}.");
            }
        }

        void AddFileRow(IFile file, bool canUpdate)
        {
            var fileItem = new RowItem();
            fileItem.AddLabel(file.Descriptor.Path);
            fileItem.AddLabel(GetSizeAsUserFriendlyFormat(file.SizeBytes), 80);

            if (canUpdate)
            {
                var deleteButton = new VisualElement();
                deleteButton.AddToClassList("delete-icon");
                fileItem.Add(deleteButton);
                deleteButton.RegisterCallback<ClickEvent>(_ =>
                {
                    m_FilesToDelete.Add(file);
                    fileItem.RemoveFromHierarchy();
                });
                m_FileDeleteIcons.Add(deleteButton);
            }

            m_FileScrollView.Add(fileItem);
        }

        void BrowseFile(ClickEvent evt)
        {
            const string addFileHeader = "Input path to the file:";

            if (string.IsNullOrEmpty(m_LastOpenedFolder))
            {
                m_LastOpenedFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

#if UNITY_EDITOR
            var path = UnityEditor.EditorUtility.OpenFilePanel(addFileHeader, m_LastOpenedFolder, "");
            if (!string.IsNullOrEmpty(path))
                OnFileSelected(path);
#else
            DialogService.ShowMessage("Upload a file", addFileHeader, OnFileSelected, m_LastOpenedFolder);
#endif
        }

        void OnFileSelected(string filePath)
        {
            if (m_FilesToCreate.Contains(filePath))
            {
                DialogService.ShowMessage("Duplicate file", "The file has already been added.");
                return;
            }

            if (File.Exists(filePath))
            {
                m_LastOpenedFolder = Path.GetDirectoryName(filePath);

                var fileName = Path.GetFileName(filePath);
                var fileInfo = new FileInfo(filePath);

                m_FilesToCreate.Add(filePath);

                var fileItem = new RowItem();
                fileItem.AddLabel(fileName);
                fileItem.AddLabel(GetSizeAsUserFriendlyFormat(fileInfo.Length), 80);

                var deleteButton = new VisualElement {name = "DeleteIcon"};
                deleteButton.AddToClassList("delete-icon");
                fileItem.Add(deleteButton);
                deleteButton.RegisterCallback<ClickEvent>(_ =>
                {
                    m_FilesToCreate.Remove(filePath);
                    fileItem.RemoveFromHierarchy();

                    FilesRemoved?.Invoke(m_FilesToCreate);
                });

                m_FileScrollView.Add(fileItem);

                FilesAdded?.Invoke(m_FilesToCreate);
            }
            else
            {
                DialogService.ShowMessage("Invalid path", "The specified file could not be found.");
            }
        }

        static async Task<IFile> UploadAssetFileAsync(IDataset dataset, string filePath, IProgress<HttpProgress> progress = null)
        {
            var fileCreation = new FileCreation(Path.GetFileName(filePath))
            {
                Description = "",
                Tags = GetAssetFileTags(filePath)
            };

            IFile assetFile;

            try
            {
                var fileStream = File.OpenRead(filePath);
                assetFile = await dataset.UploadFileAsync(fileCreation, fileStream, progress, CancellationToken.None);
                if (assetFile == null)
                {
                    DialogService.ShowMessage("Error", $"Failed to upload file: {filePath}");
                }
            }
            catch (Exception e)
            {
                e.LogException();
                DialogService.ShowMessage("Error", $"Failed to upload file: {filePath}.");
                assetFile = null;
            }

            return assetFile;
        }

        static IEnumerable<string> GetAssetFileTags(string filePath)
        {
            var assetType = GetAssetType(filePath);
            if (assetType.HasValue) return new List<string> {assetType.Value.ToString()};
            return Array.Empty<string>();
        }

        static AssetType? GetAssetType(string assetPath)
        {
            var assetExtension = Path.GetExtension(assetPath).ToLower();
            switch (assetExtension)
            {
                case ".mat":
                    return AssetType.Material;
                case ".png":
                case ".jpg":
                case ".jpeg":
                    return AssetType.Asset_2D;
                case ".prefab":
                case ".fbx":
                    return AssetType.Model_3D;
                case ".mp3":
                case ".wav":
                    return AssetType.Audio;
                case ".mp4":
                case ".mov":
                    return AssetType.Video;
                case ".cs":
                    return AssetType.Script;
                case ".unity":
                case ".shader":
                    return AssetType.Other;
            }

            return null;
        }

        static string GetSizeAsUserFriendlyFormat(long fileSize)
        {
            string[] sizes = {"B", "KB", "MB", "GB", "TB"};
            double len = fileSize;

            var order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }
    }
}
