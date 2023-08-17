using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <inheritdoc/>
    class AssetCollectionDataSource : IAssetCollectionDataSource
    {
        readonly IAssetHttpClient m_Client;

        /// <summary>
        /// Creates a new instance of the <see cref="AssetCollectionDataSource"/> class.
        /// </summary>
        /// <param name="serviceHttpClient"></param>
        /// <param name="serviceAddress"></param>
        internal AssetCollectionDataSource(IServiceHttpClient serviceHttpClient, string serviceAddress)
            : this(new AssetHttpClient(serviceHttpClient, serviceAddress)) { }

        /// <summary>
        /// Creates a new instance of the <see cref="AssetCollectionDataSource"/> class.
        /// </summary>
        /// <param name="client"></param>
        internal AssetCollectionDataSource(IAssetHttpClient client)
        {
            m_Client = client;
        }

        /// <inheritdoc/>
        public async Task<IAssetCollection[]> ListCollectionsAsync(IProject project, CancellationToken token)
        {
            var response = await m_Client.GetAsync(new GetCollectionListRequest(project.Organization.GenesisId, project.Id), ServiceHttpClientOptions.Default(), token);

            var collectionListDto = IsolatedJsonConvert.DeserializeObject<AssetCollectionListDto>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);

            if (collectionListDto.Collections == null || collectionListDto.Collections.Length == 0)
            {
                return Array.Empty<IAssetCollection>();
            }

            return collectionListDto.Collections.Select(x => InitializeAssetCollection(x, project)).ToArray();
        }

        /// <inheritdoc/>
        public async Task<IAssetCollection> GetCollectionAsync(IProject project, CollectionPath collectionPath, CancellationToken token)
        {
            var request = new CollectionRequest(project.Organization.GenesisId, project.Id, collectionPath);
            var response = await m_Client.GetAsync(request, ServiceHttpClientOptions.Default(), token);

            var assetCollection = IsolatedJsonConvert.DeserializeObject<AssetCollection>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);
            return InitializeAssetCollection(assetCollection, project);
        }

        /// <inheritdoc/>
        public async Task<CollectionPath> CreateCollectionAsync(IProject project, IAssetCollection assetCollection, CancellationToken token)
        {
            var request = new CreateCollectionRequest(project.Organization.GenesisId, project.Id, assetCollection);
            var response = await m_Client.PostAsync(request, ServiceHttpClientOptions.NoRetryOption(), token);

            var pathDto = IsolatedJsonConvert.DeserializeObject<AssetCollectionPathDto>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);
            return new CollectionPath(pathDto.Path);
        }

        /// <inheritdoc/>
        public async Task UpdateCollectionAsync(IProject project, IAssetCollection assetCollection, CancellationToken token)
        {
            var collectionPath = CollectionPath.CombinePaths(assetCollection.ParentPath, assetCollection.Name);
            var request = new UpdateCollectionRequest(project.Organization.GenesisId, project.Id, collectionPath, assetCollection);
            _ = await m_Client.PutAsync(request, ServiceHttpClientOptions.NoRetryOption(), token);
        }

        /// <inheritdoc/>
        public async Task DeleteCollectionAsync(IProject project, CollectionPath collectionPath, CancellationToken token)
        {
            var request = new CollectionRequest(project.Organization.GenesisId, project.Id, collectionPath);
            _ = await m_Client.DeleteAsync(request, ServiceHttpClientOptions.Default(), token);
        }

        /// <inheritdoc />
        public async Task<string> InsertAssetsToCollectionAsync(IProject project, CollectionPath collectionPath, IEnumerable<IAsset> assets, CancellationToken token)
        {
            var assetsInCollectionDto = new AssetsInCollectionDto
            {
                Assets = assets.Select(a => a.AssetToCollectionElementMapFrom()).ToArray()
            };

            var request = new InsertAssetsInCollectionRequest(project.Organization.GenesisId, project.Id, collectionPath, assetsInCollectionDto);
            var response = await m_Client.PostAsync(request, ServiceHttpClientOptions.NoRetryOption(), token);

            return response;
        }

        /// <inheritdoc />
        public async Task<string> RemoveAssetsFromCollectionAsync(IProject project, CollectionPath collectionPath, IEnumerable<IAsset> assets, CancellationToken token)
        {
            var assetsInCollectionDto = new AssetsInCollectionDto
            {
                Assets = assets.Select(a => a.AssetToCollectionElementMapFrom()).ToArray()
            };

            var request = new RemoveAssetsFromCollectionRequest(project.Organization.GenesisId, project.Id, collectionPath, assetsInCollectionDto);
            var response = await m_Client.PatchAsync(request, ServiceHttpClientOptions.Default(), token);

            return response;
        }

        /// <inheritdoc />
        public async Task<string> MoveCollectionToNewPathAsync(IProject project, CollectionPath collectionPath, CollectionPath newCollectionPath, CancellationToken token)
        {
            var request = new MoveCollectionToNewPathRequest(project.Organization.GenesisId, project.Id, collectionPath, newCollectionPath);
            var response = await m_Client.PatchAsync(request, ServiceHttpClientOptions.Default(), token);

            var pathDto = IsolatedJsonConvert.DeserializeObject<AssetCollectionPathDto>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);
            return pathDto.Path;
        }

        static IAssetCollection InitializeAssetCollection(AssetCollection assetCollection, IProject project)
        {
            assetCollection.Project = project;
            return assetCollection;
        }
    }
}
