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
        DropdownField m_StatusDropdown;

        Button m_BackButton;

        VisualElement m_DatasetContainer;
        ScrollView m_DatasetScrollView;
        Button m_CreateDatasetButton;

        VisualElement m_VersionContainer;
        ScrollView m_VersionScrollView;

        Button m_SaveButton;
        Button m_FreezeButton;
        Button m_UnfreezeButton;

        StatusController m_StatusController;

        string[] m_ReachableStatuses;

        IAsset m_CurrentAsset;
        AssetUpdate m_AssetUpdate;
        MetadataController m_MetadataController;
        string m_SelectedStatus;

        CancellationTokenSource m_CurrentAssetCancellationTokenSource;

        public event Action<IDataset, bool> OnDatasetOpen;
        public event Action<IAsset> OnAssetUpdated;
        public Func<Task> PrepareAssetUpdateAsync { get; set; }
        public IAsset CurrentAsset => m_CurrentAsset;

        bool IsEditable => m_CurrentAsset is {IsFrozen: false};

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

            m_StatusDropdown = assetCreationPanel.Q<DropdownField>("StatusSelectionDropdown");
            m_AssetDescriptionField = assetCreationPanel.Q<TextField>("AssetDescriptionField");
            m_AssetTypeDropdown = assetCreationPanel.Q<EnumField>("AssetTypeDropdown");
            m_AssetTagsField = assetCreationPanel.Q<TextField>("AssetTagsField");
            m_AssetTagsContainer = assetCreationPanel.Q("AssetTagsChipContainer");

            var metadataTemplate = assetCreationPanel.Q<TemplateContainer>("MetadataItemTemplate");

            var metadataContainer = assetCreationPanel.Q("MetadataContainer");
            m_MetadataController = new MetadataController(metadataContainer, metadataTemplate.templateSource, addMetadataPopup);

            m_SaveButton = assetCreationPanel.Q<Button>("Save");
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

            m_StatusDropdown.RegisterValueChangedCallback(evt =>
            {
                m_SelectedStatus = m_ReachableStatuses.FirstOrDefault(s => s == evt.newValue);
                if (m_SelectedStatus == m_CurrentAsset.StatusName)
                {
                    m_SelectedStatus = null;
                }
                m_SaveButton.SetEnabled(IsEditable || m_SelectedStatus != null);
            });

            m_SaveButton.RegisterCallback<ClickEvent>(_ => AsyncAction(UpdateAssetAsync));
            m_FreezeButton.RegisterCallback<ClickEvent>(_ => AsyncAction(FreezeAsset));
            m_UnfreezeButton.RegisterCallback<ClickEvent>(_ => AsyncAction(UnfreezeAsset));
            m_CreateDatasetButton.RegisterCallback<ClickEvent>(_ => AsyncAction(CreateNewDatasetAsync));
            m_AssetTagsField.RegisterCallback<FocusInEvent>(AddTags);
        }

        public void OpenAsset(IAsset asset)
        {
            ClearAssetInformation();

            m_CurrentAssetCancellationTokenSource = new CancellationTokenSource();
            var token = m_CurrentAssetCancellationTokenSource.Token;

            m_RightPanel?.Show();

            m_CurrentAsset = asset;
            m_AssetUpdate = new AssetUpdate(asset);

            m_SaveButton.SetEnabled(IsEditable);
            m_FreezeButton.Show(IsEditable);
            m_UnfreezeButton.Show(!IsEditable);
            m_CreateDatasetButton.Show(IsEditable);

            m_AssetNameField.SetValueWithoutNotify(asset.Name);
            m_AssetNameField.SetEnabled(IsEditable);
            m_AssetTypeDropdown.SetValueWithoutNotify(asset.Type);
            m_AssetTypeDropdown.SetEnabled(IsEditable);
            m_AssetDescriptionField.SetValueWithoutNotify(asset.Description);
            m_AssetDescriptionField.SetEnabled(IsEditable);
            m_AssetTagsField.SetEnabled(IsEditable);

            foreach (var label in asset.Labels)
            {
                m_VersionLabelsContainer.AddTag(label.LabelName, null, m_AssetTagsTemplate, false);
            }

            _ = UpdateStatusAsync(token);

            m_SequenceNumber.tooltip = asset.Descriptor.AssetVersion.ToString();
            m_SequenceNumber.text = $"Ver. {asset.FrozenSequenceNumber}";
            m_SequenceNumber.Show(asset.IsFrozen);

            m_ParentSequenceNumber.text = $"Parent Ver. {asset.ParentFrozenSequenceNumber}";
            m_ParentSequenceNumber.Show(asset.ParentFrozenSequenceNumber > 0);

            Action<string> addTagAction = tag => AddTag(tag, IsEditable);
            addTagAction.AddTags(m_AssetUpdate.Tags);

            _ = m_MetadataController.PopulateMetadataAsync(asset, IsEditable);

            _ = ListDatasets(asset, IsEditable, token);
            _ = ListVersions(asset, token);
        }

        public void Clear()
        {
            m_RightPanel?.Hide();

            ClearAssetInformation();
        }

        void ClearAssetInformation()
        {
            if (m_CurrentAssetCancellationTokenSource != null)
            {
                m_CurrentAssetCancellationTokenSource.Cancel();
                m_CurrentAssetCancellationTokenSource.Dispose();
                m_CurrentAssetCancellationTokenSource = null;
            }

            m_CurrentAsset = null;
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
            m_StatusController.Clear();
            m_StatusDropdown.choices = new List<string>();
            m_StatusDropdown.SetEnabled(false);
            m_ReachableStatuses = null;
            m_SelectedStatus = null;
        }

        void ChangeButtonEnabledState(bool state)
        {
            m_CreateDatasetButton.SetEnabled(state);
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

        void AsyncAction(Func<Task> action)
        {
            _ = action?.Invoke();
        }

        async Task UpdateAssetAsync()
        {
            if (m_CurrentAsset == null) return;

            ChangeButtonEnabledState(false);

            var updateTasks = new List<Task>();

            if (IsEditable)
            {
                if (PrepareAssetUpdateAsync != null) await PrepareAssetUpdateAsync.Invoke();

                updateTasks.Add(m_CurrentAsset.UpdateAsync(m_AssetUpdate, default));
                updateTasks.Add(m_MetadataController.UpdateMetadataAsync(default));
            }

            if (m_SelectedStatus != null)
            {
                updateTasks.Add(m_CurrentAsset.UpdateStatusAsync(m_SelectedStatus, default));
                m_SelectedStatus = null;
            }

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

            IDatasetCreation datasetCreation = new DatasetCreation($"Dataset_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}");
            var dataset = await m_CurrentAsset.CreateDatasetAsync(datasetCreation, default);
            if (dataset != null)
                AddDatasetRow(dataset, true);
        }

        async Task UpdateStatusAsync(CancellationToken cancellationToken)
        {
            m_StatusController.Update(null, null);

            if (m_CurrentAsset == null)
                return;

            m_StatusController.Update(m_CurrentAsset.StatusName, m_CurrentAsset.AuthoringInfo?.Updated);

            m_StatusController.SetStatusColor(m_CurrentAsset.StatusName switch
            {
                // Legacy || Default
                "Published" or "Approved" => new Color(0.07f, 0.65f, 0.58f, 1f),

                // Legacy || Default
                "InReview" or "Ready for Review" => new Color(0.24f, 0.39f, 0.87f, 1f),

                // Legacy || Legacy || Default
                "Withdrawn" or "Rejected" or "Needs Changes" => new Color(0.9f, 0.28f, 0.3f, 1f),

                "Draft" => new Color(0.86f, 0.60f, 0.27f, 1f),

                _ => Color.gray
            });

            await ListReachableStatusesAsync(cancellationToken);
        }

        async Task ListReachableStatusesAsync(CancellationToken cancellationToken)
        {
            m_StatusDropdown.SetEnabled(false);

            if (m_CurrentAsset == null)
                return;

            m_ReachableStatuses = await m_CurrentAsset.GetReachableStatusNamesAsync(cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                return;

            m_StatusDropdown.choices = m_ReachableStatuses.ToList();
            m_StatusDropdown.SetValueWithoutNotify(string.Empty);

            m_StatusDropdown.SetEnabled(true);
        }
    }
}
