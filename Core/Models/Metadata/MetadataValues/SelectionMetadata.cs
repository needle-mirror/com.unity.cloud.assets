using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A class for manipulating a single selection metadata value.
    /// </summary>
    public abstract class SelectionMetadata : MetadataObject
    {
        readonly IAssetDataSource m_AssetDataSource;
        readonly FieldDefinitionDescriptor m_FieldDefinitionDescriptor;

        readonly ISelectionFieldDefinition m_SelectionFieldDefinition;

        /// <summary>
        /// Returns a new instance of the <see cref="SingleSelectionMetadata"/> class.
        /// </summary>
        /// <param name="selectionFieldDefinition">A field definition of type <see cref="ISelectionFieldDefinition"/>. </param>
        protected SelectionMetadata(ISelectionFieldDefinition selectionFieldDefinition)
        {
            m_SelectionFieldDefinition = selectionFieldDefinition;
        }

        private protected SelectionMetadata(IAssetDataSource dataSource, FieldDefinitionDescriptor fieldDefinitionDescriptor)
        {
            m_AssetDataSource = dataSource;
            m_FieldDefinitionDescriptor = fieldDefinitionDescriptor;
        }

        /// <summary>
        /// Gets the accepted values for the selection.
        /// </summary>
        /// <returns>A task whose result is the collection of accepted values. </returns>
        public async Task<IEnumerable<string>> GetAcceptedValuesAsync()
        {
            if (m_SelectionFieldDefinition != null)
            {
                return m_SelectionFieldDefinition.AcceptedValues;
            }

            if (m_AssetDataSource != null)
            {
                var fieldDefinition = await m_AssetDataSource.GetFieldDefinitionAsync(m_FieldDefinitionDescriptor, default);
                return fieldDefinition.AcceptedValues;
            }

            throw new NotFoundException("Failed to fetch accepted values for selection metadata.");
        }
    }
}
