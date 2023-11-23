using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class FieldDefinitionEntity : IFieldDefinition
    {
        readonly IAssetDataSource m_DataSource;

        /// <inheritdoc/>
        public FieldDefinitionDescriptor Descriptor { get; }

        /// <inheritdoc/>
        public FieldDefinitionType Type { get; set; }

        /// <inheritdoc/>
        public string Status { get; set; }

        /// <inheritdoc/>
        public string DisplayName { get; set; }

        /// <inheritdoc/>
        public AuthoringInfo AuthoringInfo { get; set; }

        /// <inheritdoc/>
        public IEnumerable<string> AcceptedValues { get; set; }

        /// <inheritdoc/>
        public bool? Multiselection { get; set; }

        internal FieldDefinitionEntity(IAssetDataSource dataSource, FieldDefinitionDescriptor descriptor)
        {
            m_DataSource = dataSource;
            Descriptor = descriptor;
        }

        /// <inheritdoc/>
        public async Task RefreshAsync(CancellationToken cancellationToken)
        {
            var data = await m_DataSource.GetFieldDefinitionAsync(Descriptor, cancellationToken);
            this.MapFrom(data);
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(IFieldDefinitionUpdate definitionUpdate, CancellationToken cancellationToken)
        {
            await m_DataSource.UpdateFieldDefinitionAsync(Descriptor, definitionUpdate.From(), cancellationToken);
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
