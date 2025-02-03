using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetManager
{
    public class DatasetPanelController
    {
        StatusController m_StatusController;

        VisualTreeAsset m_DatasetTagsTemplate;
        VisualElement m_LeftPanel;
        VisualElement m_RightPanel;
        VisualElement m_DatasetTagsContainer;

        TextField m_DatasetNameField;
        TextField m_DatasetTagsField;
        TextField m_DatasetDescriptionField;
        Toggle m_DatasetVisibleToggle;

        Button m_SaveDatasetButton;
        Button m_GeneratePreviewButton;
        Button m_BackButton;

        IDataset m_CurrentDataset;
        DatasetProperties m_CurrentDatasetProperties;
        DatasetUpdate m_DatasetUpdate;
        FileController m_FileController;
        TransformationController m_TransformationController;
        MetadataController m_MetadataController;

        public event Action<IAsset> PanelClosed;

        public void Init(VisualElement datasetPanel, VisualTreeAsset tagsTemplate, FileController fileController, AddMetadataPopupController addMetadataPopupController)
        {
            m_DatasetTagsTemplate = tagsTemplate;

            m_LeftPanel = datasetPanel.Q("LeftPanel");
            m_RightPanel = datasetPanel.Q("RightPanel");

            m_DatasetNameField = datasetPanel.Q<TextField>("DatasetNameField");

            m_StatusController = new StatusController(datasetPanel);

            var scrollView = m_RightPanel.Q<ScrollView>("DatasetInfo");
            var content = m_RightPanel.Q("Content");
            scrollView.Add(content);

            m_DatasetDescriptionField = datasetPanel.Q<TextField>("DatasetDescriptionField");

            m_DatasetTagsContainer = datasetPanel.Q("DatasetTagsChipContainer");
            m_DatasetTagsField = datasetPanel.Q<TextField>("DatasetTagsField");

            m_DatasetVisibleToggle = datasetPanel.Q<Toggle>("DatasetVisibleToggle");

            var metadataTemplate = datasetPanel.Q<TemplateContainer>("MetadataItemTemplate");

            var metadataContainer = datasetPanel.Q("MetadataContainer");
            m_MetadataController = new MetadataController(metadataContainer, metadataTemplate.templateSource, addMetadataPopupController);

            m_SaveDatasetButton = datasetPanel.Q<Button>("DatasetSaveButton");
            m_GeneratePreviewButton = datasetPanel.Q<Button>("DatasetGeneratePreviewButton");

            m_FileController = fileController;
            m_TransformationController = new TransformationController(datasetPanel);

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
        }

        public void Cleanup()
        {
            m_SaveDatasetButton.UnregisterCallback<ClickEvent>(UpdateDataset);
            m_DatasetTagsField.UnregisterCallback<FocusInEvent>(AddTags);
        }

        public void OpenDataset(IDataset dataset, bool canUpdate)
        {
            ClearInformation();

            m_RightPanel?.Show();
            if (m_LeftPanel != null) m_LeftPanel.style.flexGrow = 0;

            m_CurrentDataset = dataset;
            m_DatasetUpdate = new DatasetUpdate();

            m_SaveDatasetButton.SetEnabled(canUpdate);
            m_GeneratePreviewButton.SetEnabled(canUpdate);
            m_DatasetTagsField.style.display = canUpdate ? DisplayStyle.Flex : DisplayStyle.None;

            m_DatasetNameField.SetEnabled(canUpdate);
            m_DatasetDescriptionField.SetEnabled(canUpdate);
            m_DatasetVisibleToggle.SetEnabled(canUpdate);

            _ = PopulateAsync(dataset, canUpdate, default);

            _ = m_FileController.ListExistingFiles(dataset, canUpdate);
            _ = m_MetadataController.PopulateMetadataAsync(dataset, canUpdate);
            _ = m_TransformationController.PopulateTransformationProgress(dataset);
        }

        public void Clear()
        {
            m_RightPanel?.Hide();
            if (m_LeftPanel != null) m_LeftPanel.style.flexGrow = 1;

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

            var updateTasks = new List<Task>
            {
                m_CurrentDataset.UpdateAsync(m_DatasetUpdate, default),
                m_MetadataController.UpdateMetadataAsync(default),
            };

            try
            {
                await Task.WhenAll(updateTasks);

                if (updateTasks.TrueForAll(x => x.IsCompletedSuccessfully))
                    DialogService.ShowMessage("Update complete", "The dataset has been saved successfully.");

                await m_CurrentDataset.RefreshAsync(default);

                OpenDataset(m_CurrentDataset, true);
            }
            catch (Exception e)
            {
                e.LogException();
                DialogService.ShowMessage(e, "Update failed", $"Failed to update dataset with reason: {e.Message}");
            }
            finally
            {
                ChangeButtonEnabledState(true);
            }
        }

        public void OnAssetCreated(IAsset createdAsset, IDataset sourceDataset)
        {
            m_CurrentDataset = sourceDataset;
            m_DatasetUpdate = new DatasetUpdate();

            ClosePanel(createdAsset);
        }

        public void ChangeButtonEnabledState(bool state)
        {
            m_FileController.SetFileUploadEnabled(state);
            m_BackButton.SetEnabled(state);
            m_SaveDatasetButton.SetEnabled(state);
            m_GeneratePreviewButton.SetEnabled(state);
        }

        async Task PopulateAsync(IDataset dataset, bool canUpdate, CancellationToken token)
        {
            m_CurrentDatasetProperties = await dataset.GetPropertiesAsync(token);

            if (token.IsCancellationRequested) return;

            m_DatasetNameField.SetValueWithoutNotify(m_CurrentDatasetProperties.Name);
            m_DatasetDescriptionField.SetValueWithoutNotify(m_CurrentDatasetProperties.Description);
            m_DatasetVisibleToggle.SetValueWithoutNotify(m_CurrentDatasetProperties.IsVisible);

            UpdateStatus();

            Action<string> addTagAction = tag => AddTag(tag, canUpdate);
            addTagAction.AddTags(GetUpdateTags());
        }

        void ClearInformation()
        {
            m_CurrentDataset = null;
            m_DatasetUpdate = null;
            m_StatusController.Clear();
            m_DatasetNameField.SetValueWithoutNotify("");
            m_DatasetDescriptionField.SetValueWithoutNotify("");
            m_DatasetTagsField.SetValueWithoutNotify("");
            m_DatasetTagsContainer.Clear();
            m_DatasetVisibleToggle.SetValueWithoutNotify(false);
            m_FileController.Clear();
            m_MetadataController.Clear();
            m_TransformationController.Clear();
        }

        void ClosePanel(IAsset asset)
        {
            m_MetadataController.Hide();

            PanelClosed?.Invoke(asset);
        }

        void UpdateDataset(ClickEvent evt)
        {
            ChangeButtonEnabledState(false);

            _ = UpdateDatasetAsync();
        }

        void UpdateStatus()
        {
            m_StatusController.Update(m_CurrentDatasetProperties.StatusName, m_CurrentDatasetProperties.AuthoringInfo?.Updated);
            m_StatusController.SetStatusColor(m_CurrentDatasetProperties.StatusName switch
            {
                "Committed" => new Color(0.07f, 0.65f, 0.58f, 1f),
                "Uncommitted" => new Color(0.86f, 0.60f, 0.27f, 1f),
                _ => Color.grey
            });
        }

        void AddTags(FocusInEvent evt)
        {
            m_DatasetTagsField.ParseTags(GetUpdateTags(), tag => AddTag(tag, true));
        }

        void AddTag(string tag, bool canRemove)
        {
            m_DatasetTagsContainer.AddTag(tag, GetUpdateTags(), m_DatasetTagsTemplate, canRemove);
        }

        List<string> GetUpdateTags()
        {
            return m_DatasetUpdate.Tags ?? (m_DatasetUpdate.Tags = m_CurrentDatasetProperties.Tags?.ToList() ?? new List<string>());
        }

        void GenerateThumbnailPreview(ClickEvent evt)
        {
            ChangeButtonEnabledState(false);

            _ = StartTransformationOnDataset(new ThumbnailGeneratorTransformation());
        }

        async Task StartTransformationOnDataset(ITransformationCreation transformationCreation)
        {
            if (m_CurrentDataset == null)
            {
                ChangeButtonEnabledState(true);
                return;
            }

            var transformationName = transformationCreation.WorkflowType == WorkflowType.Custom ? transformationCreation.CustomWorkflowName : transformationCreation.WorkflowType.ToString();

            try
            {
                var transformation = await m_CurrentDataset.StartTransformationAsync(transformationCreation, default);
                m_TransformationController.AddTransformationProgress(transformation);
            }
            catch (OperationCanceledException oe)
            {
                oe.LogException();
                DialogService.ShowMessage("Cancelled", $"Failed to start transformation {transformationName}. Request cancelled.");
            }
            catch (Exception e)
            {
                e.LogException();
                DialogService.ShowMessage(e, "Start transformation failed", $"Failed to start transformation of type {transformationName} with reason: {e.Message}");
            }
            finally
            {
                ChangeButtonEnabledState(true);
            }
        }
    }
}
