using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    partial interface IAssetDataSource
        // an object to provide all the low-level functionality.
        // Following methods will be added to the LowLevel manager after we agree on the overall design. Still can be split into several interfaces
    {
        Task<IDatasetData> CreateDatasetAsync(AssetDescriptor assetDescriptor, IDatasetBaseData datasetCreation, CancellationToken cancellationToken);

        Task<IDatasetData> GetDatasetAsync(DatasetDescriptor datasetDescriptor, FieldsFilter includedFieldsFilter, CancellationToken cancellationToken);

        Task<IDatasetData> GetDatasetBySystemTagAsync(AssetDescriptor assetDescriptor, string systemTag, FieldsFilter includedFieldsFilter, CancellationToken cancellationToken);

        Task UpdateDatasetAsync(DatasetDescriptor datasetDescriptor, IDatasetUpdateData datasetUpdate, CancellationToken cancellationToken);

        Task ReferenceFileFromDatasetAsync(DatasetDescriptor datasetDescriptor, DatasetId sourceDatasetId, string filePath, CancellationToken cancellationToken);

        Task RemoveFileFromDatasetAsync(DatasetDescriptor datasetDescriptor, string filePath, CancellationToken cancellationToken);

        Task<bool> CheckDatasetIsInProjectAssetVersionAsync(DatasetDescriptor datasetDescriptor, CancellationToken cancellationToken);

        Task RemoveDatasetMetadataAsync(DatasetDescriptor datasetDescriptor, string metadataType, IEnumerable<string> keys, CancellationToken cancellationToken);
    }
}
