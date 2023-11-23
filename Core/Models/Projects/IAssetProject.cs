using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This class contains all the information about a cloud project.
    /// </summary>
    public interface IAssetProject
    {
        /// <summary>
        /// The descriptor of the project.
        /// </summary>
        ProjectDescriptor Descriptor { get; }

        /// <summary>
        /// The project name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The project metadata.
        /// </summary>
        IDeserializable Metadata { get; set; }

        /// <summary>
        /// Retrieves an asset by its ID and version.
        /// </summary>
        /// <param name="assetId">The id of the asset. </param>
        /// <param name="assetVersion">The version of the asset. </param>
        /// <param name="includedFieldsFilter">The filter describing which fields to return populated. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the requested asset. </returns>
        Task<IAsset> GetAssetAsync(AssetId assetId, AssetVersion assetVersion, FieldsFilter includedFieldsFilter, CancellationToken cancellationToken);

        /// <summary>
        /// Creates an asset.
        /// </summary>
        /// <param name="assetCreation">The object containing all the necessary information to create the asset. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the new asset. </returns>
        Task<IAsset> CreateAssetAsync(IAssetCreation assetCreation, CancellationToken cancellationToken);

        /// <summary>
        /// Searches the project for assets which match the search filter.
        /// </summary>
        /// <param name="assetSearchFilter">A search filter. </param>
        /// <param name="pagination">The range and ordering of assets to retrieve. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an async enumeration of <see cref="IAsset"/>. </returns>
        IAsyncEnumerable<IAsset> SearchAssetsAsync(IAssetSearchFilter assetSearchFilter, Pagination pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Counts the number of assets which match the search filter.
        /// </summary>
        /// <param name="assetSearchFilter">A search filter. </param>
        /// <param name="parameters">The parameters for the count. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the asset count. </returns>
        Task<Aggregation> CountAssetsAsync(IAssetSearchFilter assetSearchFilter, AggregationParameters parameters, CancellationToken cancellationToken);

        /// <summary>
        /// Returns the collections in the project.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an enumeration of <see cref="IAssetCollection"/>. </returns>
        Task<IEnumerable<IAssetCollection>> ListCollectionsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Returns the collection at the specified path.
        /// </summary>
        /// <param name="collectionPath">The path to the collection. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the requested collection. </returns>
        Task<IAssetCollection> GetCollectionAsync(CollectionPath collectionPath, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a collection.
        /// </summary>
        /// <param name="assetCollectionCreation">The object containing the necessary information to create a collection. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the newly created collection. </returns>
        Task<IAssetCollection> CreateCollectionAsync(IAssetCollectionCreation assetCollectionCreation, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a collection.
        /// </summary>
        /// <param name="collectionPath"></param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        Task DeleteCollectionAsync(CollectionPath collectionPath, CancellationToken cancellationToken);
    }
}
