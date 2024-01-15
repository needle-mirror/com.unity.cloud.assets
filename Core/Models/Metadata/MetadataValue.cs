using System;
using System.Collections;
using System.Threading.Tasks;

namespace Unity.Cloud.Assets
{
    class MetadataValue : IMetadataValue
    {
        readonly IAssetDataSource m_AssetDataSource;
        readonly FieldDefinitionDescriptor m_FieldDefinitionDescriptor;

        /// <inheritdoc />
        public MetadataValueType ValueType { get; private set; }

        internal object Value { get; }

        internal MetadataValue(object value)
        {
            Value = value;
            ValueType = ParseType(value);
        }

        internal MetadataValue(object value, IAssetDataSource dataSource, FieldDefinitionDescriptor fieldDefinitionDescriptor) : this(value)
        {
            m_AssetDataSource = dataSource;
            m_FieldDefinitionDescriptor = fieldDefinitionDescriptor;
            _ = ValidateAsync();
        }

        async Task ValidateAsync()
        {
            if (ValueType == MetadataValueType.Unknown)
            {
                try
                {
                    var fieldDefinition = await m_AssetDataSource.GetFieldDefinitionAsync(m_FieldDefinitionDescriptor, default);
                    var multiSelection = fieldDefinition.Multiselection ?? false;
                    ValueType = fieldDefinition.Type switch
                    {
                        FieldDefinitionType.Boolean => MetadataValueType.Boolean,
                        FieldDefinitionType.Number => MetadataValueType.Number,
                        FieldDefinitionType.Text => MetadataValueType.Text,
                        FieldDefinitionType.Timestamp => MetadataValueType.Timestamp,
                        FieldDefinitionType.Url => MetadataValueType.Url,
                        FieldDefinitionType.User => MetadataValueType.User,
                        FieldDefinitionType.Selection => multiSelection ? MetadataValueType.MultiSelection : MetadataValueType.SingleSelection,
                        _ => MetadataValueType.Unknown
                    };
                }
                catch (Exception)
                {
                    // ignored - we'll just leave it as unknown
                }
            }
        }

        public override string ToString()
        {
            return Value?.ToString() ?? string.Empty;
        }

        /// <inheritdoc />
        public bool AsBoolean()
        {
            return Value as bool? ?? bool.Parse(Value?.ToString() ?? false.ToString());
        }

        /// <inheritdoc />
        public double AsNumber()
        {
            return Value as double? ?? double.Parse(Value?.ToString() ?? "0");
        }

        /// <inheritdoc />
        public DateTime AsTimestamp()
        {
            return Value as DateTime? ?? DateTime.Parse(Value?.ToString() ?? DateTime.MinValue.ToString());
        }

        /// <inheritdoc />
        public string AsText()
        {
            return ToString();
        }

        /// <inheritdoc />
        public SingleSelectionMetadata AsSingleSelection()
        {
            var singleselection = new SingleSelectionMetadata(m_AssetDataSource, m_FieldDefinitionDescriptor);
            singleselection.SetValue(Value);
            return singleselection;
        }

        /// <inheritdoc />
        public MultiSelectionMetadata AsMultiSelection()
        {
            var multiselection = new MultiSelectionMetadata(m_AssetDataSource, m_FieldDefinitionDescriptor);
            multiselection.SetValue(Value);
            return multiselection;
        }

        /// <inheritdoc />
        public UrlMetadata AsUrl()
        {
            var url = new UrlMetadata();
            url.SetValue(Value);
            return url;
        }

        /// <inheritdoc />
        public string AsUser()
        {
            return ToString();
        }

        static MetadataValueType ParseType(object value)
        {
            return value switch
            {
                bool => MetadataValueType.Boolean,
                ICollection => MetadataValueType.MultiSelection,
                double or int or float or long or short or byte or sbyte or decimal => MetadataValueType.Number,
                DateTime => MetadataValueType.Timestamp,
                string stringValue => UrlMetadata.TryParse(stringValue, out _, out _) ? MetadataValueType.Url : MetadataValueType.Unknown,
                _ => MetadataValueType.Unknown
            };
        }
    }
}
