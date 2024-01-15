using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A class for manipulating a single selection metadata value.
    /// </summary>
    public sealed class SingleSelectionMetadata : SelectionMetadata
    {
        /// <summary>
        /// The text value of a metadata field.
        /// </summary>
        public string SelectedValue { get; set; }

        /// <summary>
        /// Returns a new instance of the <see cref="SingleSelectionMetadata"/> class.
        /// </summary>
        /// <param name="selectionFieldDefinition"></param>
        public SingleSelectionMetadata(ISelectionFieldDefinition selectionFieldDefinition)
            : base(selectionFieldDefinition) { }

        internal SingleSelectionMetadata(IAssetDataSource dataSource, FieldDefinitionDescriptor fieldDefinitionDescriptor)
            : base(dataSource, fieldDefinitionDescriptor) { }

        /// <inheritdoc />
        public override object GetValue()
        {
            return SelectedValue ?? string.Empty;
        }

        /// <inheritdoc />
        internal override void SetValue(object value)
        {
            SelectedValue = value?.ToString() ?? string.Empty;
        }
    }
}
