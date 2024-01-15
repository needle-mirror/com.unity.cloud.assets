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
        readonly bool m_IsSystemMetadata;
        readonly VisualTreeAsset m_Template;

        readonly AddMetadataPopupController m_AddMetadataController;

        IMetadataContainer m_MetadataContainer;
        readonly List<string> m_MetadataKeys = new();
        readonly Dictionary<string, object> m_MetadataValues = new();
        readonly List<string> m_MetadataKeysToRemove = new();

        CancellationTokenSource m_GetterCancellationTokenSource;

        public MetadataController(VisualElement contentContainer, bool isSystemMetadata, VisualTreeAsset template, AddMetadataPopupController addMetadataController)
        {
            m_ContentContainer = contentContainer;
            m_IsSystemMetadata = isSystemMetadata;
            m_Template = template;

            m_AddMetadataController = addMetadataController;

            var addButton = m_ContentContainer.parent.Q<Button>("AddMetadataButton");
            addButton.clicked += AddMetadata;
        }

        public async Task PopulateMetadataAsync(IAsset asset)
        {
            var cancellationToken = RefreshCancellationToken();

            m_MetadataContainer = m_IsSystemMetadata ? asset.SystemMetadata : asset.Metadata;

            await PopulateMetadataAsync(cancellationToken);
        }

        public async Task PopulateMetadataAsync(IDataset dataset)
        {
            var cancellationToken = RefreshCancellationToken();

            m_MetadataContainer = m_IsSystemMetadata ? dataset.SystemMetadata : dataset.Metadata;

            if (cancellationToken.IsCancellationRequested) return;

            await PopulateMetadataAsync(cancellationToken);
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

        async Task PopulateMetadataAsync(CancellationToken cancellationToken)
        {
            var metadata = await m_MetadataContainer.Query().ExecuteAsync(cancellationToken);

            foreach (var kvp in metadata)
            {
                m_MetadataKeys.Add(kvp.Key);

                var visualElement = CreateMetadataElement(kvp.Key);

                await ParseValueAsync(kvp.Key, kvp.Value, visualElement, cancellationToken);
            }
        }

        async Task ParseValueAsync(string key, IMetadataValue value, VisualElement visualElement, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            switch (value.ValueType)
            {
                case MetadataValueType.Unknown:
                    await WaitOnUnknownAsync(key, value, visualElement, cancellationToken);
                    break;

                case MetadataValueType.Boolean:
                    var boolField = visualElement.Q<Toggle>("Boolean");
                    boolField.style.display = DisplayStyle.Flex;

                    boolField.SetValueWithoutNotify(value.AsBoolean());
                    boolField.RegisterValueChangedCallback(evt =>
                    {
                        m_MetadataValues[key] = evt.newValue;
                    });
                    break;

                case MetadataValueType.Number:
                    var numberField = visualElement.Q<DoubleField>("Number");
                    numberField.style.display = DisplayStyle.Flex;

                    numberField.SetValueWithoutNotify(value.AsNumber());
                    numberField.RegisterValueChangedCallback(evt =>
                    {
                        m_MetadataValues[key] = evt.newValue;
                    });
                    break;

                case MetadataValueType.SingleSelection:
                    var singleSelectionField = visualElement.Q<DropdownField>("SingleSelection");
                    singleSelectionField.style.display = DisplayStyle.Flex;

                    await PopulateSingleSelectionAsync(key, value.AsSingleSelection(), singleSelectionField);
                    break;

                case MetadataValueType.MultiSelection:
                    await PoplateMultiSelectionAsync(key, value.AsMultiSelection(), visualElement);
                    break;

                case MetadataValueType.Url:
                    var urlField = visualElement.Q("Url");
                    urlField.style.display = DisplayStyle.Flex;

                    PopulateUrl(key, value.AsUrl(), urlField);
                    break;

                case MetadataValueType.Timestamp:
                    var timestampField = visualElement.Q<TextField>("Text");
                    timestampField.style.display = DisplayStyle.Flex;

                    timestampField.SetValueWithoutNotify(value.ToString());
                    timestampField.RegisterValueChangedCallback(evt =>
                    {
                        if (DateTime.TryParse(evt.newValue, out var timestamp))
                        {
                            m_MetadataValues[key] = timestamp;
                        }
                    });
                    break;

                default:
                    var textField = visualElement.Q<TextField>("Text");
                    textField.style.display = DisplayStyle.Flex;

                    textField.SetValueWithoutNotify(value.ToString());
                    textField.RegisterValueChangedCallback(evt =>
                    {
                        m_MetadataValues[key] = evt.newValue;
                    });
                    break;
            }
        }

        async Task WaitOnUnknownAsync(string key, IMetadataValue value, VisualElement visualElement, CancellationToken cancellationToken)
        {
            // Apply a timeout to prevent waiting forever
            var timeoutCancellationSource = new CancellationTokenSource(3000);
            while (value.ValueType == MetadataValueType.Unknown)
            {
                await Task.Delay(100, timeoutCancellationSource.Token);

                if (cancellationToken.IsCancellationRequested || timeoutCancellationSource.IsCancellationRequested) return;
            }

            if (cancellationToken.IsCancellationRequested) return;

            await ParseValueAsync(key, value, visualElement, cancellationToken);
        }

        async Task PopulateSingleSelectionAsync(string key, SingleSelectionMetadata metadata, DropdownField dropdownField)
        {
            var choices = await metadata.GetAcceptedValuesAsync();

            dropdownField.choices = choices.ToList();
            dropdownField.SetValueWithoutNotify(metadata.SelectedValue);
            dropdownField.RegisterValueChangedCallback(evt =>
            {
                metadata.SelectedValue = evt.newValue;
                m_MetadataValues[key] = metadata;
            });
        }

        async Task PoplateMultiSelectionAsync(string key, MultiSelectionMetadata metadata, VisualElement visualElement)
        {
            var fieldTemplate = visualElement.Q<TemplateContainer>("MultiSelectionMetadataTemplate");

            var choices = await metadata.GetAcceptedValuesAsync();

            foreach (var choice in choices)
            {
                var toggle = fieldTemplate.templateSource.Instantiate().Q<Toggle>();
                fieldTemplate.parent.Add(toggle);
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

        void PopulateUrl(string key, UrlMetadata metadata, VisualElement visualElement)
        {
            var label = visualElement.Q<TextField>("UrlLabel");
            label.SetValueWithoutNotify(metadata.Label);
            label.RegisterValueChangedCallback(evt =>
            {
                metadata.Label = evt.newValue;
                m_MetadataValues[key] = metadata;
            });

            var urlField = visualElement.Q<TextField>("Uri");
            urlField.SetValueWithoutNotify(metadata.Uri?.ToString());
            urlField.RegisterValueChangedCallback(evt =>
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

        void OnMetadataSelected(IFieldDefinition fieldDefinition)
        {
            var key = fieldDefinition.Descriptor.FieldKey;

            m_MetadataKeys.Add(key);

            var visualElement = CreateMetadataElement(key);

            switch (fieldDefinition.Type)
            {
                case FieldDefinitionType.Boolean:
                    m_MetadataValues[key] = false;

                    var boolField = visualElement.Q<Toggle>("Boolean");
                    boolField.style.display = DisplayStyle.Flex;

                    boolField.SetValueWithoutNotify(false);
                    boolField.RegisterValueChangedCallback(evt =>
                    {
                        m_MetadataValues[key] = evt.newValue;
                    });
                    break;

                case FieldDefinitionType.Number:
                    m_MetadataValues[key] = 0;

                    var numberField = visualElement.Q<DoubleField>("Number");
                    numberField.style.display = DisplayStyle.Flex;

                    numberField.SetValueWithoutNotify(0);
                    numberField.RegisterValueChangedCallback(evt =>
                    {
                        m_MetadataValues[key] = evt.newValue;
                    });
                    break;

                case FieldDefinitionType.Selection:
                    var selectionFieldDefinition = fieldDefinition.AsSelectionFieldDefinition();
                    if (selectionFieldDefinition.Multiselection)
                    {
                        var multiselectionField = new MultiSelectionMetadata(selectionFieldDefinition);
                        m_MetadataValues[key] = multiselectionField;

                        _ = PoplateMultiSelectionAsync(key, multiselectionField, visualElement);
                    }
                    else
                    {
                        var singleSelectionField = visualElement.Q<DropdownField>("SingleSelection");
                        singleSelectionField.style.display = DisplayStyle.Flex;

                        _ = PopulateSingleSelectionAsync(key, new SingleSelectionMetadata(selectionFieldDefinition), singleSelectionField);
                    }

                    break;

                case FieldDefinitionType.Url:
                    var urlField = visualElement.Q("Url");
                    urlField.style.display = DisplayStyle.Flex;

                    PopulateUrl(key, new UrlMetadata(), urlField);
                    break;

                case FieldDefinitionType.Timestamp:
                    m_MetadataValues[key] = DateTime.UtcNow;

                    var timestampField = visualElement.Q<TextField>("Text");
                    timestampField.style.display = DisplayStyle.Flex;

                    timestampField.SetValueWithoutNotify(DateTime.UtcNow.ToString());
                    timestampField.RegisterValueChangedCallback(evt =>
                    {
                        if (DateTime.TryParse(evt.newValue, out var timestamp))
                        {
                            m_MetadataValues[key] = timestamp;
                        }
                    });
                    break;

                default:
                    m_MetadataValues[key] = string.Empty;

                    var textField = visualElement.Q<TextField>("Text");
                    textField.style.display = DisplayStyle.Flex;

                    textField.SetValueWithoutNotify(string.Empty);
                    textField.RegisterValueChangedCallback(evt =>
                    {
                        m_MetadataValues[key] = evt.newValue;
                    });
                    break;
            }
        }

        VisualElement CreateMetadataElement(string key)
        {
            var visualElement = m_Template.Instantiate();
            visualElement.Q<Label>("MetadataKey").text = key;

            m_ContentContainer.Add(visualElement);

            var contextMenu = new ContextMenuController(visualElement);
            contextMenu.RegisterButtonAction("Remove", () =>
            {
                m_MetadataKeysToRemove.Add(key);
                m_MetadataKeys.Remove(key);
                m_MetadataValues.Remove(key);
                m_ContentContainer.Remove(visualElement);
            });

            return visualElement;
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
