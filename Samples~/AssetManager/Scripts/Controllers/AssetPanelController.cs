using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetManager
{
    public class AssetPanelController
    {
        const string k_PublishedStatus = "Published";

        VisualTreeAsset m_DatasetListItemTemplate;

        VisualTreeAsset m_AssetTagsTemplate;

        VisualElement m_RightPanel;
        VisualElement m_AssetTagsContainer;
        VisualElement m_AssetStatusCircle;
        VisualElement m_AssetLastEdit;
        EnumField m_AssetTypeDropdown;
        Label m_AssetStatusNameLabel;
        Label m_AssetStatusLastEditLabel;
        TextField m_AssetNameField;
        TextField m_AssetTagsField;
        TextField m_AssetDescriptionField;
        Button m_AssetPublishButton;
        Button m_AssetSaveButton;
        Button m_CreateDatasetButton;
        Button m_BackButton;
        ScrollView m_DatasetScrollView;

        IAsset m_CurrentAsset;
        IAssetUpdate m_AssetUpdate;
        MetadataController m_MetadataController;
        MetadataController m_SystemMetadataController;

        CancellationTokenSource m_GetDatasetsCancellationTokenSource;

        public event Action<IDataset> OnDatasetOpen;
        public event Action<IAsset> OnAssetUpdated;
        public Func<Task> PrepareAssetUpdateAsync { get; set; }

        public IAsset CurrentAsset => m_CurrentAsset;

        public void Init(VisualElement assetCreationPanel, VisualTreeAsset datasetListItemTemplate, VisualTreeAsset tagsTemplate, AddMetadataPopupController addMetadataPopup)
        {
            m_DatasetListItemTemplate = datasetListItemTemplate;
            m_AssetTagsTemplate = tagsTemplate;

            m_DatasetScrollView = assetCreationPanel.Q<ScrollView>("DatasetScrollView");

            m_RightPanel = assetCreationPanel.Q("RightPanel");

            m_AssetNameField = assetCreationPanel.Q<TextField>("AssetNameField");

            m_AssetStatusCircle = assetCreationPanel.Q("StatusCircle");
            m_AssetStatusNameLabel = assetCreationPanel.Q<Label>("StatusNameLabel");

            m_AssetLastEdit = assetCreationPanel.Q("LastEdit");
            m_AssetStatusLastEditLabel = assetCreationPanel.Q<Label>("LastEditDate");

            var scrollView = m_RightPanel.Q<ScrollView>();
            var content = m_RightPanel.Q("Content");
            scrollView.Add(content);

            m_AssetDescriptionField = assetCreationPanel.Q<TextField>("AssetDescriptionField");
            m_AssetTypeDropdown = assetCreationPanel.Q<EnumField>("AssetTypeDropdown");
            m_AssetTagsField = assetCreationPanel.Q<TextField>("AssetTagsField");
            m_AssetTagsContainer = assetCreationPanel.Q("AssetTagsChipContainer");

            var metadataTemplate = assetCreationPanel.Q<TemplateContainer>("MetadataItemTemplate");

            var metadataContainer = assetCreationPanel.Q("MetadataContainer");
            m_MetadataController = new MetadataController(metadataContainer, false, metadataTemplate.templateSource, addMetadataPopup);

            metadataContainer = assetCreationPanel.Q("SystemMetadataContainer");
            m_SystemMetadataController = new MetadataController(metadataContainer, true, metadataTemplate.templateSource, addMetadataPopup);

            m_AssetPublishButton = assetCreationPanel.Q<Button>("AssetPublishButton");
            m_AssetPublishButton.visible = false;
            m_AssetSaveButton = assetCreationPanel.Q<Button>("AssetSaveButton");
            m_AssetSaveButton.visible = false;
            m_CreateDatasetButton = assetCreationPanel.Q<Button>("CreateDatasetButton");
            m_CreateDatasetButton.visible = false;

            m_BackButton = assetCreationPanel.Q<Button>("BackBtn");

            // Call backs------------------------------------------------------------
            m_AssetTypeDropdown.RegisterValueChangedCallback(evt =>
            {
                if (m_AssetUpdate != null)
                    m_AssetUpdate.Type = (AssetType) (evt.newValue ?? AssetType.Other);
            });

            m_AssetNameField.RegisterValueChangedCallback(evt =>
            {
                if (m_AssetUpdate != null)
                    m_AssetUpdate.Name = evt.newValue;
            });

            m_AssetDescriptionField.RegisterValueChangedCallback(evt =>
            {
                if (m_AssetUpdate != null)
                    m_AssetUpdate.Description = evt.newValue;
            });

            m_AssetPublishButton.RegisterCallback<ClickEvent>(_ => PublishAsset());
            m_AssetSaveButton.RegisterCallback<ClickEvent>(_ => UpdateAssetInformation());
            m_CreateDatasetButton.RegisterCallback<ClickEvent>(_ => CreateNewDataset());
            m_AssetTagsField.RegisterCallback<FocusInEvent>(AddTags);
        }

        public void OpenAsset(IAsset asset)
        {
            ClearAssetInformation();

            m_RightPanel?.Show();

            m_CurrentAsset = asset;
            m_AssetUpdate = new AssetUpdate(asset);

            m_AssetLastEdit?.Show();

            m_AssetNameField.SetValueWithoutNotify(asset.Name);
            m_AssetStatusLastEditLabel.text = asset.AuthoringInfo.Updated.ToString("MMM dd, yyyy h:mm tt GMT");
            m_AssetTypeDropdown.SetValueWithoutNotify(asset.Type);
            m_AssetDescriptionField.SetValueWithoutNotify(asset.Description);

            ((Action<string>) AddTag).AddTags(m_AssetUpdate.Tags);

            _ = ListDatasets(asset);

            UpdateStatus();

            _ = m_MetadataController.PopulateMetadataAsync(asset);
            _ = m_SystemMetadataController.PopulateMetadataAsync(asset);
        }

        public void Clear()
        {
            m_RightPanel?.Hide();

            ClearAssetInformation();
        }

        async Task ListDatasets(IAsset asset)
        {
            if (m_GetDatasetsCancellationTokenSource != null)
            {
                m_GetDatasetsCancellationTokenSource.Cancel();
                m_GetDatasetsCancellationTokenSource.Dispose();
            }

            m_GetDatasetsCancellationTokenSource = new CancellationTokenSource();

            await foreach (var dataset in asset.ListDatasetsAsync(Range.All, m_GetDatasetsCancellationTokenSource.Token))
            {
                AddDatasetRow(dataset);
            }
        }

        void AddDatasetRow(IDataset dataset)
        {
            var datasetItem = m_DatasetListItemTemplate.Instantiate();
            datasetItem.Q<Label>("DatasetNameLabel").text = dataset.Name;
            datasetItem.Q<Label>("DatasetDescriptionLabel").text = dataset.Description;
            datasetItem.RegisterCallback<ClickEvent>(_ =>
            {
                OnDatasetOpen?.Invoke(dataset);
            });
            m_DatasetScrollView.Add(datasetItem);
        }

        void ClearAssetInformation()
        {
            m_CurrentAsset = null;
            m_AssetNameField.SetValueWithoutNotify("");
            m_AssetStatusLastEditLabel.text = "";
            m_AssetTypeDropdown.SetValueWithoutNotify(default);
            m_AssetDescriptionField.SetValueWithoutNotify("");
            m_AssetTagsField.SetValueWithoutNotify("");
            m_AssetTagsContainer.Clear();
            m_DatasetScrollView.Clear();
            m_MetadataController.Clear();
            m_SystemMetadataController.Clear();
        }

        void UpdateAssetInformation()
        {
            ChangeButtonEnabledState(false);

            _ = UpdateAssetInformationAsync();
        }

        async Task UpdateAssetInformationAsync()
        {
            if (m_CurrentAsset == null)
            {
                ChangeButtonEnabledState(true);
                return;
            }

            if (PrepareAssetUpdateAsync != null) await PrepareAssetUpdateAsync.Invoke();

            var cancellationTokenSource = new CancellationTokenSource();
            var updateTasks = new List<Task>
            {
                m_CurrentAsset.UpdateAsync(m_AssetUpdate, cancellationTokenSource.Token),
                m_MetadataController.UpdateMetadataAsync(cancellationTokenSource.Token),
                m_SystemMetadataController.UpdateMetadataAsync(cancellationTokenSource.Token)
            };

            try
            {
                await Task.WhenAll(updateTasks);

                DialogService.ShowMessage("Success", "The asset has been saved successfully.");

                OnAssetUpdated?.Invoke(m_CurrentAsset);
            }
            catch (Exception e)
            {
                e.LogException();
                DialogService.ShowMessage("Error", $"An error occured while saving the asset.");
            }
            finally
            {
                ChangeButtonEnabledState(true);
            }
        }

        void PublishAsset()
        {
            ChangeButtonEnabledState(false);

            _ = PublishAssetAsync();
        }

        async Task PublishAssetAsync()
        {
            if (m_CurrentAsset == null)
            {
                ChangeButtonEnabledState(true);
                return;
            }

            try
            {
                var cancellationTokenSource = new CancellationTokenSource();

                // Successful publishing workflow
                //Draft -> Review -> Approved -> Published
                switch (m_CurrentAsset.Status)
                {
                    case "Draft":
                        await m_CurrentAsset.SendToReviewAsync(cancellationTokenSource.Token);
                        await m_CurrentAsset.ApproveAsync(cancellationTokenSource.Token);
                        await m_CurrentAsset.PublishAsync(cancellationTokenSource.Token);
                        break;
                    case "Ingestion": // Status when asset is in review
                        await m_CurrentAsset.ApproveAsync(cancellationTokenSource.Token);
                        await m_CurrentAsset.PublishAsync(cancellationTokenSource.Token);
                        break;
                    case "Approved":
                        await m_CurrentAsset.PublishAsync(cancellationTokenSource.Token);
                        break;
                }

                // Asset will have refreshed after publishing
                m_AssetUpdate = new AssetUpdate(m_CurrentAsset);

                OpenAsset(m_CurrentAsset);
                OnAssetUpdated?.Invoke(m_CurrentAsset);

                ChangeButtonEnabledState(true);
            }
            catch (Exception)
            {
                // Hide exception for now until we have a better way to handle it.
                // Invalid exception can occur on SendAssetToReview even if the execution completes.
            }
        }

        void CreateNewDataset()
        {
            _ = CreateNewDatasetAsync();
        }

        async Task CreateNewDatasetAsync()
        {
            if (m_CurrentAsset == null)
                return;

            var cancellationTokenSource = new CancellationTokenSource();
            var dataset = await m_CurrentAsset.CreateDatasetAsync(new DatasetCreation($"Dataset_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}"), cancellationTokenSource.Token);
            if (dataset != null)
                AddDatasetRow(dataset);
        }

        void UpdateStatus()
        {
            m_AssetPublishButton.visible = false;
            m_AssetSaveButton.visible = false;
            m_CreateDatasetButton.visible = false;

            if (m_CurrentAsset == null)
                return;

            m_AssetStatusNameLabel.text = m_CurrentAsset.Status;

            // Successful publishing workflow
            //Draft -> Review -> Approved -> Published
            switch (m_CurrentAsset.Status)
            {
                case k_PublishedStatus:
                    m_AssetStatusCircle.style.unityBackgroundImageTintColor = new Color(0.74f, 0.94f, 0.71f, 1f);
                    break;
                case "Approved":
                    m_AssetStatusCircle.style.unityBackgroundImageTintColor = new Color(0.74f, 0.94f, 0.71f, 1f);
                    m_AssetPublishButton.visible = true;
                    break;
                case "Ingestion": // Status when asset is in review
                    m_AssetPublishButton.visible = true;
                    break;
                case "Withdrawn":
                    m_AssetStatusCircle.style.unityBackgroundImageTintColor = new Color(0.93f, 0.42f, 0.37f, 1f);
                    break;
                case "Draft":
                    m_AssetStatusCircle.style.unityBackgroundImageTintColor = new Color(0.86f, 0.60f, 0.27f, 1f);
                    m_AssetPublishButton.visible = true;
                    m_AssetSaveButton.visible = true;
                    m_CreateDatasetButton.visible = true;
                    break;
            }
        }

        void ChangeButtonEnabledState(bool state)
        {
            m_CreateDatasetButton.SetEnabled(state);
            m_AssetPublishButton.SetEnabled(state);
            m_AssetSaveButton.SetEnabled(state);
            m_BackButton.SetEnabled(state);
        }

        void AddTags(FocusInEvent evt)
        {
            m_AssetTagsField.ParseTags(m_AssetUpdate.Tags, AddTag);
        }

        void AddTag(string tag)
        {
            m_AssetTagsContainer.AddTag(tag, m_AssetUpdate.Tags, m_AssetTagsTemplate);
        }
    }
}
