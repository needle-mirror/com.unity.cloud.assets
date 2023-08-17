#if !UC_EXCLUDE_SAMPLES

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetManager
{
    public class AssetCreationController
    {
        static readonly List<string> k_AssetTypes = new() {"Model", "Material", "Other"};

        VisualTreeAsset m_FileListItemTemplate;
        VisualTreeAsset m_AssetTagsTemplate;
        VisualElement m_AssetTagsContainer;
        VisualElement m_AssetFileList;
        VisualElement m_AssetStatusCircle;
        VisualElement m_AssetLastEdit;
        ScrollView m_AssetFileScrollView;
        DropdownField m_AssetTypeDropdown;
        Label m_AssetStatusNameLabel;
        Label m_AssetStatusLastEditLabel;
        Label m_AssetTitleLabel;
        TextField m_AssetNameField;
        TextField m_AssetTagsField;
        TextField m_AssetDescriptionField;
        Toggle m_AssetPublishToggle;
        Button m_AssetSaveButton;

        IAsset m_CurrentAsset;

        internal void Init(VisualElement root, VisualTreeAsset listItemTemplate, VisualTreeAsset tagsTemplate)
        {
            m_FileListItemTemplate = listItemTemplate;
            m_AssetTagsTemplate = tagsTemplate;

            m_AssetFileList = root.Q<VisualElement>("AssetFileList");
            m_AssetTagsContainer = root.Q<VisualElement>("AssetTagsChipContainer");
            m_AssetFileScrollView = root.Q<ScrollView>("AssetFileScrollView");
            m_AssetStatusCircle = root.Q<VisualElement>("StatusCircle");
            m_AssetLastEdit = root.Q<VisualElement>("AssetLastEdit");
            m_AssetTypeDropdown = root.Q<DropdownField>("AssetTypeDropdown");
            m_AssetStatusNameLabel = root.Q<Label>("StatusNameLabel");
            m_AssetStatusLastEditLabel = root.Q<Label>("AssetLastEditDateLabel");
            m_AssetTitleLabel = root.Q<Label>("AssetTitleLabel");
            m_AssetNameField = root.Q<TextField>("AssetNameField");
            m_AssetTagsField = root.Q<TextField>("AssetTagsField");
            m_AssetDescriptionField = root.Q<TextField>("AssetDescriptionField");
            m_AssetPublishToggle = root.Q<Toggle>("AssetPublishToggle");
            m_AssetSaveButton = root.Q<Button>("AssetSaveButton");

            m_AssetTypeDropdown.label = "Asset Type";
            m_AssetTypeDropdown.choices = k_AssetTypes;
            m_AssetNameField.label = "Asset Name";
            m_AssetTagsField.label = "Tags";
            m_AssetDescriptionField.label = "Description";

            m_AssetPublishToggle.value = true;
            m_AssetPublishToggle.RegisterValueChangedCallback(evt =>
            {
                m_AssetSaveButton.text = GetSaveButtonText();
            });

            m_AssetSaveButton.text = GetSaveButtonText();
            m_AssetSaveButton.RegisterCallback<ClickEvent>(_ => UpdateAssetInformation());

            // if on asset tags text field and press enter, call "add new tag" (if not empty)
            m_AssetTagsField.RegisterCallback<FocusInEvent>(evt =>
            {
                if (Input.GetKey(KeyCode.Return) && m_AssetTagsField.value != "")
                {
                    m_CurrentAsset.Tags.Add(m_AssetTagsField.value);
                    AddTag(m_AssetTagsField.value);
                }
            });
        }

        string GetSaveButtonText()
        {
            return m_AssetPublishToggle.value && !string.Equals(m_CurrentAsset?.Status, "Published") ? "Publish asset" : "Save asset";
        }

        internal void OpenExistingAsset(IAsset asset)
        {
            ClearAssetInformation();

            m_AssetLastEdit.style.display = DisplayStyle.Flex;

            m_AssetTitleLabel.text = asset.Name;
            m_AssetStatusLastEditLabel.text = asset.Updated.ToString("MMM dd, yyyy h:mm tt GMT");
            m_AssetTypeDropdown.value = asset.Type;
            m_AssetNameField.value = asset.Name;
            m_AssetDescriptionField.value = asset.Description;

            AddExistingTags(asset.Tags);
            AddExistingFiles(asset.Files);

            UpdateStatus(asset.Status);

            m_AssetSaveButton.text = GetSaveButtonText();

            m_CurrentAsset = asset;
        }

        void AddExistingTags(List<string> tagsList)
        {
            if (tagsList.Count == 0) return;

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
                m_CurrentAsset.Tags.Remove(tag);
                chip.RemoveFromHierarchy();
            };
        }

        void AddExistingFiles(IEnumerable<IAssetFile> fileList)
        {
            m_AssetFileScrollView.style.display = DisplayStyle.Flex;

            if (!fileList.Any()) return;

            foreach (var file in fileList)
            {
                var fileItem = m_FileListItemTemplate.Instantiate();
                fileItem.Q<Label>("AssetNameLabel").text = file.Name;
                fileItem.Q<Label>("AssetSizeLabel").text = file.FileSize + " Kb";
                m_AssetFileScrollView.Add(fileItem);
            }

        }

        internal void CreateNewAsset()
        {
            ClearAssetInformation();

            m_AssetPublishToggle.value = false;

            UpdateStatus("Draft");
        }

        void ClearAssetInformation()
        {
            m_CurrentAsset = null;
            m_AssetTitleLabel.text = "AssetName";
            m_AssetStatusLastEditLabel.text = "";
            m_AssetTypeDropdown.value = "";
            m_AssetNameField.value = "";
            m_AssetDescriptionField.value = "";
            m_AssetTagsContainer.Clear();
        }

        void UpdateAssetInformation()
        {
            _ = UpdateAssetInformationAsync();
        }

        async Task UpdateAssetInformationAsync()
        {
            if (m_CurrentAsset == null)
                return;

            if (m_AssetPublishToggle.value && !string.Equals(m_CurrentAsset.Status, "Published"))
            {
                await PublishAsset();
                return;
            }

            await PlatformServices.AssetManager.UpdateAssetAsync(m_CurrentAsset, CancellationToken.None);
        }

        async Task PublishAsset()
        {
            if (m_CurrentAsset == null)
                return;

            try
            {
                switch (m_CurrentAsset.Status)
                {
                    case "Draft":
                        await PlatformServices.AssetManager.SendAssetToReviewAsync(m_CurrentAsset, CancellationToken.None);
                        await PlatformServices.AssetManager.ApproveAssetAsync(m_CurrentAsset, CancellationToken.None);
                        await PlatformServices.AssetManager.PublishApprovedAssetAsync(m_CurrentAsset, CancellationToken.None);
                        break;
                    case "Ingestion":
                        await PlatformServices.AssetManager.ApproveAssetAsync(m_CurrentAsset, CancellationToken.None);
                        await PlatformServices.AssetManager.PublishApprovedAssetAsync(m_CurrentAsset, CancellationToken.None);
                        break;
                    case "Approved":
                        await PlatformServices.AssetManager.PublishApprovedAssetAsync(m_CurrentAsset, CancellationToken.None);
                        break;
                }

                var updatedAsset = await PlatformServices.AssetManager.GetAssetAsync(m_CurrentAsset.Project, m_CurrentAsset.Id, m_CurrentAsset.Version, CancellationToken.None);
                if (updatedAsset != null)
                {
                    UpdateStatus(updatedAsset.Status);
                }
            }
            catch (Exception)
            {
                // Hide exception for now until we have a better way to handle it.
                // Invalid exception append on SendAssetToReview even if at the end the execution is done completely.
            }
        }

        void UpdateStatus(string status)
        {
            m_AssetStatusNameLabel.text = status;

            switch (status)
            {
                case "Published":
                case "Approved":
                    m_AssetStatusCircle.style.unityBackgroundImageTintColor = new Color(0.74f, 0.94f, 0.71f, 1f);
                    break;
                case "Withdrawn":
                    m_AssetStatusCircle.style.unityBackgroundImageTintColor = new Color(0.93f, 0.42f, 0.37f, 1f);
                    break;
                case "Draft":
                    m_AssetStatusCircle.style.unityBackgroundImageTintColor = new Color(0.86f, 0.60f, 0.27f, 1f);
                    break;
            }
        }
    }
}
#endif
