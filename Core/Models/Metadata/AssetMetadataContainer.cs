using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class AssetMetadataContainer : MetadataContainerEntity
    {
        readonly AssetDescriptor m_Descriptor;

        protected override OrganizationId OrganizationId => m_Descriptor.OrganizationId;

        internal AssetMetadataContainer(AssetDescriptor assetDescriptor, AssetFields field, IAssetDataSource assetDataSource)
            : base(assetDataSource, MetadataContainerSpecification.metadata)
        {
            m_Descriptor = assetDescriptor;
            m_BuildFieldsFilter = () => new FieldsFilter
            {
                AssetFields = field,
            };
        }

        protected override async Task<IMetadataInfo> GetMetadataInfoAsync(FieldsFilter filter, CancellationToken cancellationToken)
        {
            return await m_AssetDataSource.GetAssetAsync(m_Descriptor, filter, cancellationToken);
        }

        protected override Task ExecuteAddOrUpdateAsync(Dictionary<string, object> properties, CancellationToken cancellationToken)
        {
            var data = new AssetUpdateData();
            switch (m_ContainerSpecification)
            {
                case MetadataContainerSpecification.metadata:
                    data.Metadata = properties;
                    break;
            }
            return m_AssetDataSource.UpdateAssetAsync(m_Descriptor, data, cancellationToken);
        }

        protected override Task DatasourceRemoveAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            return m_AssetDataSource.RemoveAssetMetadataAsync(m_Descriptor, m_ContainerSpecification.ToString(), keys, cancellationToken);
        }
    }
}
