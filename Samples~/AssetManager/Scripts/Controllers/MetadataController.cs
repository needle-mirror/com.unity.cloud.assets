using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetManager
{
    public class MetadataController
    {
        readonly VisualElement m_ContentContainer;
        readonly VisualTreeAsset m_Template;

        readonly AddMetadataPopupController m_AddMetadataController;
        readonly VisualElement m_AddButton;

        IMetadataContainer m_MetadataContainer;
        readonly List<string> m_MetadataKeys = new();
        readonly Dictionary<string, MetadataValue> m_MetadataValues = new();
        readonly List<string> m_MetadataKeysToRemove = new();

        CancellationTokenSource m_GetterCancellationTokenSource;

        public MetadataController(VisualElement contentContainer, VisualTreeAsset template, AddMetadataPopupController addMetadataController)
        {
            m_ContentContainer = contentContainer;
            m_Template = template;

            m_AddMetadataController = addMetadataController;

            var addButton = m_ContentContainer.parent.Q<Button>("AddMetadataButton");
            addButton.clicked += AddMetadata;

            m_AddButton = addButton;
        }

        public async Task PopulateMetadataAsync(IAsset asset, bool canUpdate)
        {
            var cancellationToken = RefreshCancellationToken();

            await PopulateMetadataAsync(asset.SystemMetadata, cancellationToken, false);

            m_MetadataContainer = asset.Metadata;

            m_AddButton.style.display = canUpdate ? DisplayStyle.Flex : DisplayStyle.None;

            await PopulateMetadataAsync(m_MetadataContainer as IReadOnlyMetadataContainer, cancellationToken, canUpdate);
        }

        public async Task PopulateMetadataAsync(IDataset dataset, bool canUpdate)
        {
            var cancellationToken = RefreshCancellationToken();

            await PopulateMetadataAsync(dataset.SystemMetadata, cancellationToken, false);

            m_MetadataContainer = dataset.Metadata;

            m_AddButton.style.display = canUpdate ? DisplayStyle.Flex : DisplayStyle.None;

            await PopulateMetadataAsync(m_MetadataContainer as IReadOnlyMetadataContainer, cancellationToken, canUpdate);
        }

        public void Hide()
        {
            m_AddMetadataController.Hide();
        }

        public void Clear()
        {
            Hide();

            m_ContentContainer.Clear();
            m_MetadataValues.Clear();
            m_MetadataKeysToRemove.Clear();
            m_MetadataKeys.Clear();
        }

        public async Task UpdateMetadataAsync(CancellationToken cancellationToken)
        {
            List<Exception> exceptions = new();

            try
            {
                await m_MetadataContainer.AddOrUpdateAsync(m_MetadataValues, cancellationToken);
                m_MetadataValues.Clear();
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }

            try
            {
                await m_MetadataContainer.RemoveAsync(m_MetadataKeysToRemove, cancellationToken);
                m_MetadataKeysToRemove.Clear();
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }
        }

        async Task PopulateMetadataAsync(IReadOnlyMetadataContainer metadataContainer, CancellationToken cancellationToken, bool canUpdate)
        {
            var metadata = metadataContainer.Query().ExecuteAsync(cancellationToken);

            await foreach (var kvp in metadata)
            {
                m_MetadataKeys.Add(kvp.Key);

                var visualElement = CreateMetadataElement(kvp.Key, canUpdate);

                _ = ParseValueAsync(kvp.Key, kvp.Value, visualElement, canUpdate, cancellationToken);
            }
        }

        async Task ParseValueAsync(string key, MetadataValue value, VisualElement visualElement, bool canUpdate, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            switch (value.ValueType)
            {
                case MetadataValueType.Unknown:
                    await WaitOnUnknownAsync(key, value, visualElement, canUpdate, cancellationToken);
                    break;

                case MetadataValueType.Boolean:
                    PopulateBoolean(key, value.AsBoolean(), visualElement, canUpdate);
                    break;

                case MetadataValueType.Number:
                    PopulateNumber(key, value.AsNumber(), visualElement, canUpdate);
                    break;

                case MetadataValueType.SingleSelection:
                    await PopulateSingleSelectionAsync(key, null, value.AsSingleSelection(), visualElement, canUpdate);
                    break;

                case MetadataValueType.MultiSelection:
                    await PoplateMultiSelectionAsync(key, null, value.AsMultiSelection(), visualElement, canUpdate);
                    break;

                case MetadataValueType.Url:
                    PopulateUrl(key, value.AsUrl(), visualElement, canUpdate);
                    break;

                case MetadataValueType.Timestamp:
                    PopulateTimestamp(key, value.AsTimestamp(), visualElement, canUpdate);
                    break;

                default:
                    PopulateText(key, value.AsText(), visualElement, canUpdate);
                    break;
            }
        }

        async Task WaitOnUnknownAsync(string key, MetadataValue value, VisualElement visualElement, bool canUpdate, CancellationToken cancellationToken)
        {
            // Apply a timeout to prevent waiting forever
            var timeoutCancellationSource = new CancellationTokenSource(3000);
            while (value.ValueType == MetadataValueType.Unknown)
            {
                await UnityTask.Delay(100, timeoutCancellationSource.Token);

                if (cancellationToken.IsCancellationRequested || timeoutCancellationSource.IsCancellationRequested) return;
            }

            if (cancellationToken.IsCancellationRequested) return;

            await ParseValueAsync(key, value, visualElement, canUpdate, cancellationToken);
        }

        static void PopulateLabel(string value, VisualElement visualElement)
        {
            var label = visualElement.Q<Label>("NoEdit");
            label.style.display = DisplayStyle.Flex;
            label.text = value;
        }

        void PopulateBoolean(string key, BooleanMetadata metadata, VisualElement visualElement, bool canUpdate = true)
        {
            if (!canUpdate)
            {
                PopulateLabel(metadata.Value.ToString(), visualElement);
                return;
            }

            var boolField = visualElement.Q<Toggle>("Boolean");
            boolField.style.display = DisplayStyle.Flex;

            boolField.SetValueWithoutNotify(metadata.Value);
            boolField.RegisterValueChangedCallback(evt =>
            {
                metadata.Value = evt.newValue;
                m_MetadataValues[key] = metadata;
            });
        }

        void PopulateNumber(string key, NumberMetadata metadata, VisualElement visualElement, bool canUpdate = true)
        {
            if (!canUpdate)
            {
                PopulateLabel(metadata.Value.ToString(), visualElement);
                return;
            }

            var numberField = visualElement.Q<DoubleField>("Number");
            numberField.style.display = DisplayStyle.Flex;

            numberField.SetValueWithoutNotify(metadata.Value);
            numberField.RegisterValueChangedCallback(evt =>
            {
                metadata.Value = evt.newValue;
                m_MetadataValues[key] = metadata;
            });
        }

        void PopulateTimestamp(string key, DateTimeMetadata metadata, VisualElement visualElement, bool canUpdate = true)
        {
            if (!canUpdate)
            {
                PopulateLabel(metadata.Value.ToString(), visualElement);
                return;
            }

            var timestampField = visualElement.Q<TextField>("Text");
            timestampField.style.display = DisplayStyle.Flex;

            timestampField.SetValueWithoutNotify(metadata.Value.ToString());
            timestampField.RegisterValueChangedCallback(evt =>
            {
                if (DateTime.TryParse(evt.newValue, out var timestamp))
                {
                    metadata.Value = timestamp;
                    m_MetadataValues[key] = metadata;
                }
            });
        }

        void PopulateText(string key, StringMetadata metadata, VisualElement visualElement, bool canUpdate = true)
        {
            if (!canUpdate)
            {
                PopulateLabel(metadata.Value, visualElement);
                return;
            }

            var textField = visualElement.Q<TextField>("Text");
            textField.style.display = DisplayStyle.Flex;

            textField.SetValueWithoutNotify(metadata.Value);
            textField.RegisterValueChangedCallback(evt =>
            {
                metadata.Value = evt.newValue;
                m_MetadataValues[key] = metadata;
            });
        }

        async Task PopulateSingleSelectionAsync(string key, ISelectionFieldDefinition selectionFieldDefinition, SingleSelectionMetadata metadata, VisualElement visualElement, bool canUpdate = true)
        {
            if (!canUpdate)
            {
                PopulateLabel(metadata.SelectedValue, visualElement);
                return;
            }

            var dropdownField = visualElement.Q<DropdownField>("SingleSelection");
            dropdownField.style.display = DisplayStyle.Flex;

            if (selectionFieldDefinition == null)
            {
                var fieldDefinition = await m_AddMetadataController.GetFieldDefinitionAsync(key);
                selectionFieldDefinition = fieldDefinition.AsSelectionFieldDefinition();
            }

            var properties = await selectionFieldDefinition.GetPropertiesAsync(RefreshCancellationToken());

            var choices = properties.AcceptedValues;

            dropdownField.choices = choices.ToList();
            dropdownField.SetValueWithoutNotify(metadata.SelectedValue);
            dropdownField.RegisterValueChangedCallback(evt =>
            {
                metadata.SelectedValue = evt.newValue;
                m_MetadataValues[key] = metadata;
            });
        }

        async Task PoplateMultiSelectionAsync(string key, ISelectionFieldDefinition selectionFieldDefinition, MultiSelectionMetadata metadata, VisualElement visualElement, bool canUpdate = true)
        {
            if (!canUpdate)
            {
                PopulateLabel(string.Join(", ", metadata.SelectedValues), visualElement);
                return;
            }

            var parent = visualElement.Q("MultiSelection");

            if (selectionFieldDefinition == null)
            {
                var fieldDefinition = await m_AddMetadataController.GetFieldDefinitionAsync(key);
                selectionFieldDefinition = fieldDefinition.AsSelectionFieldDefinition();
            }

            var properties = await selectionFieldDefinition.GetPropertiesAsync(RefreshCancellationToken());

            var choices = properties.AcceptedValues;

            foreach (var choice in choices)
            {
                var toggle = new Toggle
                {
                    style =
                    {
                        flexDirection = FlexDirection.RowReverse,
                        alignSelf = Align.FlexStart
                    }
                };
                parent.Add(toggle);
                toggle.style.display = DisplayStyle.Flex;

                toggle.label = choice;
                toggle.SetValueWithoutNotify(metadata.SelectedValues.Contains(choice));
                toggle.RegisterCallback<ChangeEvent<bool>>(evt =>
                {
                    if (evt.newValue)
                    {
                        metadata.SelectedValues.Add(choice);
                    }
                    else
                    {
                        metadata.SelectedValues.Remove(choice);
                    }

                    m_MetadataValues[key] = metadata;
                });
            }
        }

        void PopulateUrl(string key, UrlMetadata metadata, VisualElement visualElement, bool canUpdate = true)
        {
            if (!canUpdate)
            {
                var urlLabel = string.IsNullOrEmpty(metadata.Label) ? metadata.Uri.ToString() : metadata.Label;
                PopulateLabel($"<a href=\"{metadata.Uri}\">{urlLabel}</a>", visualElement);
                return;
            }

            var urlField = visualElement.Q("Url");
            urlField.style.display = DisplayStyle.Flex;

            var label = urlField.Q<TextField>("UrlLabel");
            label.SetValueWithoutNotify(metadata.Label);
            label.RegisterValueChangedCallback(evt =>
            {
                metadata.Label = evt.newValue;
                m_MetadataValues[key] = metadata;
            });

            var url = urlField.Q<TextField>("Uri");
            url.SetValueWithoutNotify(metadata.Uri?.ToString());
            url.RegisterValueChangedCallback(evt =>
            {
                if (Uri.TryCreate(evt.newValue, UriKind.Absolute, out var uri))
                {
                    metadata.Uri = uri;
                    m_MetadataValues[key] = metadata;
                }
            });
        }

        void AddMetadata()
        {
            m_AddMetadataController.Show(m_MetadataKeys, OnMetadataSelected);
        }

        void OnMetadataSelected(IFieldDefinition fieldDefinition, FieldDefinitionProperties properties)
        {
            if (fieldDefinition == null) return;

            var key = fieldDefinition.Descriptor.FieldKey;

            m_MetadataKeys.Add(key);

            var visualElement = CreateMetadataElement(key);

            switch (properties.Type)
            {
                case FieldDefinitionType.Boolean:
                    var boolMetadata = new BooleanMetadata();
                    m_MetadataValues[key] = boolMetadata;

                    PopulateBoolean(key, boolMetadata, visualElement);
                    break;

                case FieldDefinitionType.Number:
                    var numberMetadata = new NumberMetadata();
                    m_MetadataValues[key] = numberMetadata;

                    PopulateNumber(key, numberMetadata, visualElement);
                    break;

                case FieldDefinitionType.Selection:
                    var selectionProperties = properties.AsSelectionFieldDefinitionProperties();
                    if (selectionProperties.Multiselection)
                    {
                        var multiselectionField = new MultiSelectionMetadata();
                        m_MetadataValues[key] = multiselectionField;

                        _ = PoplateMultiSelectionAsync(key, fieldDefinition.AsSelectionFieldDefinition(), multiselectionField, visualElement);
                    }
                    else
                    {
                        _ = PopulateSingleSelectionAsync(key, fieldDefinition.AsSelectionFieldDefinition(), new SingleSelectionMetadata(), visualElement);
                    }

                    break;

                case FieldDefinitionType.Url:
                    PopulateUrl(key, new UrlMetadata(), visualElement);
                    break;

                case FieldDefinitionType.Timestamp:
                    var timestampMetadata = new DateTimeMetadata(DateTime.UtcNow);
                    m_MetadataValues[key] = timestampMetadata;

                    PopulateTimestamp(key, timestampMetadata, visualElement);
                    break;

                default:
                    var stringMetadata = new StringMetadata();
                    m_MetadataValues[key] = stringMetadata;

                    PopulateText(key, stringMetadata, visualElement);
                    break;
            }
        }

        VisualElement CreateMetadataElement(string key, bool canUpdate = true)
        {
            var visualElement = m_Template.Instantiate();
            _ = GetDisplayNameAsync(visualElement.Q<Label>("MetadataKey"), key);

            m_ContentContainer.Add(visualElement);

            var contextMenu = new ContextMenuController(visualElement);
            contextMenu.SetEnabled(canUpdate);
            if (canUpdate)
            {
                contextMenu.RegisterButtonAction("Remove", () =>
                {
                    m_MetadataKeysToRemove.Add(key);
                    m_MetadataKeys.Remove(key);
                    m_MetadataValues.Remove(key);
                    m_ContentContainer.Remove(visualElement);
                });
            }

            return visualElement;
        }

        async Task GetDisplayNameAsync(TextElement label, string key)
        {
            label.text = key;

            var displayName = await m_AddMetadataController.GetFieldDefinitionNameAsync(key);
            if (!string.IsNullOrEmpty(displayName))
            {
                label.text = displayName;
            }
        }

        CancellationToken RefreshCancellationToken()
        {
            if (m_GetterCancellationTokenSource != null)
            {
                m_GetterCancellationTokenSource.Cancel();
                m_GetterCancellationTokenSource.Dispose();
            }

            m_GetterCancellationTokenSource = new CancellationTokenSource();
            return m_GetterCancellationTokenSource.Token;
        }
    }
}
