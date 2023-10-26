#if UC_MOCK_ASSETS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    partial class MockDataSource : IAssetDataSource
    {
        const string k_Author = "Author";
        const string k_DefaultStatus = "DefaultStatus";

        static DatasetData GetDefaultDatasetData()
        {
            return new DatasetData
            {
                Created = DateTime.UtcNow,
                CreatedBy = k_Author,
                DatasetId = new DatasetId(Guid.NewGuid()),

                Name = k_DefaultName,
                Description = k_DefaultDescription,
                Metadata = null,
                PortalMetadata = null,
                SystemMetadata = null,
                Tags = new List<string>(),

                SystemTags = new List<string>(),
                FileOrder = new List<string> { $"{k_DefaultName}_1", $"{k_DefaultName}_2" },
                Updated = DateTime.UtcNow,
                UpdatedBy = k_Author,
                Status = k_DefaultStatus,
            };
        }

        /// <inheritdoc />
        public async Task<IDatasetData> CreateDatasetAsync(AssetDescriptor assetDescriptor, IDatasetBaseData datasetCreation, CancellationToken token)
        {
            await Task.CompletedTask;

            var createdDataset = GetDefaultDatasetData();

            createdDataset.Name = datasetCreation.Name;
            createdDataset.Description = datasetCreation.Description;
            createdDataset.Metadata = datasetCreation.Metadata;
            createdDataset.PortalMetadata = datasetCreation.SystemMetadata;
            createdDataset.Tags = datasetCreation.Tags?.ToList() ?? new List<string>();

            return createdDataset;
        }

        /// <inheritdoc />
        public async Task<IDatasetData> GetDatasetAsync(DatasetDescriptor datasetDescriptor, FieldsFilter fieldsFilter, CancellationToken token)
        {
            await Task.CompletedTask;

            var dataset = GetDefaultDatasetData();
            dataset.DatasetId = datasetDescriptor.DatasetId;
            return dataset;
        }

        /// <inheritdoc />
        public async Task<IDatasetData> GetDatasetBySystemTagAsync(AssetDescriptor assetDescriptor, string systemTag, FieldsFilter fieldsFilter, CancellationToken token)
        {
            await Task.CompletedTask;

            var dataset = GetDefaultDatasetData();
            dataset.SystemTags = new[] { systemTag };
            return dataset;
        }

        /// <inheritdoc />
        public Task UpdateDatasetAsync(DatasetDescriptor datasetDescriptor, IDatasetUpdateData datasetUpdate, CancellationToken token)
        {
            var dataset = GetDefaultDatasetData();

            dataset.DatasetId = datasetDescriptor.DatasetId;
            dataset.Name = datasetUpdate.Name;
            dataset.Description = datasetUpdate.Description;
            dataset.Metadata = datasetUpdate.Metadata;
            dataset.PortalMetadata = datasetUpdate.PortalMetadata;
            dataset.SystemMetadata = datasetUpdate.SystemMetadata;
            dataset.Tags = datasetUpdate.Tags?.ToList() ?? new List<string>();
            dataset.FileOrder = new List<string>(datasetUpdate.FileOrder ?? Array.Empty<string>());

            return Task.FromResult<IDatasetData>(dataset);
        }

        /// <inheritdoc />
        public Task ReferenceFileFromDatasetAsync(DatasetDescriptor datasetDescriptor, DatasetId sourceDatasetId, string filePath, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        public Task RemoveFileFromDatasetAsync(DatasetDescriptor datasetDescriptor, string filePath, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task<bool> CheckDatasetIsInProjectAssetVersionAsync(DatasetDescriptor datasetDescriptor, CancellationToken token)
        {
            await Task.CompletedTask;
            var dataset = GetDefaultDatasetData();
            return dataset.DatasetId == datasetDescriptor.DatasetId;
        }

        /// <inheritdoc />
        public Task RemoveDatasetMetadataAsync(DatasetDescriptor datasetDescriptor, string metadataType, IEnumerable<string> keys, CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}
#endif
