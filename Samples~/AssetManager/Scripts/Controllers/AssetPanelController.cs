using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetManager
{
    public class AssetPanelController
    {
        class TabsController
        {
            readonly List<(VisualElement tab, VisualElement container)> m_Tabs = new();

            public TabsController(VisualElement root, params (string, VisualElement)[] tabContents)
            {
                foreach (var (name, tabContent) in tabContents)
                {
                    var tabButton = root.Q<Button>(name);

                    m_Tabs.Add((tabButton, tabContent));

                    tabButton.clicked += () => SelectTab(name);
                }

                SelectTab("Datasets");
            }

            void SelectTab(string name)
            {
                foreach (var (tab, container) in m_Tabs)
                {
                    var isSelected = tab.name == name;

                    tab.SetEnabled(!isSelected);
                    tab.style.borderBottomColor = new Color(0.8f, 0.8f, 0.8f, isSelected ? 1f : 0f);

                    container.Show(isSelected);
                }
            }
        }

        VisualTreeAsset m_DatasetListItemTemplate;
        VisualTreeAsset m_AssetTagsTemplate;

        VisualElement m_RightPanel;
        VisualElement m_AssetTagsContainer;
        EnumField m_AssetTypeDropdown;
        TextField m_AssetNameField;
        VisualElement m_VersionLabelsContainer;
        Label m_SequenceNumber;
        Label m_ParentSequenceNumber;
        TextField m_AssetTagsField;
        TextField m_AssetDescriptionField;

        Button m_BackButton;

        VisualElement m_DatasetContainer;
        ScrollView m_DatasetScrollView;
        Button m_CreateDatasetButton;

        VisualElement m_VersionContainer;
        ScrollView m_VersionScrollView;

        Button m_PublishButton;
        Button m_SaveButton;
        Button m_FreezeButton;
        Button m_UnfreezeButton;

        StatusController m_StatusController;

        IAsset m_CurrentAsset;
        AssetUpdate m_AssetUpdate;
        MetadataController m_MetadataController;

        CancellationTokenSource m_ListCancellationTokenSource;

        public event Action<IDataset, bool> OnDatasetOpen;
        public event Action<IAsset> OnAssetUpdated;
        public Func<Task> PrepareAssetUpdateAsync { get; set; }

        public IAsset CurrentAsset => m_CurrentAsset;

        public void Init(VisualElement assetCreationPanel, VisualTreeAsset datasetListItemTemplate, VisualTreeAsset tagsTemplate, AddMetadataPopupController addMetadataPopup)
        {
            m_DatasetListItemTemplate = datasetListItemTemplate;
            m_AssetTagsTemplate = tagsTemplate;

            m_DatasetContainer = assetCreationPanel.Q("DatasetContainer");
            m_DatasetScrollView = m_DatasetContainer.Q<ScrollView>();

            m_VersionContainer = assetCreationPanel.Q("VersionContainer");
            m_VersionScrollView = m_VersionContainer.Q<ScrollView>();

            _ = new TabsController(assetCreationPanel,
                ("Datasets", m_DatasetContainer),
                ("History", m_VersionContainer)
            );

            m_RightPanel = assetCreationPanel.Q("RightPanel");

            m_AssetNameField = assetCreationPanel.Q<TextField>("AssetNameField");

            m_VersionLabelsContainer = assetCreationPanel.Q("LabelsChipContainer");

            m_SequenceNumber = assetCreationPanel.Q<Label>("SequenceNumber");
            m_ParentSequenceNumber = assetCreationPanel.Q<Label>("ParentSequenceNumber");

            m_StatusController = new StatusController(assetCreationPanel);

            var scrollView = m_RightPanel.Q<ScrollView>();
            scrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            scrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;

            m_AssetDescriptionField = assetCreationPanel.Q<TextField>("AssetDescriptionField");
            m_AssetTypeDropdown = assetCreationPanel.Q<EnumField>("AssetTypeDropdown");
            m_AssetTagsField = assetCreationPanel.Q<TextField>("AssetTagsField");
            m_AssetTagsContainer = assetCreationPanel.Q("AssetTagsChipContainer");

            var metadataTemplate = assetCreationPanel.Q<TemplateContainer>("MetadataItemTemplate");

            var metadataContainer = assetCreationPanel.Q("MetadataContainer");
            m_MetadataController = new MetadataController(metadataContainer, metadataTemplate.templateSource, addMetadataPopup);

            m_PublishButton = assetCreationPanel.Q<Button>("Publish");
            m_PublishButton.Hide();
            m_SaveButton = assetCreationPanel.Q<Button>("Save");
            m_SaveButton.Hide();
            m_FreezeButton = assetCreationPanel.Q<Button>("Freeze");
            m_FreezeButton.Hide();
            m_UnfreezeButton = assetCreationPanel.Q<Button>("Unfreeze");
            m_UnfreezeButton.Hide();
            m_CreateDatasetButton = assetCreationPanel.Q<Button>("CreateDatasetButton");
            m_CreateDatasetButton.Hide();

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

            m_PublishButton.RegisterCallback<ClickEvent>(_ => AsyncAction(PublishAssetAsync));
            m_SaveButton.RegisterCallback<ClickEvent>(_ => AsyncAction(UpdateAssetInformationAsync));
            m_FreezeButton.RegisterCallback<ClickEvent>(_ => AsyncAction(FreezeAsset));
            m_UnfreezeButton.RegisterCallback<ClickEvent>(_ => AsyncAction(UnfreezeAsset));
            m_CreateDatasetButton.RegisterCallback<ClickEvent>(_ => AsyncAction(CreateNewDatasetAsync));
            m_AssetTagsField.RegisterCallback<FocusInEvent>(AddTags);
        }

        public void OpenAsset(IAsset asset)
        {
            ClearAssetInformation();

            m_ListCancellationTokenSource = new CancellationTokenSource();
            var token = m_ListCancellationTokenSource.Token;

            m_RightPanel?.Show();

            m_CurrentAsset = asset;
            m_AssetUpdate = new AssetUpdate(asset);

            var canUpdate = !asset.IsFrozen && asset.Status == "Draft";

            m_SaveButton.Show(canUpdate);
            m_FreezeButton.Show(canUpdate);
            m_UnfreezeButton.Show(!canUpdate);
            m_CreateDatasetButton.Show(canUpdate);
            m_PublishButton.Show(asset.Status != "Published");

            m_AssetNameField.SetValueWithoutNotify(asset.Name);
            m_AssetNameField.SetEnabled(canUpdate);
            m_AssetTypeDropdown.SetValueWithoutNotify(asset.Type);
            m_AssetTypeDropdown.SetEnabled(canUpdate);
            m_AssetDescriptionField.SetValueWithoutNotify(asset.Description);
            m_AssetDescriptionField.SetEnabled(canUpdate);
            m_AssetTagsField.SetEnabled(canUpdate);

            foreach (var label in asset.Labels)
            {
                m_VersionLabelsContainer.AddTag(label.LabelName, null, m_AssetTagsTemplate, false);
            }

            UpdateStatus();

            m_SequenceNumber.tooltip = asset.Descriptor.AssetVersion.ToString();
            m_SequenceNumber.text = $"Ver. {asset.FrozenSequenceNumber}";
            m_SequenceNumber.Show(asset.IsFrozen);

            m_ParentSequenceNumber.text = $"Parent Ver. {asset.ParentFrozenSequenceNumber}";
            m_ParentSequenceNumber.Show(asset.ParentFrozenSequenceNumber > 0);

            Action<string> addTagAction = tag => AddTag(tag, canUpdate);
            addTagAction.AddTags(m_AssetUpdate.Tags);

            _ = m_MetadataController.PopulateMetadataAsync(asset, canUpdate);

            _ = ListDatasets(asset, canUpdate, token);
            _ = ListVersions(asset, token);
        }

        public void Clear()
        {
            m_RightPanel?.Hide();

            ClearAssetInformation();
        }

        async Task ListDatasets(IAsset asset, bool canUpdate, CancellationToken cancellationToken)
        {
            await foreach (var dataset in asset.ListDatasetsAsync(Range.All, cancellationToken))
            {
                AddDatasetRow(dataset, canUpdate);
            }
        }

        void AddDatasetRow(IDataset dataset, bool canUpdate)
        {
            var item = m_DatasetListItemTemplate.Instantiate();

            item.Q<Label>("DatasetNameLabel").text = dataset.Name;

            var description = item.Q<Label>("DatasetDescriptionLabel");
            description.text = dataset.Description;
            description.Show(!string.IsNullOrWhiteSpace(dataset.Description));

            item.RegisterCallback<ClickEvent>(_ =>
            {
                OnDatasetOpen?.Invoke(dataset, canUpdate);
            });
            m_DatasetScrollView.Add(item);
        }

        async Task ListVersions(IAsset asset, CancellationToken cancellationToken)
        {
            await foreach (var version in asset.QueryVersions()
                               .OrderBy("versionNumber", SortingOrder.Descending)
                               .ExecuteAsync(cancellationToken))
            {
                AddVersionRow(version);
            }
        }

        void AddVersionRow(IAsset asset)
        {
            var item = m_DatasetListItemTemplate.Instantiate();

            item.Q<Label>("DatasetNameLabel").text = asset.IsFrozen ? $"Ver. {asset.FrozenSequenceNumber}" : $"Pending";
            item.Q<Label>("DatasetDescriptionLabel").text = asset.Descriptor.AssetVersion.ToString();

            item.RegisterCallback<ClickEvent>(_ =>
            {
                OpenAsset(asset);
            });
            m_VersionScrollView.Add(item);
        }

        void ClearAssetInformation()
        {
            if (m_ListCancellationTokenSource != null)
            {
                m_ListCancellationTokenSource.Cancel();
                m_ListCancellationTokenSource.Dispose();
                m_ListCancellationTokenSource = null;
            }

            m_CurrentAsset = null;
            m_StatusController.Clear();
            m_AssetNameField.SetValueWithoutNotify("");
            m_VersionLabelsContainer.Clear();
            m_SequenceNumber.text = "";
            m_ParentSequenceNumber.text = "";
            m_ParentSequenceNumber.Hide();
            m_AssetTypeDropdown.SetValueWithoutNotify(default);
            m_AssetDescriptionField.SetValueWithoutNotify("");
            m_AssetTagsField.SetValueWithoutNotify("");
            m_AssetTagsContainer.Clear();
            m_DatasetScrollView.Clear();
            m_VersionScrollView.Clear();
            m_MetadataController.Clear();
        }

        void AsyncAction(Func<Task> action)
        {
            _ = action?.Invoke();
        }

        async Task UpdateAssetInformationAsync()
        {
            if (m_CurrentAsset == null) return;

            ChangeButtonEnabledState(false);

            if (PrepareAssetUpdateAsync != null) await PrepareAssetUpdateAsync.Invoke();

            var updateTasks = new List<Task>
            {
                m_CurrentAsset.UpdateAsync(m_AssetUpdate, default),
                m_MetadataController.UpdateMetadataAsync(default),
            };

            try
            {
                await Task.WhenAll(updateTasks);

                if (updateTasks.TrueForAll(x => x.IsCompletedSuccessfully))
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

        async Task PublishAssetAsync()
        {
            if (m_CurrentAsset == null) return;

            ChangeButtonEnabledState(false);

            try
            {
                // Successful publishing workflow
                //Draft -> Review -> Approved -> Published
                switch (m_CurrentAsset.Status)
                {
                    case "Draft":
                        await m_CurrentAsset.UpdateStatusAsync(AssetStatusAction.SendForReview, CancellationToken.None);
                        await m_CurrentAsset.UpdateStatusAsync(AssetStatusAction.Approve, CancellationToken.None);
                        await m_CurrentAsset.UpdateStatusAsync(AssetStatusAction.Publish, CancellationToken.None);
                        break;
                    case "Ingestion": // Status when asset is in review
                        await m_CurrentAsset.UpdateStatusAsync(AssetStatusAction.Approve, CancellationToken.None);
                        await m_CurrentAsset.UpdateStatusAsync(AssetStatusAction.Publish, CancellationToken.None);
                        break;
                    case "Approved":
                        await m_CurrentAsset.UpdateStatusAsync(AssetStatusAction.Publish, CancellationToken.None);
                        break;
                }

                OnAssetUpdated?.Invoke(m_CurrentAsset);
            }
            catch (Exception)
            {
                // Hide exception for now until we have a better way to handle it.
                // Invalid exception can occur on SendAssetToReview even if the execution completes.
            }
            finally
            {
                ChangeButtonEnabledState(true);
            }
        }

        async Task FreezeAsset()
        {
            if (m_CurrentAsset == null) return;

            ChangeButtonEnabledState(false);

            try
            {
                await m_CurrentAsset.FreezeAsync("Asset Manager sample submission.", CancellationToken.None);
                await Task.Delay(1000); // There is a delay between the AM service and the search database

                OnAssetUpdated?.Invoke(m_CurrentAsset);
            }
            catch (Exception e)
            {
                e.LogException();
            }
            finally
            {
                ChangeButtonEnabledState(true);
            }
        }

        async Task UnfreezeAsset()
        {
            if (m_CurrentAsset == null) return;

            ChangeButtonEnabledState(false);

            try
            {
                var asset = await m_CurrentAsset.CreateUnfrozenVersionAsync(CancellationToken.None);
                await Task.Delay(1000); // There is a delay between the AM service and the search database
                if (asset != null)
                {
                    OnAssetUpdated?.Invoke(asset);
                }
            }
            catch (Exception e)
            {
                e.LogException();
            }
            finally
            {
                ChangeButtonEnabledState(true);
            }
        }

        async Task CreateNewDatasetAsync()
        {
            if (m_CurrentAsset == null)
                return;

            var dataset = await m_CurrentAsset.CreateDatasetAsync(new DatasetCreation($"Dataset_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}"), default);
            if (dataset != null)
                AddDatasetRow(dataset, true);
        }

        void UpdateStatus()
        {
            if (m_CurrentAsset == null)
                return;

            m_StatusController.Update(m_CurrentAsset.Status, m_CurrentAsset.AuthoringInfo?.Updated);

            // Successful publishing workflow
            //Draft -> Review -> Approved -> Published
            switch (m_CurrentAsset.Status)
            {
                case "Published":
                    m_StatusController.SetStatusColor(new Color(0.74f, 0.94f, 0.71f, 1f));
                    break;
                case "Approved":
                    m_StatusController.SetStatusColor(new Color(0.74f, 0.94f, 0.71f, 1f));
                    m_PublishButton.visible = true;
                    break;
                case "Ingestion": // Status when asset is in review
                    m_PublishButton.visible = true;
                    break;
                case "Withdrawn":
                    m_StatusController.SetStatusColor(new Color(0.93f, 0.42f, 0.37f, 1f));
                    break;
                case "Draft":
                    m_StatusController.SetStatusColor(new Color(0.86f, 0.60f, 0.27f, 1f));
                    m_PublishButton.visible = true;
                    m_CreateDatasetButton.visible = true;
                    break;
            }
        }

        void ChangeButtonEnabledState(bool state)
        {
            m_CreateDatasetButton.SetEnabled(state);
            m_PublishButton.SetEnabled(state);
            m_SaveButton.SetEnabled(state);
            m_FreezeButton.SetEnabled(state);
            m_UnfreezeButton.SetEnabled(state);
            m_BackButton.SetEnabled(state);
        }

        void AddTags(FocusInEvent evt)
        {
            m_AssetTagsField.ParseTags(m_AssetUpdate.Tags, tag => AddTag(tag, true));
        }

        void AddTag(string tag, bool canRemove)
        {
            m_AssetTagsContainer.AddTag(tag, m_AssetUpdate.Tags, m_AssetTagsTemplate, canRemove);
        }
    }
}
