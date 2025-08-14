using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetManager
{
    public class AssetPanelController
    {
        class TabsController
        {
            public event Action OnSelection;

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

                OnSelection?.Invoke();
            }
        }

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

        VisualElement m_ReferencesContainer;
        ScrollView m_ReferencesScrollView;
        AddReferencesPopupController m_AddReferencePopup;

        Button m_SaveButton;
        Button m_FreezeButton;
        Button m_UnfreezeButton;

        StatusController m_StatusController;

        string[] m_ReachableStatuses;

        IAsset m_CurrentAsset;
        AssetProperties m_CurrentAssetProperties;
        bool m_IsEditable;
        AssetUpdate m_AssetUpdate;
        MetadataController m_MetadataController;
        string m_SelectedStatus;

        CancellationTokenSource m_CurrentAssetCancellationTokenSource;

        public event Action<IDataset, bool> OnDatasetOpen;
        public event Action<IAsset> OnAssetUpdated;
        public Func<Task> PrepareAssetUpdateAsync { get; set; }
        public IAsset CurrentAsset => m_CurrentAsset;

        public void Init(VisualElement assetCreationPanel, VisualTreeAsset tagsTemplate, AddMetadataPopupController addMetadataPopup)
        {
            m_AssetTagsTemplate = tagsTemplate;

            m_DatasetContainer = assetCreationPanel.Q("DatasetContainer");
            m_DatasetScrollView = m_DatasetContainer.Q<ScrollView>();

            m_VersionContainer = assetCreationPanel.Q("VersionContainer");
            m_VersionScrollView = m_VersionContainer.Q<ScrollView>();

            m_ReferencesContainer = assetCreationPanel.Q("ReferencesContainer");
            m_ReferencesScrollView = m_ReferencesContainer.Q<ScrollView>();
            m_AddReferencePopup = new AddReferencesPopupController(assetCreationPanel);

            var tabsController = new TabsController(assetCreationPanel,
                ("Datasets", m_DatasetContainer),
                ("History", m_VersionContainer),
                ("References", m_ReferencesContainer)
            );
            tabsController.OnSelection += () =>
            {
                m_AddReferencePopup.Hide();
            };

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
                if (m_SelectedStatus == m_CurrentAssetProperties.StatusName)
                {
                    m_SelectedStatus = null;
                }
                m_SaveButton.SetEnabled(m_IsEditable || m_SelectedStatus != null);
            });

            m_SaveButton.RegisterCallback<ClickEvent>(_ => AsyncAction(UpdateAssetAsync));
            m_FreezeButton.RegisterCallback<ClickEvent>(_ => AsyncAction(FreezeAsset));
            m_UnfreezeButton.RegisterCallback<ClickEvent>(_ => AsyncAction(UnfreezeAsset));
            m_CreateDatasetButton.RegisterCallback<ClickEvent>(_ => AsyncAction(CreateNewDatasetAsync));
            m_AssetTagsField.RegisterCallback<FocusInEvent>(AddEditableTags);
        }

        public void OpenAsset(IAsset asset)
        {
            Clear();

            m_CurrentAssetCancellationTokenSource = new CancellationTokenSource();
            var token = m_CurrentAssetCancellationTokenSource.Token;

            m_RightPanel?.Show();

            m_CurrentAsset = asset;

            _ = PopulateAsync(asset, token);

            _ = m_MetadataController.PopulateMetadataAsync(asset, m_IsEditable);
        }

        public void Clear()
        {
            m_AddReferencePopup.Hide();
            m_RightPanel?.Hide();

            ClearAssetInformation();
        }

        async Task PopulateAsync(IAsset asset, CancellationToken token)
        {
            m_CurrentAssetProperties = await asset.GetPropertiesAsync(token);

            if (token.IsCancellationRequested) return;

            m_IsEditable = m_CurrentAssetProperties is {State: AssetState.Unfrozen};
            m_AssetUpdate = new AssetUpdate();

            m_SaveButton.SetEnabled(m_IsEditable);
            m_FreezeButton.Show(m_IsEditable);
            m_UnfreezeButton.Show(!m_IsEditable);
            m_CreateDatasetButton.Show(m_IsEditable);

            m_AssetNameField.SetValueWithoutNotify(m_CurrentAssetProperties.Name);
            m_AssetNameField.SetEnabled(m_IsEditable);
            m_AssetTypeDropdown.SetValueWithoutNotify(m_CurrentAssetProperties.Type);
            m_AssetTypeDropdown.SetEnabled(m_IsEditable);
            m_AssetDescriptionField.SetValueWithoutNotify(m_CurrentAssetProperties.Description);
            m_AssetDescriptionField.SetEnabled(m_IsEditable);

            m_AssetTagsField.Show(m_IsEditable);

            Action<string> addTagAction = tag => m_AssetTagsContainer.AddTag(tag, m_AssetTagsTemplate, m_IsEditable ? OnTagRemoved : null);
            addTagAction.AddTags(m_CurrentAssetProperties.Tags?.ToList() ?? new List<string>());

            foreach (var label in m_CurrentAssetProperties.Labels)
            {
                m_VersionLabelsContainer.AddTag(label.LabelName, m_AssetTagsTemplate);
            }

            m_SequenceNumber.tooltip = asset.Descriptor.AssetVersion.ToString();
            m_SequenceNumber.text = $"Ver. {m_CurrentAssetProperties.FrozenSequenceNumber}";
            m_SequenceNumber.Show(m_CurrentAssetProperties.State == AssetState.Frozen);

            m_ParentSequenceNumber.text = $"Parent Ver. {m_CurrentAssetProperties.ParentFrozenSequenceNumber}";
            m_ParentSequenceNumber.Show(m_CurrentAssetProperties.ParentFrozenSequenceNumber > 0);

            UpdateStatusAsync();

            await Task.WhenAll(
                ListReachableStatusesAsync(token),
                ListDatasets(asset, m_IsEditable, token),
                ListVersions(asset, token),
                ListReferences(asset, token)
            );
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
            m_ReferencesScrollView.Clear();
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
            m_SaveButton.SetEnabled(m_IsEditable && state);
            m_FreezeButton.SetEnabled(state);
            m_UnfreezeButton.SetEnabled(state);
            m_BackButton.SetEnabled(state);
        }

        void AddEditableTags(FocusInEvent _) => m_AssetTagsField.ParseTags(AddEditableTag);
        
        void AddEditableTag(string tag)
        {
            m_AssetUpdate.Tags ??= m_CurrentAssetProperties.Tags?.ToList() ?? new List<string>();
            m_AssetUpdate.Tags.Add(tag);
            m_AssetTagsContainer.AddTag(tag, m_AssetTagsTemplate, OnTagRemoved);
        }
        
        void OnTagRemoved(string tag)
        {
            m_AssetUpdate.Tags ??= m_CurrentAssetProperties.Tags?.ToList() ?? new List<string>();
            m_AssetUpdate.Tags.Remove(tag);
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
            var rowItem = new RowItem();
            rowItem.RegisterCallback<ClickEvent>(_ =>
            {
                OnDatasetOpen?.Invoke(dataset, canUpdate);
            });
            m_DatasetScrollView.Add(rowItem);

            _ = PopulateDatasetRow(dataset, rowItem);
        }

        static async Task PopulateDatasetRow(IDataset dataset, RowItem rowItem)
        {
            var properties = await dataset.GetPropertiesAsync(CancellationToken.None);

            rowItem.AddLabel(properties.Name);

            if (properties.SystemTags.Any())
            {
                rowItem.AddLabel(string.Join(", ", properties.SystemTags));
            }

            if (!string.IsNullOrEmpty(properties.Description))
            {
                rowItem.AddLabel(properties.Description);
            }
        }

        async Task ListVersions(IAsset asset, CancellationToken cancellationToken)
        {
            await foreach (var version in asset.ListVersionsAsync(Range.All, cancellationToken))
            {
                AddVersionRow(version);
            }
        }

        void AddVersionRow(IAsset asset)
        {
            var rowItem = new RowItem();
            rowItem.RegisterCallback<ClickEvent>(_ =>
            {
                OnAssetUpdated?.Invoke(asset);
            });
            m_VersionScrollView.Add(rowItem);

            _ = PopulateVersionRow(asset, rowItem);
        }

        static async Task PopulateVersionRow(IAsset asset, RowItem rowItem)
        {
            var properties = await asset.GetPropertiesAsync(CancellationToken.None);

            rowItem.AddLabel(properties.State == AssetState.Frozen ? $"Ver. {properties.FrozenSequenceNumber}" : $"Pending");
            rowItem.AddLabel(asset.Descriptor.AssetVersion.ToString());
        }

        async Task ListReferences(IAsset asset, CancellationToken cancellationToken)
        {
            var targetFoldout = new Foldout
            {
                text = "References",
                value = true
            };
            m_ReferencesScrollView.Add(targetFoldout);
            var sourceFoldout = new Foldout
            {
                text = "Is referenced by",
                value = true
            };
            m_ReferencesScrollView.Add(sourceFoldout);

            await foreach (var reference in asset.ListReferencesAsync(Range.All, cancellationToken))
            {
                if (reference.SourceAssetId == asset.Descriptor.AssetId && reference.SourceAssetVersion == asset.Descriptor.AssetVersion)
                {
                    if (reference.IsValid)
                    {
                        _ = AddReferenceRow(targetFoldout, reference.ProjectDescriptor, reference.ReferenceId, reference.TargetAssetId, reference.TargetAssetVersion, reference.TargetLabel);
                    }
                    else
                    {
                        AddInvalidReferenceRow(targetFoldout, reference.ReferenceId, reference.TargetAssetId, reference.TargetAssetVersion, reference.TargetLabel);
                    }
                }
                else
                {
                    if (reference.IsValid)
                    {
                        _ = AddReferenceRow(sourceFoldout, reference.ProjectDescriptor, reference.ReferenceId, reference.SourceAssetId, reference.SourceAssetVersion);
                    }
                    else
                    {
                        AddInvalidReferenceRow(sourceFoldout, reference.ReferenceId, reference.SourceAssetId, reference.SourceAssetVersion);
                    }
                }
            }

            var addButton = new Button {text = "Add Reference"};
            addButton.AddToClassList("sample-button");
            addButton.AddToClassList("button-blue");
            targetFoldout.Add(addButton);
            addButton.clicked += () =>
            {
                m_AddReferencePopup.Show(m_CurrentAsset.Descriptor.ProjectDescriptor, async (assetId, version, label) =>
                {
                    IAssetReference reference;
                    if (version.HasValue)
                    {
                        reference = await m_CurrentAsset.AddReferenceAsync(assetId, version.Value, default);
                    }
                    else
                    {
                        reference = await m_CurrentAsset.AddReferenceAsync(assetId, label, default);
                    }
                    _ = AddReferenceRow(targetFoldout, reference.ProjectDescriptor, reference.ReferenceId, reference.TargetAssetId, reference.TargetAssetVersion, reference.TargetLabel);
                });
            };
        }

        void AddInvalidReferenceRow(VisualElement parent, string referenceId, AssetId assetId, AssetVersion? assetVersion = null, string label = null)
        {
            var rowItem = new RowItem(referenceId);
            rowItem.AddLabel(assetId + " INVALID\n" + (assetVersion?.ToString() ?? label));
            parent.Add(rowItem);

            AddRemoveButton(rowItem, referenceId);
        }

        async Task AddReferenceRow(VisualElement parent, ProjectDescriptor projectDescriptor, string referenceId, AssetId assetId, AssetVersion? assetVersion = null, string label = null)
        {
            var rowItem = new RowItem(referenceId);

            var title = rowItem.AddLabel($"{assetId}\n{assetVersion?.ToString() ?? label}");

            parent.Add(rowItem);
            rowItem.SendToBack();

            IAsset asset;
            if (assetVersion.HasValue)
            {
                var descriptor = new AssetDescriptor(projectDescriptor, assetId, assetVersion.Value);
                asset = await PlatformServices.AssetRepository.GetAssetAsync(descriptor, default);
            }
            else
            {
                asset = await PlatformServices.AssetRepository.GetAssetAsync(projectDescriptor, assetId, label, default);
            }

            var properties = await asset.GetPropertiesAsync(CancellationToken.None);

            title.text = $"{properties.Name}\n({label ?? (properties.State == AssetState.Frozen ? $"Ver. {properties.FrozenSequenceNumber}" : "Ver.1 - Pending")})";
            rowItem.AddLabel(properties.Type.GetValueAsString());

            rowItem.RegisterCallback<ClickEvent>(_ =>
            {
                OpenAsset(asset);
            });

            AddRemoveButton(rowItem, referenceId);
        }

        void AddRemoveButton(RowItem rowItem, string referenceId)
        {
            var button = new Button
            {
                text = "Remove",
                style = {width = 80}
            };
            button.AddToClassList("sample-button");
            button.AddToClassList("button-blue");
            rowItem.Add(button);
            button.RegisterCallback<ClickEvent>(evt =>
            {
                evt.StopPropagation();
            });
            button.clicked += async () =>
            {
                await m_CurrentAsset.RemoveReferenceAsync(referenceId, default);
                rowItem.RemoveFromHierarchy();
            };
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

            if (m_IsEditable)
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
                DialogService.ShowMessage(e, "Update failed", $"Failed to update the asset with reason: {e.Message}");
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
                await m_CurrentAsset.FreezeAsync(new AssetFreeze("Asset Manager sample submission."), CancellationToken.None);
                await UnityTask.Delay(1000); // There is a delay between the AM service and the search database

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
                await UnityTask.Delay(1000); // There is a delay between the AM service and the search database
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

        void UpdateStatusAsync()
        {
            m_StatusController.Update(m_CurrentAssetProperties.StatusName, m_CurrentAssetProperties.AuthoringInfo?.Updated);

            m_StatusController.SetStatusColor(m_CurrentAssetProperties.StatusName switch
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
