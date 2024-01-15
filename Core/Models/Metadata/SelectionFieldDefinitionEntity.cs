using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Assets
{
    class SelectionFieldDefinitionEntity : FieldDefinitionEntity, ISelectionFieldDefinition
    {
        /// <inheritdoc/>
        public IEnumerable<string> AcceptedValues { get; set; }

        /// <inheritdoc/>
        public bool Multiselection { get; set; }

        internal SelectionFieldDefinitionEntity(IAssetDataSource dataSource, FieldDefinitionDescriptor descriptor)
            : base(dataSource, descriptor) { }

        /// <inheritdoc/>
        public async Task SetSelectionValuesAsync(IEnumerable<string> acceptedValues, CancellationToken cancellationToken)
        {
            var definitionUpdate = new FieldDefinitionBaseData
            {
                AcceptedValues = acceptedValues.ToArray()
            };
            await m_DataSource.UpdateFieldDefinitionAsync(Descriptor, definitionUpdate, cancellationToken);
            if (cancellationToken.IsCancellationRequested) return;
            await RefreshAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task AddSelectionValuesAsync(IEnumerable<string> acceptedValues, CancellationToken cancellationToken)
        {
            await m_DataSource.AddAcceptedValuesToFieldDefinitionAsync(Descriptor, acceptedValues, cancellationToken);
            if (cancellationToken.IsCancellationRequested) return;
            await RefreshAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task RemoveSelectionValuesAsync(IEnumerable<string> acceptedValues, CancellationToken cancellationToken)
        {
            await m_DataSource.RemoveAcceptedValuesFromFieldDefinitionAsync(Descriptor, acceptedValues, cancellationToken);
            if (cancellationToken.IsCancellationRequested) return;
            await RefreshAsync(cancellationToken);
        }
    }
}
