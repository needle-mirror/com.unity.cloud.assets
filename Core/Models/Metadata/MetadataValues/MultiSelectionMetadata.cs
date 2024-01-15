using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A class for manipulating a multi-selection metadata value.
    /// </summary>
    public sealed class MultiSelectionMetadata : SelectionMetadata
    {
        /// <summary>
        /// The list of selected values.
        /// </summary>
        public List<string> SelectedValues { get; set; } = new();

        public MultiSelectionMetadata(ISelectionFieldDefinition selectionFieldDefinition)
            : base(selectionFieldDefinition) { }

        internal MultiSelectionMetadata(IAssetDataSource dataSource, FieldDefinitionDescriptor fieldDefinitionDescriptor)
            : base(dataSource, fieldDefinitionDescriptor) { }

        /// <inheritdoc />
        public override object GetValue()
        {
            return SelectedValues;
        }

        /// <inheritdoc />
        internal override void SetValue(object value)
        {
            SelectedValues = value switch
            {
                null => new List<string>(),
                string stringValue => ParseValue(stringValue),
                IEnumerable<string> stringEnumerable => stringEnumerable.ToList(),
                ICollection collection => collection.Cast<object>().Select(o => o?.ToString() ?? string.Empty).ToList(),
                _ => new List<string> {value.ToString()}
            };
        }

        static List<string> ParseValue(string stringValue)
        {
            var list = new List<string>();

            var splitValues = stringValue.Split(',');
            foreach (var split in splitValues)
            {
                list.Add(split.Trim());
            }

            return list;
        }
    }
}
