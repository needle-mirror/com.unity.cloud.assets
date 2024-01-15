using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    partial class AssetDataSource
    {
        /// <inheritdoc />
        public async Task<IDatasetData> CreateDatasetAsync(AssetDescriptor assetDescriptor, IDatasetBaseData datasetCreation, CancellationToken token)
        {
            var request = new CreateDatasetRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, datasetCreation);
            var response = await m_ServiceHttpClient.PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), token);
            var jsonContent = await response.GetContentAsString();

            var createdDatasetResponse = IsolatedSerialization.DeserializeWithConverters<CreatedDatasetDto>(jsonContent, IsolatedSerialization.DatasetIdConverter);

            var createdDatasetData = new DatasetData
            {
                DatasetId = createdDatasetResponse.DatasetId,
                Name = datasetCreation.Name,
                Description = datasetCreation.Description,
                Tags = datasetCreation.Tags ?? new List<string>(),
                Metadata = datasetCreation.Metadata,
                SystemMetadata = datasetCreation.SystemMetadata,
                SystemTags = new List<string>(),
                FileOrder = new List<string>(),
            };

            return createdDatasetData;
        }

        /// <inheritdoc />
        public async Task<IDatasetData> GetDatasetAsync(DatasetDescriptor datasetDescriptor, FieldsFilter includedFieldsFilter, CancellationToken token)
        {
            var assetData = await GetAssetAsync(datasetDescriptor.AssetDescriptor, includedFieldsFilter, token);
            var dataset = assetData.Datasets.FirstOrDefault(d => d.DatasetId == datasetDescriptor.DatasetId);
            if (dataset == null)
            {
                throw new NotFoundException($"Dataset with id \"{datasetDescriptor.DatasetId}\" not found at that location.");
            }

            return dataset;
        }

        /// <inheritdoc />
        public async Task<IDatasetData> GetDatasetBySystemTagAsync(AssetDescriptor assetDescriptor, string systemTag, FieldsFilter includedFieldsFilter, CancellationToken token)
        {
            var assetData = await GetAssetAsync(assetDescriptor, includedFieldsFilter, token);
            var dataset = assetData.Datasets.FirstOrDefault(d => d.SystemTags != null && d.SystemTags.Contains(systemTag));
            if (dataset == null)
            {
                throw new NotFoundException($"Dataset with system tag \"{systemTag}\" not found at that location.");
            }

            return dataset;
        }

        /// <inheritdoc />
        public Task UpdateDatasetAsync(DatasetDescriptor datasetDescriptor, IDatasetUpdateData datasetUpdate, CancellationToken token)
        {
            var request = new DatasetRequest(datasetDescriptor.ProjectId, datasetDescriptor.AssetId, datasetDescriptor.AssetVersion, datasetDescriptor.DatasetId, datasetUpdate);
            return m_ServiceHttpClient.PatchAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), token);
        }

        /// <inheritdoc />
        public Task ReferenceFileFromDatasetAsync(DatasetDescriptor datasetDescriptor, DatasetId sourceDatasetId, string filePath, CancellationToken token)
        {
            var request = new AddFileReferenceRequest(datasetDescriptor.ProjectId, datasetDescriptor.AssetId, datasetDescriptor.AssetVersion, sourceDatasetId, filePath, datasetDescriptor.DatasetId);
            return m_ServiceHttpClient.PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), token);
        }

        public Task RemoveFileFromDatasetAsync(DatasetDescriptor datasetDescriptor, string filePath, CancellationToken token)
        {
            var request = new FileRequest(datasetDescriptor.ProjectId, datasetDescriptor.AssetId, datasetDescriptor.AssetVersion, datasetDescriptor.DatasetId, filePath);
            return m_ServiceHttpClient.DeleteAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), token);
        }

        /// <inheritdoc />
        public async Task<bool> CheckDatasetIsInProjectAssetVersionAsync(DatasetDescriptor datasetDescriptor, CancellationToken token)
        {
            var request = new CheckDatasetBelongsToAssetRequest(datasetDescriptor.ProjectId, datasetDescriptor.AssetId, datasetDescriptor.AssetVersion, datasetDescriptor.DatasetId);
            var response = await m_ServiceHttpClient.GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                token);
            var jsonContent = await response.GetContentAsString();

            var dto = JsonSerialization.Deserialize<DatasetAssetCheckDto>(jsonContent);

            return !string.IsNullOrEmpty(dto.DatasetVersionId);
        }

        /// <inheritdoc />
        public Task RemoveDatasetMetadataAsync(DatasetDescriptor datasetDescriptor, string metadataType, IEnumerable<string> keys, CancellationToken token)
        {
            var request = new RemoveMetadataRequest(datasetDescriptor.ProjectId,
                datasetDescriptor.AssetId,
                datasetDescriptor.AssetVersion,
                datasetDescriptor.DatasetId,
                metadataType,
                keys);
            return m_ServiceHttpClient.DeleteAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), token);
        }
    }
}
