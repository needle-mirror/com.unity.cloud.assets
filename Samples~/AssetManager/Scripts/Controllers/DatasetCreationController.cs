#if !UC_EXCLUDE_SAMPLES

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetManager
{
    public class DatasetCreationController
    {
        VisualTreeAsset m_FileListItemTemplate;
        VisualTreeAsset m_DatasetTagsTemplate;
        VisualElement m_RightPanel;
        VisualElement m_DatasetPanel;
        VisualElement m_DatasetTagsContainer;
        VisualElement m_FileUpload;
        VisualElement m_DatasetStatusCircle;
        VisualElement m_DatasetLastEdit;

        Label m_DatasetTitleLabel;
        Label m_DatasetStatusNameLabel;
        Label m_DatasetStatusLastEditLabel;
        TextField m_DatasetNameField;
        TextField m_DatasetTagsField;
        TextField m_DatasetDescriptionField;
        ScrollView m_FileScrollView;
        Toggle m_DatasetVisibleToggle;

        string m_LastOpenedFolder;

        readonly List<string> m_FilesToCreate = new();
        readonly List<IFile> m_FilesToDelete = new();
        AssetCreation m_CurrentAssetCreation;
        DatasetCreation m_CurrentDatasetCreation;
        Button m_CreateAssetButton;
        Button m_FileUploadButton;
        Button m_SaveDatasetButton;
        Button m_BackButton;

        public event Action<IAsset> OnClosePanel;

        MessageDialogController m_MessageDialogController;
        TextInputDialogController m_AssetFilePathDialogController;

        IAssetProject m_AssetProject;
        IAsset m_CurrentAsset;
        IDataset m_CurrentDataset;
        IDatasetUpdate m_DatasetUpdate;
        CancellationTokenSource m_GetFilesCancellationTokenSource;

        internal void Init(VisualElement datasetPanel, VisualTreeAsset fileListItemTemplate, VisualTreeAsset tagsTemplate, IDialogController dialogController, IDialogController assetFilePathDialogController)
        {
            m_DatasetPanel = datasetPanel;
            m_FileListItemTemplate = fileListItemTemplate;
            m_DatasetTagsTemplate = tagsTemplate;
            m_MessageDialogController = (MessageDialogController) dialogController;

#if !UNITY_EDITOR
            m_AssetFilePathDialogController = (TextInputDialogController)assetFilePathDialogController;
#endif

            m_RightPanel = datasetPanel.Q<VisualElement>("RightPanel");
            m_DatasetTagsContainer = datasetPanel.Q<VisualElement>("DatasetTagsChipContainer");

            m_DatasetTitleLabel = m_DatasetPanel.Q<Label>("DatasetTitleLabel");
            m_DatasetStatusCircle = m_DatasetPanel.Q<VisualElement>("StatusCircle");
            m_DatasetLastEdit = m_DatasetPanel.Q<VisualElement>("DatasetLastEdit");
            m_DatasetStatusNameLabel = m_DatasetPanel.Q<Label>("StatusNameLabel");
            m_DatasetStatusLastEditLabel = m_DatasetPanel.Q<Label>("DatasetLastEditDateLabel");
            m_FileScrollView = m_DatasetPanel.Q<ScrollView>("DatasetFileScrollView");
            m_DatasetNameField = m_DatasetPanel.Q<TextField>("DatasetNameField");
            m_DatasetTagsField = m_DatasetPanel.Q<TextField>("DatasetTagsField");
            m_DatasetDescriptionField = m_DatasetPanel.Q<TextField>("DatasetDescriptionField");
            m_DatasetVisibleToggle = m_DatasetPanel.Q<Toggle>("DatasetVisibleToggle");

            m_FileUpload = m_DatasetPanel.Q<VisualElement>("FileUpload");

            m_FileUploadButton = m_DatasetPanel.Q<Button>("FileUploadButton");
            m_CreateAssetButton = m_DatasetPanel.Q<Button>("CreateAssetButton");
            m_SaveDatasetButton = m_DatasetPanel.Q<Button>("DatasetSaveButton");

            // Call backs----------------------------------
            m_FileUploadButton.RegisterCallback<ClickEvent>(BrowseFile);
            m_CreateAssetButton.RegisterCallback<ClickEvent>(CreateAsset);
            m_SaveDatasetButton.RegisterCallback<ClickEvent>(UpdateDataset);

            m_DatasetNameField.RegisterValueChangedCallback(evt =>
            {
                if (m_DatasetUpdate != null && m_DatasetUpdate.Name != evt.newValue)
                    m_DatasetUpdate.Name = evt.newValue;
            });

            m_DatasetDescriptionField.RegisterValueChangedCallback(evt =>
            {
                if (m_DatasetUpdate != null && m_DatasetUpdate.Description != evt.newValue)
                    m_DatasetUpdate.Description = evt.newValue;
            });

            m_DatasetVisibleToggle.RegisterValueChangedCallback(evt =>
            {
                if (m_DatasetUpdate != null && m_DatasetUpdate.IsVisible != evt.newValue)
                    m_DatasetUpdate.IsVisible = evt.newValue;
            });

            m_DatasetTagsField.RegisterCallback<FocusInEvent>(AddTags);

            m_BackButton = datasetPanel.Q<Button>("BackBtn");
            m_BackButton.RegisterCallback<ClickEvent>(_ => { ClosePanel(null); });
        }

        public void Cleanup()
        {
            m_FileUploadButton.UnregisterCallback<ClickEvent>(BrowseFile);
            m_CreateAssetButton.UnregisterCallback<ClickEvent>(CreateAsset);
            m_SaveDatasetButton.UnregisterCallback<ClickEvent>(UpdateDataset);
            m_DatasetTagsField.UnregisterCallback<FocusInEvent>(AddTags);
        }

        internal void OpenDataset(IAsset asset, IDataset dataset)
        {
            DisplayElement(m_DatasetPanel);
            DisplayElement(m_RightPanel);

            ClearDatasetInformation();

            DisplayElement(m_DatasetLastEdit);
            HideElement(m_CreateAssetButton);

            m_CurrentAsset = asset;
            m_CurrentDataset = dataset;
            m_DatasetUpdate = new DatasetUpdate(m_CurrentDataset);
            m_DatasetTitleLabel.text = dataset.Name;
            m_DatasetNameField.value = dataset.Name;
            m_DatasetDescriptionField.value = dataset.Description;
            m_DatasetStatusLastEditLabel.text = dataset.AuthoringInfo.Updated.ToString("MMM dd, yyyy h:mm tt GMT");
            m_DatasetVisibleToggle.value = dataset.IsVisible;

            DrawTags(m_DatasetUpdate.Tags);
            _ = ListExistingFiles(dataset);

            UpdateStatus();
        }

        internal void CreateNewAssetAndDataset(IAssetProject project)
        {
            DisplayElement(m_DatasetPanel);
            HideElement(m_RightPanel);

            ClearDatasetInformation();
            m_FileUpload.visible = true;
            m_FilesToCreate.Clear();
            m_FilesToDelete.Clear();

            m_AssetProject = project;
            m_CurrentDatasetCreation = new DatasetCreation("Default");
            var assetName = $"New Asset {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            m_CurrentAssetCreation = new AssetCreation(assetName)
            {
                Description = $"Description {assetName}"
            };

            ChangeButtonEnabledState(true);
        }

        internal async Task UpdateDatasetAsync()
        {
            if (m_CurrentDataset == null)
            {
                ChangeButtonEnabledState(true);
                return;
            }

            if (m_FilesToDelete.Count > 0)
            {
                // Delete files
                foreach (var file in m_FilesToDelete)
                {
                    await RemoveFileAsync(file);
                }
            }

            if (m_FilesToCreate.Count > 0)
            {
                // Upload files
                foreach (var file in m_FilesToCreate)
                {
                    var assetFile = await UploadAssetFileAsync(m_CurrentDataset, file);
                    if (assetFile == null)
                    {
                        ChangeButtonEnabledState(true);
                        m_MessageDialogController.OpenDialog($"Failed to update asset: an error occurs during creation of file:{file}.");
                        return;
                    }
                }
            }

            var cancellationTokenSource = new CancellationTokenSource();
            var updateTask = m_CurrentDataset.UpdateAsync(m_DatasetUpdate, cancellationTokenSource.Token);

            await updateTask;

            ChangeButtonEnabledState(true);

            if (updateTask.Status == TaskStatus.RanToCompletion)
            {
                m_MessageDialogController.OpenDialog("The dataset has been saved successfully.");
            }
            else
            {
                m_MessageDialogController.OpenDialog("An error occurs during the dataset saving.");
            }
        }

        async Task RemoveFileAsync(IFile file)
        {
            if (m_CurrentDataset == null)
                return;

            try
            {
                var cancellationTokenSource = new CancellationTokenSource();

                var datasets = file.GetLinkedDatasetsAsync(Range.All, cancellationTokenSource.Token);
                var taskList = new List<Task>();
                await foreach (var dataset in datasets)
                {
                    taskList.Add(dataset.RemoveFileAsync(file.Descriptor.Path, cancellationTokenSource.Token));
                }

                await Task.WhenAll(taskList);
            }
            catch (TaskCanceledException)
            {
                m_MessageDialogController.OpenDialog($"Failed to remove file:{file.Descriptor.Path}. Request timed out.");
            }
            catch (Exception e)
            {
                m_MessageDialogController.OpenDialog($"Failed to remove file:{file.Descriptor.Path}. {e.Message}");
            }
        }

        void DrawTags(IEnumerable<string> tagsList)
        {
            if (tagsList == null || !tagsList.Any()) return;

            foreach (var tag in tagsList.ToList())
            {
                AddTag(tag);
            }
        }

        void AddTag(string tag)
        {
            var chip = m_DatasetTagsTemplate.Instantiate();
            chip.Q<Label>().text = tag;
            m_DatasetTagsContainer.Add(chip);
            chip.Q<Button>().clicked += () =>
            {
                m_DatasetUpdate.Tags.Remove(tag);
                chip.RemoveFromHierarchy();
            };
        }

        void ClearDatasetInformation()
        {
            m_AssetProject = null;
            m_CurrentAsset = null;
            m_CurrentDataset = null;
            m_DatasetUpdate = null;
            m_CurrentAssetCreation = null;
            m_CurrentDatasetCreation = null;
            m_DatasetTitleLabel.text = "DatasetName";
            m_DatasetNameField.value = "";
            m_DatasetDescriptionField.value = "";
            m_DatasetTagsField.value = "";
            m_DatasetStatusLastEditLabel.text = "";
            m_DatasetTagsContainer.Clear();
            m_DatasetVisibleToggle.value = false;

            m_FilesToCreate.Clear();
            m_FilesToDelete.Clear();
            m_FileScrollView.Clear();
        }

        async Task ListExistingFiles(IDataset dataset)
        {
            if (m_GetFilesCancellationTokenSource != null)
            {
                m_GetFilesCancellationTokenSource.Cancel();
                m_GetFilesCancellationTokenSource.Dispose();
            }

            m_GetFilesCancellationTokenSource = new CancellationTokenSource();

            await foreach (var file in dataset.ListFilesAsync(Range.All, m_GetFilesCancellationTokenSource.Token))
            {
                AddFileRow(file);
            }
        }

        void AddFileRow(IFile file)
        {
            var fileItem = m_FileListItemTemplate.Instantiate();
            fileItem.Q<Label>("FileNameLabel").text = file.Descriptor.Path;
            fileItem.Q<Label>("FileSizeLabel").text = GetSizeAsUserFriendlyFormat(file.SizeBytes);

            var deleteButton = fileItem.Q<VisualElement>("DeleteIcon");
            deleteButton.style.display = DisplayStyle.Flex;
            deleteButton.RegisterCallback<ClickEvent>(_ =>
            {
                m_FilesToDelete.Add(file);
                fileItem.RemoveFromHierarchy();
            });

            m_FileScrollView.Add(fileItem);
        }

        void ClosePanel(IAsset asset)
        {
            HideElement(m_DatasetPanel);
            OnClosePanel?.Invoke(asset);
        }

        void BrowseFile(ClickEvent evt)
        {
            _ = BrowseFileAsync();
        }

        async Task BrowseFileAsync()
        {
            if (m_CurrentAssetCreation == null && m_CurrentDatasetCreation == null && m_CurrentDataset == null)
                return;

            if (string.IsNullOrEmpty(m_LastOpenedFolder))
            {
                m_LastOpenedFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            var dialogResult = await GetFilePath();
            if (dialogResult.IsConfirmed)
            {
                var filePath = dialogResult.Content;

                if (File.Exists(filePath))
                {
                    m_LastOpenedFolder = Path.GetDirectoryName(filePath);

                    var fileName = Path.GetFileName(filePath);
                    var fileInfo = new FileInfo(filePath);

                    m_FilesToCreate.Add(filePath);

                    var fileItem = m_FileListItemTemplate.Instantiate();
                    fileItem.Q<Label>("FileNameLabel").text = fileName;
                    fileItem.Q<Label>("FileSizeLabel").text = GetSizeAsUserFriendlyFormat(fileInfo.Length);

                    var deleteButton = fileItem.Q<VisualElement>("DeleteIcon");
                    deleteButton.style.display = DisplayStyle.Flex;
                    deleteButton.RegisterCallback<ClickEvent>(_ =>
                    {
                        m_FilesToCreate.Remove(filePath);
                        fileItem.RemoveFromHierarchy();

                        if (m_FilesToCreate.Count == 0)
                            HideElement(m_CreateAssetButton);
                    });

                    m_FileScrollView.Add(fileItem);

                    // Creation Mode
                    if (m_CurrentAssetCreation != null && m_CurrentDatasetCreation != null)
                    {
                        DisplayElement(m_CreateAssetButton);
                    }
                }
                else
                {
                    m_MessageDialogController.OpenDialog("The file specified does not exist.");
                }
            }
        }

        const string k_AddFileHeader = "Input path to the file:";

        Task<IDialogResult<string>> GetFilePath()
        {
#if UNITY_EDITOR
            string path = UnityEditor.EditorUtility.OpenFilePanel(k_AddFileHeader, m_LastOpenedFolder, "");
            if (path.Length != 0)
            {
                return Task.FromResult(Result.From(path));
            }

            return Task.FromResult(Result.Cancelled<string>());
#else
            return m_AssetFilePathDialogController.OpenDialogAsync((k_AddFileHeader, m_LastOpenedFolder));
#endif
        }

        void CreateAsset(ClickEvent evt)
        {
            ChangeButtonEnabledState(false);

            _ = CreateAssetAsync();
        }

        async Task CreateAssetAsync()
        {
            if (m_AssetProject == null || m_CurrentAssetCreation == null || m_CurrentDatasetCreation == null || m_FilesToCreate.Count == 0)
            {
                ChangeButtonEnabledState(true);
                return;
            }

            if (m_FilesToCreate.Count == 1)
            {
                var assetPath = m_FilesToCreate.First();

                m_CurrentAssetCreation.Name = Path.GetFileNameWithoutExtension(assetPath);
                m_CurrentAssetCreation.Type = GetAssetType(assetPath);
            }
            else
            {
                m_CurrentAssetCreation.Type = AssetType.Other;
            }

            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var createdAsset = await m_AssetProject.CreateAssetAsync(m_CurrentAssetCreation, cancellationTokenSource.Token);
                if (createdAsset == null)
                {
                    m_MessageDialogController.OpenDialog($"Failed to create asset: {m_CurrentAssetCreation.Name}");
                }
                else
                {
                    await OnAssetCreated(createdAsset);
                }
            }
            catch (TaskCanceledException)
            {
                m_MessageDialogController.OpenDialog("Failed to create asset: Request timed out.");

                // Restore Creation UI
                CreateNewAssetAndDataset(m_AssetProject);
            }
            catch (Exception e)
            {
                m_MessageDialogController.OpenDialog($"Failed to create asset: {e.Message}");

                // Restore Creation UI
                CreateNewAssetAndDataset(m_AssetProject);
            }
        }

        async Task OnAssetCreated(IAsset createdAsset)
        {
            var datasets = new List<IDataset>();
            await foreach(var dataset in createdAsset.ListDatasetsAsync(Range.All, CancellationToken.None))
            {
                datasets.Add(dataset);
            }

            var sourceDataset = datasets.FirstOrDefault();
            if (sourceDataset == null)
            {
                Debug.LogError($"No datasets found for created asset {createdAsset.Name}.");
            }

            // Upload files
            foreach (var file in m_FilesToCreate)
            {
                var assetFile = await UploadAssetFileAsync(sourceDataset, file);
                if (assetFile == null)
                {
                    // Restore Creation UI
                    CreateNewAssetAndDataset(m_AssetProject);
                    return;
                }
            }

            m_FilesToCreate.Clear();
            m_CurrentDataset = sourceDataset;
            m_DatasetUpdate = new DatasetUpdate(m_CurrentDataset);

            // Restore create asset button state
            ChangeButtonEnabledState(true);

            // Open created asset to update
            ClosePanel(createdAsset);
        }

        async Task<IFile> UploadAssetFileAsync(IDataset dataset, string filePath)
        {
            var assetFileType = GetAssetType(filePath);

            var fileCreation = new FileCreation
            {
                Path = Path.GetFileName(filePath),
                Description = "",
                Tags = GetAssetFileTags(assetFileType)
            };

            IFile assetFile;

            try
            {
                var cancellationTokenSource = new CancellationTokenSource();

                var fileStream = File.OpenRead(filePath);
                assetFile = await dataset.UploadFileAsync(fileCreation, fileStream, null, cancellationTokenSource.Token);
            }
            catch (Exception e)
            {
                m_MessageDialogController.OpenDialog($"Failed to upload file: {filePath}, an exception occurs: {e.Message}");
                assetFile = null;
            }

            return assetFile;
        }

        void UpdateDataset(ClickEvent evt)
        {
            ChangeButtonEnabledState(false);

            _ = UpdateDatasetAsync();
        }

        void ChangeButtonEnabledState(bool state)
        {
            m_FileUploadButton.SetEnabled(state);
            m_CreateAssetButton.SetEnabled(state);
            m_BackButton.SetEnabled(state);
            m_SaveDatasetButton.SetEnabled(state);
        }

        static List<string> GetAssetFileTags(AssetType assetFileType)
        {
            return new List<string> { assetFileType.GetValueAsString() };
        }

        static AssetType GetAssetType(string assetPath)
        {
            var assetExtension = Path.GetExtension(assetPath).ToLower();
            switch (assetExtension)
            {
                case ".mat":
                    return AssetType.Material;
                case ".prefab":
                case ".fbx":
                    return AssetType.Model_3D;
                case ".unity":
                case ".shader":
                    return AssetType.Other;
            }

            return AssetType.Other;
        }

        void UpdateStatus()
        {
            if (m_CurrentAsset == null || m_CurrentDataset == null)
                return;

            m_DatasetStatusNameLabel.text = m_CurrentDataset.Status;

            m_DatasetStatusCircle.style.unityBackgroundImageTintColor = m_CurrentAsset.Status switch
            {
                "Committed" => new Color(0.74f, 0.94f, 0.71f, 1f),
                "Uncommitted" => new Color(0.86f, 0.60f, 0.27f, 1f),
                _ => m_DatasetStatusCircle.style.unityBackgroundImageTintColor
            };
        }

        void AddTags(FocusInEvent evt)
        {
            // if on asset tags text field and press enter, call "add new tag" (if not empty)
            if (Input.GetKey(KeyCode.Return) && string.IsNullOrEmpty(m_DatasetTagsField.value))
            {
                var tags = m_DatasetTagsField.value.Split(',');
                foreach (var tag in tags)
                {
                    m_DatasetUpdate.Tags.Add(tag);
                    AddTag(tag);
                }

                // clear the text field
                m_DatasetTagsField.value = "";
            }
        }

        static string GetSizeAsUserFriendlyFormat(long fileSize)
        {
            string[] sizes = {"B", "KB", "MB", "GB", "TB"};
            double len = fileSize;

            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }

        static void DisplayElement(VisualElement element)
        {
            if (element == null)
                return;

            element.style.display = DisplayStyle.Flex;
        }

        static void HideElement(VisualElement element)
        {
            if (element == null)
                return;

            element.style.display = DisplayStyle.None;
        }
    }
}
#endif
