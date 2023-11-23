using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetManager
{
    public class AssetCreationController
    {
        static readonly List<string> k_AssetTypes = AssetTypeExtensions.AssetTypeList();
        const string k_PublishedStatus = "Published";

        VisualTreeAsset m_DatasetListItemTemplate;
        VisualTreeAsset m_AssetTagsTemplate;
        VisualElement m_RightPanel;
        VisualElement m_AssetTagsContainer;
        VisualElement m_AssetStatusCircle;
        VisualElement m_AssetLastEdit;
        VisualElement m_AssetCreationPanel;

        DropdownField m_AssetTypeDropdown;
        Label m_AssetStatusNameLabel;
        Label m_AssetStatusLastEditLabel;
        Label m_AssetTitleLabel;
        TextField m_AssetNameField;
        TextField m_AssetTagsField;
        TextField m_AssetDescriptionField;
        Button m_AssetPublishButton;
        Button m_AssetSaveButton;
        Button m_CreateDatasetButton;
        Button m_BackButton;

        IAsset m_CurrentAsset;
        ScrollView m_DatasetScrollView;

        MessageDialogController m_MessageDialogController;
        IAssetUpdate m_AssetUpdate;

        public delegate Task<IAsset> GetRefreshedAsset(IAsset asset);

        GetRefreshedAsset m_GetRefreshedAsset;

        DatasetCreationController m_DatasetCreationController;

        internal void Init(VisualElement assetCreationPanel, DatasetCreationController datasetCreationController, VisualTreeAsset datasetListItemTemplate, VisualTreeAsset tagsTemplate, GetRefreshedAsset getRefreshedAsset, IDialogController dialogController)
        {
            m_GetRefreshedAsset = getRefreshedAsset;
            m_DatasetCreationController = datasetCreationController;
            m_DatasetCreationController.OnClosePanel += OnDatasetPanelClose;
            m_AssetCreationPanel = assetCreationPanel;
            m_DatasetListItemTemplate = datasetListItemTemplate;
            m_AssetTagsTemplate = tagsTemplate;
            m_MessageDialogController = (MessageDialogController) dialogController;

            m_DatasetScrollView = assetCreationPanel.Q<ScrollView>("DatasetScrollView");
            m_RightPanel = assetCreationPanel.Q<VisualElement>("RightPanel");
            m_AssetTagsContainer = assetCreationPanel.Q<VisualElement>("AssetTagsChipContainer");
            m_AssetStatusCircle = assetCreationPanel.Q<VisualElement>("StatusCircle");
            m_AssetLastEdit = assetCreationPanel.Q<VisualElement>("AssetLastEdit");
            m_AssetTypeDropdown = assetCreationPanel.Q<DropdownField>("AssetTypeDropdown");
            m_AssetStatusNameLabel = assetCreationPanel.Q<Label>("StatusNameLabel");
            m_AssetStatusLastEditLabel = assetCreationPanel.Q<Label>("AssetLastEditDateLabel");
            m_AssetTitleLabel = assetCreationPanel.Q<Label>("AssetTitleLabel");

            m_AssetNameField = assetCreationPanel.Q<TextField>("AssetNameField");
            m_AssetTagsField = assetCreationPanel.Q<TextField>("AssetTagsField");
            m_AssetDescriptionField = assetCreationPanel.Q<TextField>("AssetDescriptionField");
            m_AssetPublishButton = assetCreationPanel.Q<Button>("AssetPublishButton");
            m_AssetPublishButton.visible = false;
            m_AssetSaveButton = assetCreationPanel.Q<Button>("AssetSaveButton");
            m_AssetSaveButton.visible = false;
            m_CreateDatasetButton = assetCreationPanel.Q<Button>("CreateDatasetButton");
            m_CreateDatasetButton.visible = false;

            m_BackButton = assetCreationPanel.Q<Button>("BackBtn");

            m_AssetTypeDropdown.choices = k_AssetTypes;

            // Call backs------------------------------------------------------------
            m_AssetTypeDropdown.RegisterValueChangedCallback(evt =>
            {
                var assetTypeNewValue = evt.newValue.GetAssetTypeFromString();
                if (m_AssetUpdate != null && m_AssetUpdate.Type != assetTypeNewValue)
                    m_AssetUpdate.Type = assetTypeNewValue;
            });

            m_AssetNameField.RegisterValueChangedCallback(evt =>
            {
                if (m_AssetUpdate != null && m_AssetUpdate.Name != evt.newValue)
                    m_AssetUpdate.Name = evt.newValue;
            });

            m_AssetDescriptionField.RegisterValueChangedCallback(evt =>
            {
                if (m_AssetUpdate != null && m_AssetUpdate.Description != evt.newValue)
                    m_AssetUpdate.Description = evt.newValue;
            });

            m_AssetPublishButton.RegisterCallback<ClickEvent>(_ => PublishAsset());
            m_AssetSaveButton.RegisterCallback<ClickEvent>(_ => UpdateAssetInformation());
            m_CreateDatasetButton.RegisterCallback<ClickEvent>(_ => CreateNewDataset());

            // if on asset tags text field and press enter, call "add new tag" (if not empty)
            m_AssetTagsField.RegisterCallback<FocusInEvent>(_ =>
            {
                if (Input.GetKey(KeyCode.Return) && m_AssetTagsField.value != "")
                {
                    m_AssetUpdate.Tags.Add(m_AssetTagsField.value);
                    AddTag(m_AssetTagsField.value);

                    // clear the text field
                    m_AssetTagsField.value = "";
                }
            });
        }

        internal void OpenAsset(IAsset asset)
        {
            DisplayElement(m_RightPanel);

            ClearAssetInformation();

            m_CurrentAsset = asset;
            m_AssetUpdate = new AssetUpdate(asset);

            DisplayElement(m_AssetLastEdit);

            m_AssetTitleLabel.text = asset.Name;
            m_AssetStatusLastEditLabel.text = asset.AuthoringInfo.Updated.ToString("MMM dd, yyyy h:mm tt GMT");
            m_AssetTypeDropdown.value = asset.Type.GetValueAsString();
            m_AssetNameField.value = asset.Name;
            m_AssetDescriptionField.value = asset.Description;

            DrawTags(m_AssetUpdate.Tags);
            _ = ListDatasets(asset);

            UpdateStatus();
        }

        internal void CreateNewAsset(IAssetProject project)
        {
            HideElement(m_RightPanel);

            ClearAssetInformation();

            var assetProject = project;

            m_DatasetCreationController.CreateNewAssetAndDataset(assetProject);
        }

        void DrawTags(List<string> tagsList)
        {
            if (tagsList == null || tagsList.Count == 0) return;

            foreach (var tag in tagsList)
            {
                AddTag(tag);
            }
        }

        void AddTag(string tag)
        {
            var chip = m_AssetTagsTemplate.Instantiate();
            chip.Q<Label>().text = tag;
            m_AssetTagsContainer.Add(chip);
            chip.Q<Button>().clicked += () =>
            {
                m_AssetUpdate.Tags.Remove(tag);
                chip.RemoveFromHierarchy();
            };
        }

        CancellationTokenSource m_GetDatasetsCancellationTokenSource;

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
                OpenDatasetView(dataset);
            });
            m_DatasetScrollView.Add(datasetItem);
        }

        void OpenDatasetView(IDataset dataset)
        {
            HideElement(m_AssetCreationPanel);
            m_DatasetCreationController.OpenDataset(m_CurrentAsset, dataset);
        }

        void OnDatasetPanelClose(IAsset asset)
        {
            DisplayElement(m_AssetCreationPanel);

            if (asset != null)
            {
                _ = RefreshedAsset(asset);
            }
        }

        async Task RefreshedAsset(IAsset asset)
        {
            var refreshedAsset = await m_GetRefreshedAsset(asset);
            if (refreshedAsset != null)
            {
                OpenAsset(refreshedAsset);
            }
        }

        void ClearAssetInformation()
        {
            m_CurrentAsset = null;
            m_AssetTitleLabel.text = "AssetName";
            m_AssetStatusLastEditLabel.text = "";
            m_AssetTypeDropdown.value = "";
            m_AssetNameField.value = "";
            m_AssetDescriptionField.value = "";
            m_AssetTagsField.value = "";
            m_AssetTagsContainer.Clear();
            m_DatasetScrollView.Clear();
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

            await m_DatasetCreationController.UpdateDatasetAsync();

            var cancellationTokenSource = new CancellationTokenSource();
            var updateTask = m_CurrentAsset.UpdateAsync(m_AssetUpdate, cancellationTokenSource.Token);

            await updateTask;

            ChangeButtonEnabledState(true);

            if (updateTask.Status == TaskStatus.RanToCompletion)
            {
                m_MessageDialogController.OpenDialog("The asset has been saved successfully.");
            }
            else
            {
                m_MessageDialogController.OpenDialog("An error occurs during the asset saving.");
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

                var updatedAsset = await m_GetRefreshedAsset(m_CurrentAsset);
                if (updatedAsset != null)
                {
                    m_CurrentAsset = updatedAsset;

                    UpdateStatus();
                }

                ChangeButtonEnabledState(true);
            }
            catch (Exception)
            {
                // Hide exception for now until we have a better way to handle it.
                // Invalid exception append on SendAssetToReview even if at the end the execution is done completely.
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
