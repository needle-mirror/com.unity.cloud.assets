using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetManager
{
    public class DatasetPanelController
    {
        VisualTreeAsset m_DatasetTagsTemplate;
        VisualElement m_RightPanel;
        VisualElement m_DatasetTagsContainer;
        VisualElement m_DatasetStatusCircle;
        VisualElement m_DatasetLastEdit;

        Label m_DatasetStatusNameLabel;
        Label m_DatasetStatusLastEditLabel;
        TextField m_DatasetNameField;
        TextField m_DatasetTagsField;
        TextField m_DatasetDescriptionField;
        Toggle m_DatasetVisibleToggle;

        Button m_SaveDatasetButton;
        Button m_GeneratePreviewButton;
        Button m_BackButton;

        IDataset m_CurrentDataset;
        DatasetUpdate m_DatasetUpdate;
        FileController m_FileController;
        MetadataController m_MetadataController;
        MetadataController m_SystemMetadataController;

        public event Action<IAsset> PanelClosed;

        public void Init(VisualElement datasetPanel, VisualTreeAsset tagsTemplate, FileController fileController, AddMetadataPopupController addMetadataPopupController)
        {
            m_DatasetTagsTemplate = tagsTemplate;

            m_RightPanel = datasetPanel.Q("RightPanel");

            m_DatasetNameField = datasetPanel.Q<TextField>("DatasetNameField");

            m_DatasetStatusCircle = datasetPanel.Q("StatusCircle");
            m_DatasetStatusNameLabel = datasetPanel.Q<Label>("StatusNameLabel");

            m_DatasetLastEdit = datasetPanel.Q("LastEdit");
            m_DatasetStatusLastEditLabel = datasetPanel.Q<Label>("LastEditDate");

            var scrollView = m_RightPanel.Q<ScrollView>();
            var content = m_RightPanel.Q("Content");
            scrollView.Add(content);

            m_DatasetDescriptionField = datasetPanel.Q<TextField>("DatasetDescriptionField");

            m_DatasetTagsContainer = datasetPanel.Q("DatasetTagsChipContainer");
            m_DatasetTagsField = datasetPanel.Q<TextField>("DatasetTagsField");

            m_DatasetVisibleToggle = datasetPanel.Q<Toggle>("DatasetVisibleToggle");

            var metadataTemplate = datasetPanel.Q<TemplateContainer>("MetadataItemTemplate");

            var metadataContainer = datasetPanel.Q("MetadataContainer");
            m_MetadataController = new MetadataController(metadataContainer, false, metadataTemplate.templateSource, addMetadataPopupController);

            metadataContainer = datasetPanel.Q("SystemMetadataContainer");
            m_SystemMetadataController = new MetadataController(metadataContainer, true, metadataTemplate.templateSource, addMetadataPopupController);

            m_SaveDatasetButton = datasetPanel.Q<Button>("DatasetSaveButton");
            m_GeneratePreviewButton = datasetPanel.Q<Button>("DatasetGeneratePreviewButton");

            // Call backs----------------------------------
            m_SaveDatasetButton.RegisterCallback<ClickEvent>(UpdateDataset);
            m_GeneratePreviewButton.RegisterCallback<ClickEvent>(GenerateThumbnailPreview);

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
            m_BackButton.RegisterCallback<ClickEvent>(_ => ClosePanel(null));

            m_FileController = fileController;
        }

        public void Cleanup()
        {
            m_SaveDatasetButton.UnregisterCallback<ClickEvent>(UpdateDataset);
            m_DatasetTagsField.UnregisterCallback<FocusInEvent>(AddTags);
        }

        public void OpenDataset(IDataset dataset)
        {
            ClearInformation();

            m_RightPanel?.Show();
            m_DatasetLastEdit?.Show();

            m_CurrentDataset = dataset;
            m_DatasetUpdate = new DatasetUpdate(m_CurrentDataset);
            m_DatasetNameField.SetValueWithoutNotify(dataset.Name);
            m_DatasetDescriptionField.SetValueWithoutNotify(dataset.Description);
            m_DatasetStatusLastEditLabel.text = dataset.AuthoringInfo?.Updated.ToString("MMM dd, yyyy h:mm tt GMT") ?? "unknown";
            m_DatasetVisibleToggle.SetValueWithoutNotify(dataset.IsVisible);

            ((Action<string>) AddTag).AddTags(m_DatasetUpdate.Tags);

            _ = m_FileController.ListExistingFiles(dataset);

            UpdateStatus(dataset.Status);

            _ = m_MetadataController.PopulateMetadataAsync(dataset);
            _ = m_SystemMetadataController.PopulateMetadataAsync(dataset);
        }

        public void Clear()
        {
            m_RightPanel?.Hide();

            ClearInformation();
        }

        public async Task UpdateDatasetAsync()
        {
            if (m_CurrentDataset == null)
            {
                ChangeButtonEnabledState(true);
                return;
            }

            await m_FileController.RemoveFiles();

            var didSucceed = await m_FileController.UploadFiles(m_CurrentDataset);
            if (!didSucceed)
            {
                ChangeButtonEnabledState(true);
                return;
            }

            var cancellationTokenSource = new CancellationTokenSource();
            var updateTasks = new List<Task>
            {
                m_CurrentDataset.UpdateAsync(m_DatasetUpdate, cancellationTokenSource.Token),
                m_MetadataController.UpdateMetadataAsync(cancellationTokenSource.Token),
                m_SystemMetadataController.UpdateMetadataAsync(cancellationTokenSource.Token)
            };

            try
            {
                await Task.WhenAll(updateTasks);

                DialogService.ShowMessage("Update complete", "The dataset has been saved successfully.");

                OpenDataset(m_CurrentDataset);
            }
            catch (Exception e)
            {
                e.LogException();
                DialogService.ShowMessage("Error", $"An error occurred while saving the dataset.");
            }
            finally
            {
                ChangeButtonEnabledState(true);
            }
        }

        async Task StartTransformationOnDataset(WorkflowType workflowType)
        {
            if (m_CurrentDataset == null)
            {
                ChangeButtonEnabledState(true);
                return;
            }

            try
            {
                var cancellationTokenSource = new CancellationTokenSource();

                var transformation =
                    await m_CurrentDataset.StartTransformationAsync(workflowType, cancellationTokenSource.Token);

                while (transformation.Status != TransformationStatus.Succeeded)
                {
                    await Task.Delay(1000, cancellationTokenSource.Token);

                    transformation = await m_CurrentDataset.GetTransformationAsync(transformation.Descriptor.TransformationId, cancellationTokenSource.Token);
                }

                DialogService.ShowMessage("Success", $"Transformation of type {workflowType} has been started.");
                ChangeButtonEnabledState(true);
            }
            catch (OperationCanceledException oe)
            {
                oe.LogException();
                DialogService.ShowMessage("Error", $"Failed to start transformation {workflowType}. Request canceled.");
            }
            catch (Exception e)
            {
                e.LogException();
                DialogService.ShowMessage("Error", $"Transformation of type {workflowType} failed.");
            }
        }

        public void OnAssetCreated(IAsset createdAsset, IDataset sourceDataset)
        {
            m_CurrentDataset = sourceDataset;
            m_DatasetUpdate = new DatasetUpdate(m_CurrentDataset);

            ClosePanel(createdAsset);
        }

        public void ChangeButtonEnabledState(bool state)
        {
            m_FileController.SetFileUploadEnabled(state);
            m_BackButton.SetEnabled(state);
            m_SaveDatasetButton.SetEnabled(state);
            m_GeneratePreviewButton.SetEnabled(state);
        }

        void ClearInformation()
        {
            m_CurrentDataset = null;
            m_DatasetUpdate = null;
            m_DatasetNameField.SetValueWithoutNotify("");
            m_DatasetDescriptionField.SetValueWithoutNotify("");
            m_DatasetTagsField.SetValueWithoutNotify("");
            m_DatasetStatusLastEditLabel.text = "";
            m_DatasetTagsContainer.Clear();
            m_DatasetVisibleToggle.SetValueWithoutNotify(false);
            m_FileController.Clear();
            m_MetadataController.Clear();
            m_SystemMetadataController.Clear();
        }

        void ClosePanel(IAsset asset)
        {
            m_MetadataController.Hide();
            m_SystemMetadataController.Hide();

            PanelClosed?.Invoke(asset);
        }

        void UpdateDataset(ClickEvent evt)
        {
            ChangeButtonEnabledState(false);

            _ = UpdateDatasetAsync();
        }

        void UpdateStatus(string status)
        {
            if (m_CurrentDataset == null)
                return;

            m_DatasetStatusNameLabel.text = string.IsNullOrEmpty(status) ? "Unknown" : status;

            m_DatasetStatusCircle.style.unityBackgroundImageTintColor = status switch
            {
                "Committed" => new Color(0.74f, 0.94f, 0.71f, 1f),
                "Uncommitted" => new Color(0.86f, 0.60f, 0.27f, 1f),
                _ => Color.grey
            };
        }

        void AddTags(FocusInEvent evt)
        {
            m_DatasetTagsField.ParseTags(m_DatasetUpdate.Tags, AddTag);
        }

        void AddTag(string tag)
        {
            m_DatasetTagsContainer.AddTag(tag, m_DatasetUpdate.Tags, m_DatasetTagsTemplate);
        }

        void GenerateThumbnailPreview(ClickEvent evt)
        {
            ChangeButtonEnabledState(false);

            _ = StartTransformationOnDataset(WorkflowType.Thumbnail_Generation);
        }
    }
}
