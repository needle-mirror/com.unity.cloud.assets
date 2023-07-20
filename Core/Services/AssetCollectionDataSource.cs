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
        public async Task<IAssetCollection[]> ListCollectionsAsync(IOrganization organization, IProject project, CancellationToken token)
        {
            var response = await m_Client.GetAsync(new GetCollectionListRequest(organization.GenesisId, project.Id), ServiceHttpClientOptions.Default(), token);

            var collectionListDto = IsolatedJsonConvert.DeserializeObject<AssetCollectionListDto>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);

            if (collectionListDto.Collections == null || collectionListDto.Collections.Length == 0)
            {
                return Array.Empty<IAssetCollection>();
            }

            return collectionListDto.Collections.Select(x => InitializeAssetCollection(x, organization, project)).ToArray();
        }

        /// <inheritdoc/>
        public async Task<IAssetCollection> GetCollectionAsync(IOrganization organization, IProject project, CollectionPath collectionPath, CancellationToken token)
        {
            var request = new CollectionRequest(organization.GenesisId, project.Id, collectionPath);
            var response = await m_Client.GetAsync(request, ServiceHttpClientOptions.Default(), token);

            var assetCollection = IsolatedJsonConvert.DeserializeObject<AssetCollection>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);
            return InitializeAssetCollection(assetCollection, organization, project);
        }

        /// <inheritdoc/>
        public async Task<CollectionPath> CreateCollectionAsync(IOrganization organization, IProject project, IAssetCollection assetCollection, CancellationToken token)
        {
            var request = new CreateCollectionRequest(organization.GenesisId, project.Id, assetCollection);
            var response = await m_Client.PostAsync(request, ServiceHttpClientOptions.NoRetryOption(), token);

            var pathDto = IsolatedJsonConvert.DeserializeObject<AssetCollectionPathDto>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);
            return new CollectionPath(pathDto.Path);
        }

        /// <inheritdoc/>
        public async Task UpdateCollectionAsync(IOrganization organization, IProject project, IAssetCollection assetCollection, CancellationToken token)
        {
            var collectionPath = CollectionPath.CombinePaths(assetCollection.ParentPath, assetCollection.Name);
            var request = new UpdateCollectionRequest(organization.GenesisId, project.Id, collectionPath, assetCollection);
            _ = await m_Client.PutAsync(request, ServiceHttpClientOptions.NoRetryOption(), token);
        }

        /// <inheritdoc/>
        public async Task DeleteCollectionAsync(IOrganization organization, IProject project, CollectionPath collectionPath, CancellationToken token)
        {
            var request = new CollectionRequest(organization.GenesisId, project.Id, collectionPath);
            _ = await m_Client.DeleteAsync(request, ServiceHttpClientOptions.Default(), token);
        }

        /// <inheritdoc />
        public async Task<string> InsertAssetsToCollectionAsync(IOrganization organization, IProject project, CollectionPath collectionPath, IEnumerable<IAsset> assets, CancellationToken token)
        {
            var assetsInCollectionDto = new AssetsInCollectionDto
            {
                Assets = assets.Select(a => a.AssetToCollectionElementMapFrom()).ToArray()
            };

            var request = new InsertAssetsInCollectionRequest(organization.GenesisId, project.Id, collectionPath, assetsInCollectionDto);
            var response = await m_Client.PostAsync(request, ServiceHttpClientOptions.NoRetryOption(), token);

            return response;
        }

        /// <inheritdoc />
        public async Task<string> RemoveAssetsFromCollectionAsync(IOrganization organization, IProject project, CollectionPath collectionPath, IEnumerable<IAsset> assets, CancellationToken token)
        {
            var assetsInCollectionDto = new AssetsInCollectionDto
            {
                Assets = assets.Select(a => a.AssetToCollectionElementMapFrom()).ToArray()
            };

            var request = new RemoveAssetsFromCollectionRequest(organization.GenesisId, project.Id, collectionPath, assetsInCollectionDto);
            var response = await m_Client.PatchAsync(request, ServiceHttpClientOptions.Default(), token);

            return response;
        }

        /// <inheritdoc />
        public async Task<string> MoveCollectionToNewPathAsync(IOrganization organization, IProject project, CollectionPath collectionPath, CollectionPath newCollectionPath, CancellationToken token)
        {
            var request = new MoveCollectionToNewPathRequest(organization.GenesisId, project.Id, collectionPath, newCollectionPath);
            var response = await m_Client.PatchAsync(request, ServiceHttpClientOptions.Default(), token);

            var pathDto = IsolatedJsonConvert.DeserializeObject<AssetCollectionPathDto>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);
            return pathDto.Path;
        }

        static IAssetCollection InitializeAssetCollection(AssetCollection assetCollection, IOrganization organization, IProject project)
        {
            assetCollection.Organization = organization;
            assetCollection.Project = project;
            return assetCollection;
        }
    }
}
