namespace Unity.Cloud.Documentation.Assets
{
#pragma warning disable S4487 // Unread "private" fields should be removed
#pragma warning disable S1186 // Methods should not be empty

    #region Example_UIClass

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using UnityEngine;
    using Unity.Cloud.Assets;
    using Unity.Cloud.Identity;

    public class UseCaseAssetMetadataExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public UseCaseAssetMetadataExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    #region HelperClass_BooleanDisplay

    public interface IMetadataValueDisplayer
    {
        MetadataValue Value { get; }

        bool IsValid { get; }

        void Display();
    }

    public class BooleanMetadataValueDisplayer : IMetadataValueDisplayer
    {
        readonly BooleanMetadata m_Boolean;

        public MetadataValue Value => m_Boolean;

        public bool IsValid => true;

        public BooleanMetadataValueDisplayer(BooleanMetadata value)
        {
            m_Boolean = value;
        }

        public void Display()
        {
            m_Boolean.Value = GUILayout.Toggle(m_Boolean.Value, "Is enabled");
        }
    }

    #endregion

    #region HelperClass_NumberDisplay

    public class NumberMetadataValueDisplayer : IMetadataValueDisplayer
    {
        readonly NumberMetadata m_Number;

        public MetadataValue Value => m_Number;

        public bool IsValid { get; private set; }

        public NumberMetadataValueDisplayer(NumberMetadata value)
        {
            m_Number = value;
        }

        public void Display()
        {
            var number = GUILayout.TextField(m_Number.Value.ToString());
            if (string.IsNullOrWhiteSpace(number))
            {
                number = "0";
            }

            if (number.StartsWith('.'))
            {
                number = number.Insert(0, "0");
            }
            else if (number.EndsWith('.'))
            {
                number = number.Insert(number.Length, "0");
            }

            if (double.TryParse(number, out var parsedNumber))
            {
                m_Number.Value = parsedNumber;
                IsValid = true;
            }
            else
            {
                IsValid = false;
                GUILayout.Label("Invalid number");
            }
        }
    }

    #endregion

    #region HelperClass_UrlDisplay

    public class UrlMetadataValueDisplayer : IMetadataValueDisplayer
    {
        readonly UrlMetadata m_UrlMetadata;
        string m_Url;

        public MetadataValue Value => m_UrlMetadata;

        public bool IsValid { get; private set; }

        public UrlMetadataValueDisplayer(UrlMetadata value)
        {
            m_UrlMetadata = value;
            m_Url = m_UrlMetadata.Uri.ToString();
        }

        public void Display()
        {
            m_UrlMetadata.Label = GUILayout.TextField(m_UrlMetadata.Label);

            m_Url = GUILayout.TextField(m_Url);
            if (Uri.TryCreate(m_Url, UriKind.Absolute, out var uri))
            {
                m_UrlMetadata.Uri = uri;
                IsValid = true;
            }
            else
            {
                IsValid = false;
                GUILayout.Label("Invalid URL");
            }
        }
    }

    #endregion

    #region HelperClass_SingleSelectionDisplay

    public class SingleSelectionMetadataValueDisplayer : IMetadataValueDisplayer
    {
        readonly SingleSelectionMetadata m_SelectionMetadata;
        readonly HashSet<string> m_AcceptedValues = new();

        public MetadataValue Value => m_SelectionMetadata;

        public bool IsValid { get; private set; }

        public SingleSelectionMetadataValueDisplayer(SingleSelectionMetadata value, FieldDefinitionDescriptor definitionDescriptor)
        {
            m_SelectionMetadata = value;
            _ = PopulateAcceptedValues(definitionDescriptor);
        }

        async Task PopulateAcceptedValues(FieldDefinitionDescriptor descriptor)
        {
            var fieldDefinition = await PlatformServices.AssetRepository.GetFieldDefinitionAsync(descriptor, default);
            if (fieldDefinition.Type == FieldDefinitionType.Selection)
                m_AcceptedValues.UnionWith(fieldDefinition.AsSelectionFieldDefinition().AcceptedValues);
        }

        public void Display()
        {
            GUILayout.Label("Accepted values: " + string.Join(", ", m_AcceptedValues));

            m_SelectionMetadata.SelectedValue = GUILayout.TextField(m_SelectionMetadata.SelectedValue);

            IsValid = m_AcceptedValues.Contains(m_SelectionMetadata.SelectedValue);
            if (!IsValid)
            {
                GUILayout.Label("Invalid value");
            }
        }
    }

    #endregion

    #region HelperClass_MultiSelectionDisplay

    public class MultiSelectionMetadataValueDisplayer : IMetadataValueDisplayer
    {
        readonly MultiSelectionMetadata m_SelectionMetadata;
        readonly HashSet<string> m_AcceptedValues = new();
        string m_SelectedValues;

        public MetadataValue Value => m_SelectionMetadata;

        public bool IsValid { get; private set; }

        public MultiSelectionMetadataValueDisplayer(MultiSelectionMetadata value, FieldDefinitionDescriptor descriptor)
        {
            m_SelectionMetadata = value;
            m_SelectedValues = string.Join(", ", m_SelectionMetadata.SelectedValues);
            _ = PopulateAcceptedValues(descriptor);
        }

        async Task PopulateAcceptedValues(FieldDefinitionDescriptor descriptor)
        {
            var fieldDefinition = await PlatformServices.AssetRepository.GetFieldDefinitionAsync(descriptor, default);
            if (fieldDefinition.Type == FieldDefinitionType.Selection)
                m_AcceptedValues.UnionWith(fieldDefinition.AsSelectionFieldDefinition().AcceptedValues);
        }

        public void Display()
        {
            GUILayout.Label("Accepted values: " + string.Join(", ", m_AcceptedValues));

            m_SelectedValues = GUILayout.TextField(m_SelectedValues);

            IsValid = true;

            var selectedValues = new List<string>();
            var selectedValuesString = m_SelectedValues.Split(',');
            foreach (var selectedValue in selectedValuesString)
            {
                var value = selectedValue.Trim();
                if (string.IsNullOrWhiteSpace(value)) continue;

                if (!m_AcceptedValues.Contains(value))
                {
                    IsValid = false;
                    break;
                }

                selectedValues.Add(value);
            }

            if (!IsValid)
            {
                GUILayout.Label("Invalid value");
            }
            else
            {
                m_SelectionMetadata.SelectedValues = selectedValues;
            }
        }
    }

    #endregion

    #region HelperClass_TextDisplay

    public class TextMetadataValueDisplayer : IMetadataValueDisplayer
    {
        readonly StringMetadata m_Text;

        public MetadataValue Value => m_Text;

        public bool IsValid => true;

        public TextMetadataValueDisplayer(StringMetadata value)
        {
            m_Text = value;
        }

        public void Display()
        {
            m_Text.Value = GUILayout.TextField(m_Text.Value);
        }
    }

    #endregion

    public class UseCaseManageAssetMetadataExample : IAssetManagementUI
    {
        readonly UseCaseAssetMetadataExampleBehaviour m_Behaviour;

        public UseCaseManageAssetMetadataExample(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = new UseCaseAssetMetadataExampleBehaviour(behaviour);
        }

        #region Example_UIContent

        enum MetadataType
        {
            none,
            metadata
        }

        IAsset m_CurrentAsset;
        MetadataType m_MetadataType;
        Vector2 m_MetadataListScrollPosition;

        string m_CurrentMetadataKey;
        IMetadataValueDisplayer m_MetadataValueDisplayer;

        string m_NewKey = string.Empty;
        string m_NewValue = string.Empty;

        public void OnGUI()
        {
            if (!m_Behaviour.IsProjectSelected) return;

            if (m_CurrentAsset != m_Behaviour.CurrentAsset)
            {
                m_CurrentAsset = m_Behaviour.CurrentAsset;
                m_MetadataType = MetadataType.none;
                m_CurrentMetadataKey = null;
                m_MetadataValueDisplayer = null;
            }

            if (m_CurrentAsset == null)
            {
                GUILayout.Label(" ! No asset selected !");
                return;
            }

            GUILayout.BeginVertical();

            DisplayMetadataTypeToggle();

            if (m_MetadataType == MetadataType.none)
            {
                GUILayout.Label(" ! No metadata selected !");
                GUILayout.EndVertical();
                return;
            }

            if (m_Behaviour.Metadata == null)
            {
                GUILayout.Label("Loading...");
                GUILayout.EndVertical();
                return;
            }

            ListAssetMetadata();

            AddMetadata();

            GUILayout.EndVertical();

            DisplayCurrentMetadataValue();
        }

        void DisplayMetadataTypeToggle()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Refresh"))
            {
                m_MetadataType = MetadataType.metadata;
                _ = m_Behaviour.GetMetadataAsync(m_Behaviour.CurrentAsset.Metadata);
            }

            GUILayout.EndHorizontal();
        }

        void ListAssetMetadata()
        {
            m_MetadataListScrollPosition = GUILayout.BeginScrollView(m_MetadataListScrollPosition);
            foreach (var key in m_Behaviour.Metadata.Keys)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label(key);

                if (GUILayout.Button("Select"))
                {
                    m_CurrentMetadataKey = key;

                    var metadataValue = m_Behaviour.Metadata[key];
                    m_MetadataValueDisplayer = metadataValue.ValueType switch
                    {
                        MetadataValueType.Boolean => new BooleanMetadataValueDisplayer(metadataValue.AsBoolean()),
                        MetadataValueType.Number => new NumberMetadataValueDisplayer(metadataValue.AsNumber()),
                        MetadataValueType.Url => new UrlMetadataValueDisplayer(metadataValue.AsUrl()),
                        MetadataValueType.SingleSelection => new SingleSelectionMetadataValueDisplayer(metadataValue.AsSingleSelection(),
                            new FieldDefinitionDescriptor(m_Behaviour.CurrentOrganization.Id, key)),
                        MetadataValueType.MultiSelection => new MultiSelectionMetadataValueDisplayer(metadataValue.AsMultiSelection(),
                            new FieldDefinitionDescriptor(m_Behaviour.CurrentOrganization.Id, key)),
                        _ => new TextMetadataValueDisplayer(metadataValue.AsText())
                    };
                }

                if (GUILayout.Button("Remove"))
                {
                    _ = m_Behaviour.RemoveMetadata(key);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        void AddMetadata()
        {
            if (m_Behaviour.Metadata == null) return;

            GUILayout.BeginVertical();

            GUILayout.Label("Key:");
            m_NewKey = GUILayout.TextField(m_NewKey);
            GUILayout.Label("Value:");
            m_NewValue = GUILayout.TextField(m_NewValue);

            GUI.enabled = !string.IsNullOrWhiteSpace(m_NewKey) && !string.IsNullOrWhiteSpace(m_NewValue);

            try
            {
                m_NewValue ??= string.Empty;

                if (GUILayout.Button("Add Boolean"))
                {
                    _ = m_Behaviour.UpdateAsync(m_NewKey, new BooleanMetadata(bool.Parse(m_NewValue)));
                }

                if (GUILayout.Button("Add Number"))
                {
                    _ = m_Behaviour.UpdateAsync(m_NewKey, new NumberMetadata(double.Parse(m_NewValue)));
                }

                if (GUILayout.Button("Add Timestamp"))
                {
                    _ = m_Behaviour.UpdateAsync(m_NewKey, new DateTimeMetadata(DateTime.Parse(m_NewValue)));
                }

                if (GUILayout.Button("Add Multi-selection"))
                {
                    _ = m_Behaviour.UpdateAsync(m_NewKey, new MultiSelectionMetadata(m_NewValue.Split(',').Select(x => x.Trim()).ToArray()));
                }

                if (GUILayout.Button("Add String"))
                {
                    _ = m_Behaviour.UpdateAsync(m_NewKey, new StringMetadata(m_NewValue));
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Could not add metadata: " + e.Message);
            }

            GUI.enabled = true;

            GUILayout.EndVertical();
        }

        void DisplayCurrentMetadataValue()
        {
            if (m_CurrentMetadataKey == null)
            {
                GUILayout.Label(" ! No metadata value selected !");
                return;
            }

            GUILayout.BeginVertical();

            GUILayout.Label(m_CurrentMetadataKey);

            m_MetadataValueDisplayer.Display();

            GUI.enabled = m_MetadataValueDisplayer.IsValid;

            if (GUILayout.Button("Update"))
            {
                _ = m_Behaviour.UpdateAsync(m_CurrentMetadataKey, m_MetadataValueDisplayer.Value);
            }

            GUI.enabled = true;

            GUILayout.EndVertical();
        }

        #endregion
    }

    class UseCaseAssetMetadataExampleBehaviour
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public IOrganization CurrentOrganization => m_Behaviour.CurrentOrganization;
        public bool IsProjectSelected => m_Behaviour.IsProjectSelected;
        public IAsset CurrentAsset => m_Behaviour.CurrentAsset;

        public UseCaseAssetMetadataExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_Behaviour_FetchMetadata

        IMetadataContainer MetadataContainer { get; set; }

        public IReadOnlyDictionary<string, MetadataValue> Metadata { get; private set; }

        public async Task GetMetadataAsync(IMetadataContainer metadataContainer)
        {
            MetadataContainer = metadataContainer;
            Metadata = null;

            if (metadataContainer == null) return;

            var result = MetadataContainer.Query().ExecuteAsync(CancellationToken.None);
            var metadata = new Dictionary<string, MetadataValue>();
            await foreach (var item in result)
            {
                metadata[item.Key] = item.Value;
            }

            Metadata = metadata;
            Debug.Log("Successfully fetched metadata.");
        }

        #endregion

        #region Example_Behaviour_UpdateMetadata

        public async Task UpdateAsync(string key, MetadataValue value)
        {
            try
            {
                await MetadataContainer.AddOrUpdateAsync(key, value, CancellationToken.None);
                Debug.Log("Successfully updated metadata.");
            }
            catch (Exception e)
            {
                Debug.LogError("Could not update metadata: " + e.Message);
            }
        }

        #endregion

        #region Example_Behaviour_RemoveMetadata

        public async Task RemoveMetadata(string key)
        {
            try
            {
                await MetadataContainer.RemoveAsync(new[] {key}, CancellationToken.None);
                Debug.Log("Successfully removed metadata.");
            }
            catch (Exception e)
            {
                Debug.LogError("Could not remove metadata: " + e.Message);
            }
        }

        #endregion
    }
}
