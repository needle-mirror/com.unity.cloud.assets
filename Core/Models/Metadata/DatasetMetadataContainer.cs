using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class DatasetMetadataContainer : MetadataContainerEntity
    {
        readonly DatasetDescriptor m_Descriptor;

        protected override OrganizationId OrganizationId => m_Descriptor.OrganizationId;

        internal DatasetMetadataContainer(DatasetDescriptor datasetDescriptor, DatasetFields field, IAssetDataSource assetDataSource)
            : base(assetDataSource, MetadataContainerSpecification.metadata)
        {
            m_Descriptor = datasetDescriptor;
            m_BuildFieldsFilter = () => new FieldsFilter
            {
                AssetFields = AssetFields.datasets,
                DatasetFields = field,
            };
        }

        protected override async Task<IMetadataInfo> GetMetadataInfoAsync(FieldsFilter filter, CancellationToken cancellationToken)
        {
            return await m_AssetDataSource.GetDatasetAsync(m_Descriptor, filter, cancellationToken);
        }

        protected override Task ExecuteAddOrUpdateAsync(Dictionary<string, object> properties, CancellationToken cancellationToken)
        {
            var data = new DatasetUpdateData();
            switch (m_ContainerSpecification)
            {
                case MetadataContainerSpecification.metadata:
                    data.Metadata = properties;
                    break;
            }
            return m_AssetDataSource.UpdateDatasetAsync(m_Descriptor, data, cancellationToken);
        }

        protected override Task DatasourceRemoveAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            return m_AssetDataSource.RemoveDatasetMetadataAsync(m_Descriptor, m_ContainerSpecification.ToString(), keys, cancellationToken);
        }
    }
}
