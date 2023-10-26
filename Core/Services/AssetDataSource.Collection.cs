using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    partial class AssetDataSource
    {
        /// <inheritdoc />
        public async Task<IEnumerable<IAssetCollectionData>> GetAssetCollectionsAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            var request = new GetAssetCollectionsRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId);
            var requestUri = m_PublicServiceHostResolver.GetResolvedRequestUri(request.ConstructUrl(GetPublicApiPath()));
            var response = await m_ServiceHttpClient.GetAsync(requestUri, ServiceHttpClientOptions.Default(),
                cancellationToken);
            var jsonContent = await response.GetContentAsString();

            var assetCollectionDtos = IsolatedJsonConvert.DeserializeObject<AssetCollectionData[]>(jsonContent, s_AssetConverters);

            return assetCollectionDtos ?? Array.Empty<AssetCollectionData>();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IAssetCollectionData>> ListCollectionsAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken)
        {
            var request = new GetCollectionListRequest(projectDescriptor.ProjectId);
            var requestUri = m_PublicServiceHostResolver.GetResolvedRequestUri(request.ConstructUrl(GetPublicApiPath()));
            var response = await m_ServiceHttpClient.GetAsync(requestUri, ServiceHttpClientOptions.Default(),
                cancellationToken);
            var jsonContent = await response.GetContentAsString();

            var collectionListDto = IsolatedJsonConvert.DeserializeObject<AssetCollectionData[]>(jsonContent, s_AssetConverters);

            return collectionListDto ?? Array.Empty<AssetCollectionData>();
        }

        /// <inheritdoc/>
        public async Task<IAssetCollectionData> GetCollectionAsync(CollectionDescriptor collectionDescriptor, CancellationToken cancellationToken)
        {
            var request = new CollectionRequest(collectionDescriptor.ProjectId, collectionDescriptor.CollectionPath);
            var requestUri = m_PublicServiceHostResolver.GetResolvedRequestUri(request.ConstructUrl(GetPublicApiPath()));
            var response = await m_ServiceHttpClient.GetAsync(requestUri, ServiceHttpClientOptions.Default(),
                cancellationToken);
            var jsonContent = await response.GetContentAsString();

            var assetCollection = IsolatedJsonConvert.DeserializeObject<AssetCollectionData>(jsonContent, s_AssetConverters);

            return assetCollection;
        }

        /// <inheritdoc/>
        public async Task<CollectionPath> CreateCollectionAsync(ProjectDescriptor projectDescriptor, IAssetCollectionData assetCollection, CancellationToken cancellationToken)
        {
            var request = new CreateCollectionRequest(projectDescriptor.ProjectId, assetCollection);
            var requestUri = m_PublicServiceHostResolver.GetResolvedRequestUri(request.ConstructUrl(GetPublicApiPath()));
            var response = await m_ServiceHttpClient.PostAsync(requestUri, request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
            var jsonContent = await response.GetContentAsString();

            var pathDto = IsolatedJsonConvert.DeserializeObject<AssetCollectionPathDto>(jsonContent, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);

            return new CollectionPath(pathDto.Path);
        }

        /// <inheritdoc/>
        public Task UpdateCollectionAsync(CollectionDescriptor collectionDescriptor, IAssetCollectionData assetCollection, CancellationToken cancellationToken)
        {
            var request = new UpdateCollectionRequest(collectionDescriptor.ProjectId, collectionDescriptor.CollectionPath, assetCollection);
            var requestUri = m_PublicServiceHostResolver.GetResolvedRequestUri(request.ConstructUrl(GetPublicApiPath()));
            return m_ServiceHttpClient.PutAsync(requestUri, request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc/>
        public Task DeleteCollectionAsync(CollectionDescriptor collectionDescriptor, CancellationToken cancellationToken)
        {
            var request = new CollectionRequest(collectionDescriptor.ProjectId, collectionDescriptor.CollectionPath);
            var requestUri = m_PublicServiceHostResolver.GetResolvedRequestUri(request.ConstructUrl(GetPublicApiPath()));
            return m_ServiceHttpClient.DeleteAsync(requestUri, request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public Task AddAssetsToCollectionAsync(CollectionDescriptor collectionDescriptor, IEnumerable<AssetId> assets, CancellationToken cancellationToken)
        {
            var request = new ModifyAssetsInCollectionRequest(collectionDescriptor.ProjectId, collectionDescriptor.CollectionPath, assets);
            var requestUri = m_PublicServiceHostResolver.GetResolvedRequestUri(request.ConstructUrl(GetPublicApiPath()));
            return m_ServiceHttpClient.PostAsync(requestUri, request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public Task RemoveAssetsFromCollectionAsync(CollectionDescriptor collectionDescriptor, IEnumerable<AssetId> assets, CancellationToken cancellationToken)
        {
            var request = new ModifyAssetsInCollectionRequest(collectionDescriptor.ProjectId, collectionDescriptor.CollectionPath, assets);
            var requestUri = m_PublicServiceHostResolver.GetResolvedRequestUri(request.ConstructUrl(GetPublicApiPath()));
            return m_ServiceHttpClient.PatchAsync(requestUri, request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public async Task<CollectionPath> MoveCollectionToNewPathAsync(CollectionDescriptor collectionDescriptor, CollectionPath newCollectionPath, CancellationToken cancellationToken)
        {
            var request = new MoveCollectionToNewPathRequest(collectionDescriptor.ProjectId, collectionDescriptor.CollectionPath, newCollectionPath);
            var requestUri = m_PublicServiceHostResolver.GetResolvedRequestUri(request.ConstructUrl(GetPublicApiPath()));
            var response = await m_ServiceHttpClient.PatchAsync(requestUri, request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
            var jsonContent = await response.GetContentAsString();

            var pathDto = IsolatedJsonConvert.DeserializeObject<AssetCollectionPathDto>(jsonContent, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);

            return new CollectionPath(pathDto.Path);
        }
    }
}
