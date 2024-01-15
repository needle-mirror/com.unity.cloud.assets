using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class FileMetadataContainer : MetadataContainerEntity
    {
        readonly FileDescriptor m_Descriptor;

        protected override OrganizationId OrganizationId => m_Descriptor.OrganizationGenesisId;

        internal FileMetadataContainer(FileDescriptor fileDescriptor, FileFields field, IAssetDataSource assetDataSource, Dictionary<string, MetadataValue> properties = null)
            : base(assetDataSource, field == FileFields.metadata ? MetadataContainerSpecification.metadata : MetadataContainerSpecification.systemMetadata, properties)
        {
            m_Descriptor = fileDescriptor;
            m_BuildFieldsFilter = () => new FieldsFilter
            {
                AssetFields = AssetFields.files,
                FileFields = field,
            };
        }

        protected override async Task<IMetadataInfo> GetMetadataInfoAsync(FieldsFilter filter, CancellationToken cancellationToken)
        {
            return await m_AssetDataSource.GetFileAsync(m_Descriptor, filter, cancellationToken);
        }

        protected override Task ExecuteAddOrUpdateAsync(Dictionary<string, object> properties, CancellationToken cancellationToken)
        {
            var data = new FileBaseData();
            switch (m_ContainerSpecification)
            {
                case MetadataContainerSpecification.metadata:
                    data.Metadata = properties;
                    break;
                case MetadataContainerSpecification.systemMetadata:
                    data.SystemMetadata = properties;
                    break;
            }
            return m_AssetDataSource.UpdateFileAsync(m_Descriptor, data, cancellationToken);
        }

        protected override Task DatasourceRemoveAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            return m_AssetDataSource.RemoveFileMetadataAsync(m_Descriptor, m_ContainerSpecification.ToString(), keys, cancellationToken);
        }
    }
}
