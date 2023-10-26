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
        Task<IDatasetData> CreateDatasetAsync(AssetDescriptor assetDescriptor, IDatasetBaseData datasetCreation, CancellationToken token);

        Task<IDatasetData> GetDatasetAsync(DatasetDescriptor datasetDescriptor, FieldsFilter includedFieldsFilter, CancellationToken token);

        Task<IDatasetData> GetDatasetBySystemTagAsync(AssetDescriptor assetDescriptor, string systemTag, FieldsFilter includedFieldsFilter, CancellationToken token);

        Task UpdateDatasetAsync(DatasetDescriptor datasetDescriptor, IDatasetUpdateData datasetUpdate, CancellationToken token);

        Task ReferenceFileFromDatasetAsync(DatasetDescriptor datasetDescriptor, DatasetId sourceDatasetId, string filePath, CancellationToken token);

        Task RemoveFileFromDatasetAsync(DatasetDescriptor datasetDescriptor, string filePath, CancellationToken token);

        Task<bool> CheckDatasetIsInProjectAssetVersionAsync(DatasetDescriptor datasetDescriptor, CancellationToken token);

        Task RemoveDatasetMetadataAsync(DatasetDescriptor datasetDescriptor, string metadataType, IEnumerable<string> keys, CancellationToken token);
    }
}
