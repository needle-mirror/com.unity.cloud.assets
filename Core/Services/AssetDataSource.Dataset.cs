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
            var requestUri = m_PublicServiceHostResolver.GetResolvedRequestUri(request.ConstructUrl(GetPublicApiPath()));
            var response = await m_ServiceHttpClient.PostAsync(requestUri, request.ConstructBody(),
                ServiceHttpClientOptions.Default(), token);
            var jsonContent = await response.GetContentAsString();

            var createdDatasetResponse = IsolatedJsonConvert.DeserializeObject<CreatedDatasetDto>(jsonContent, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);

            var createdDatasetData = new DatasetData
            {
                DatasetId = new DatasetId(createdDatasetResponse.DatasetId),
                Name = datasetCreation.Name,
                Description = datasetCreation.Description,
                Tags = datasetCreation.Tags ?? new List<string>(),
                PortalMetadata = datasetCreation.PortalMetadata,
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
            return assetData.Datasets.FirstOrDefault(d => d.DatasetId == datasetDescriptor.DatasetId);
        }

        /// <inheritdoc />
        public async Task<IDatasetData> GetDatasetBySystemTagAsync(AssetDescriptor assetDescriptor, string systemTag, FieldsFilter includedFieldsFilter, CancellationToken token)
        {
            var assetData = await GetAssetAsync(assetDescriptor, includedFieldsFilter, token);
            return assetData.Datasets.FirstOrDefault(d => d.SystemTags != null && d.SystemTags.Contains(systemTag));
        }

        /// <inheritdoc />
        public Task UpdateDatasetAsync(DatasetDescriptor datasetDescriptor, IDatasetUpdateData datasetUpdate, CancellationToken token)
        {
            var request = new UpdateDatasetRequest(datasetDescriptor.ProjectId, datasetDescriptor.AssetId, datasetDescriptor.AssetVersion, datasetDescriptor.DatasetId, datasetUpdate);
            var requestUri = m_PublicServiceHostResolver.GetResolvedRequestUri(request.ConstructUrl(GetPublicApiPath()));
            return m_ServiceHttpClient.PatchAsync(requestUri, request.ConstructBody(),
                ServiceHttpClientOptions.Default(), token);
        }

        /// <inheritdoc />
        public Task ReferenceFileFromDatasetAsync(DatasetDescriptor datasetDescriptor, DatasetId sourceDatasetId, string filePath, CancellationToken token)
        {
            var request = new AddFileReferenceRequest(datasetDescriptor.ProjectId, datasetDescriptor.AssetId, datasetDescriptor.AssetVersion, sourceDatasetId, filePath, datasetDescriptor.DatasetId);
            var requestUri = m_PublicServiceHostResolver.GetResolvedRequestUri(request.ConstructUrl(GetPublicApiPath()));
            return m_ServiceHttpClient.PostAsync(requestUri, request.ConstructBody(),
                ServiceHttpClientOptions.Default(), token);
        }

        public Task RemoveFileFromDatasetAsync(DatasetDescriptor datasetDescriptor, string filePath, CancellationToken token)
        {
            var request = new FileRequest(datasetDescriptor.ProjectId, datasetDescriptor.AssetId, datasetDescriptor.AssetVersion, datasetDescriptor.DatasetId, filePath);
            var requestUri = m_PublicServiceHostResolver.GetResolvedRequestUri(request.ConstructUrl(GetPublicApiPath()));
            return m_ServiceHttpClient.DeleteAsync(requestUri, request.ConstructBody(),
                ServiceHttpClientOptions.Default(), token);
        }

        /// <inheritdoc />
        public async Task<bool> CheckDatasetIsInProjectAssetVersionAsync(DatasetDescriptor datasetDescriptor, CancellationToken token)
        {
            var request = new CheckDatasetBelongsToAssetRequest(datasetDescriptor.ProjectId, datasetDescriptor.AssetId, datasetDescriptor.AssetVersion, datasetDescriptor.DatasetId);
            var requestUri = m_PublicServiceHostResolver.GetResolvedRequestUri(request.ConstructUrl(GetPublicApiPath()));
            var response = await m_ServiceHttpClient.GetAsync(requestUri, ServiceHttpClientOptions.Default(),
                token);
            var jsonContent = await response.GetContentAsString();

            var dto = IsolatedJsonConvert.DeserializeObject<DatasetAssetCheckDto>(jsonContent, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);

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
            var requestUri = m_PublicServiceHostResolver.GetResolvedRequestUri(request.ConstructUrl(GetPublicApiPath()));
            return m_ServiceHttpClient.DeleteAsync(requestUri, request.ConstructBody(),
                ServiceHttpClientOptions.Default(), token);
        }
    }
}
