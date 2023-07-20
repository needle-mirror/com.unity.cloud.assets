using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// An interface that provides all the methods to interact with an <see cref="IAsset"/>.
    /// </summary>
    public interface IAssetManager
    {
        /// <summary>
        /// Implement this method to get a single <see cref="IAsset"/>.
        /// </summary>
        /// <param name="organization">The organization the <paramref name="project"/> belongs to. </param>
        /// <param name="project">The id of the project the <paramref name="assetId"/> belongs to. </param>
        /// <param name="assetId">The id of the asset to retrieve. </param>
        /// <param name="assetVersion">The version number of the asset. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is the requested <see cref="IAsset"/>.</returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetManagerExample.cs" region="GetAsset"/>
        /// </example>
        Task<IAsset> GetAssetAsync(IOrganization organization, IProject project, string assetId, int assetVersion, CancellationToken token);

        /// <summary>
        /// Implement this method to get a single asset of type <typeparamref name="TAsset"/>.
        /// </summary>
        /// <param name="organization">The organization the <paramref name="project"/> belongs to. </param>
        /// <param name="project">The id of the project the <paramref name="assetId"/> belongs to. </param>
        /// <param name="assetId">The id of the asset to retrieve. </param>
        /// <param name="assetVersion">The version number of the asset. </param>
        /// <typeparam name="TAsset">A type which inherits from <see cref="IAsset"/>. </typeparam>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is the requested <see cref="IAsset"/>.</returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetManagerExample.cs" region="GetAssetSpecifiedType"/>
        /// </example>
        Task<TAsset> GetAssetAsync<TAsset>(IOrganization organization, IProject project, string assetId, int assetVersion, CancellationToken token)
            where TAsset : IAsset, new();

        /// <summary>
        /// Implement this method to get a collection of assets that satisfy the search criteria.
        /// </summary>
        /// <param name="assetSearchFilter">The object containing the parameters for the asset search. </param>
        /// <param name="pagination">An object containing the necessary information create an <see cref="IAssetPage"/>. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is an <see cref="IAssetPage"/> containing a collection of <see cref="Pagination.PageSize"/> <see cref="IAsset"/> results.</returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetManagerExample.cs" region="SearchForAssets"/>
        /// </example>
        Task<IAssetPage> SearchAsync(IAssetSearchFilter assetSearchFilter, Pagination pagination, CancellationToken token);

        /// <summary>
        /// Implement this method to get a collection of assets that satisfy the search criteria.
        /// </summary>
        /// <param name="assetSearchFilter">The object containing the parameters for the asset search. </param>
        /// <param name="pagination">An object containing the necessary information create an <see cref="IAssetPage"/>. </param>
        /// <param name="token">The cancellation token</param>
        /// <typeparam name="TAsset">A type which inherits from <see cref="IAsset"/>. </typeparam>
        /// <returns>A task whose result is an <see cref="IAssetPage"/> containing a collection of <see cref="Pagination.PageSize"/> <see cref="IAsset"/> results.</returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetManagerExample.cs" region="SearchForAssetSpecifiedType"/>
        /// </example>
        Task<IAssetPage> SearchAsync<TAsset>(IAssetSearchFilter assetSearchFilter, Pagination pagination, CancellationToken token)
            where TAsset : IAsset, new();

        /// <summary>
        /// Implement this method to get a count of assets that satisfy the search criteria.
        /// </summary>
        /// <param name="assetSearchFilter">The object containing the parameters for the asset search. </param>
        /// <param name="parameters">The object containing the necessary information for aggregation. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is a count of assets that satisfy the <paramref name="assetSearchFilter"/>. </returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetManagerExample.cs" region="AggregateAssets"/>
        /// </example>
        Task<Aggregation> AggregateAsync(IAssetSearchFilter assetSearchFilter, AggregationParameters parameters, CancellationToken token);

        /// <summary>
        /// Implement this method to create an asset.
        /// </summary>
        /// <param name="assetCreation">The object containing the necessary information to create an <see cref="IAsset"/>. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with no result. </returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetManagerExample.cs" region="CreateAsset"/>
        /// </example>
        Task<IAsset> CreateAssetAsync(IAssetCreation assetCreation, CancellationToken token);

        /// <summary>
        /// Implement this method to update an asset.
        /// </summary>
        /// <param name="asset">The asset you want to update. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with no result. </returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetManagerExample.cs" region="UpdateAsset"/>
        /// </example>
        Task UpdateAssetAsync(IAsset asset, CancellationToken token);

        /// <summary>
        /// Implement this method to delete an asset.
        /// </summary>
        /// <param name="asset">The asset to delete.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with no result. </returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetManagerExample.cs" region="DeleteAsset"/>
        /// </example>
        Task DeleteAssetAsync(IAsset asset, CancellationToken token);

        /// <summary>
        /// Implement this method to get the asset download urls.
        /// </summary>
        /// <param name="asset">The the asset.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with no result. </returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetManagerExample.cs" region="GetAssetDownloadUrls"/>
        /// </example>
        Task GetAssetDownloadUrlsAsync(IAsset asset, CancellationToken token);

        /// <summary>
        /// Implement this method to get the asset collections.
        /// </summary>
        /// <param name="asset">The the asset.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with no result. </returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetManagerExample.cs" region="GetAssetCollections"/>
        /// </example>
        Task GetAssetCollectionsAsync(IAsset asset, CancellationToken token);

        /// <summary>
        /// Implement this method to get the asset download urls.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <param name="destinationOrganizationId">The destination organization id.</param>
        /// <param name="destinationProjectId">The destination project id.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with no result. </returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetManagerExample.cs" region="LinkAnAssetToProject"/>
        /// </example>
        Task LinkAnAssetToProjectAsync(IAsset asset, ulong destinationOrganizationId, string destinationProjectId, CancellationToken token);

        /// <summary>
        /// Implement this method to unlink the asset from the project.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with no result. </returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetManagerExample.cs" region="UnlinkAssetFromProject"/>
        /// </example>
        Task UnlinkAssetFromProjectAsync(IAsset asset, CancellationToken token);

        /// <summary>
        /// Implement this method to check if the project is an asset source project.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with a boolean. True if asset's project is the source. False otherwise</returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetManagerExample.cs" region="CheckProjectIsAssetSourceProject"/>
        /// </example>
        Task<bool> CheckProjectIsAssetSourceProjectAsync(IAsset asset, CancellationToken token);

        /// <summary>
        /// Implement this method to publish an approved asset.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with no result. </returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetManagerExample.cs" region="PublishApprovedAsset"/>
        /// </example>
        Task PublishApprovedAssetAsync(IAsset asset, CancellationToken token);

        /// <summary>
        /// Implement this method to withdraw an published asset.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with no result. </returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetManagerExample.cs" region="WithdrawPublishedAsset"/>
        /// </example>
        Task WithdrawPublishedAssetAsync(IAsset asset, CancellationToken token);

        /// <summary>
        /// Implement this method to send an asset to review.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with no result. </returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetManagerExample.cs" region="SendAssetToReview"/>
        /// </example>
        Task SendAssetToReviewAsync(IAsset asset, CancellationToken token);

        /// <summary>
        /// Implement this method to approve an asset in review.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with no result. </returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetManagerExample.cs" region="ApproveAsset"/>
        /// </example>
        Task ApproveAssetAsync(IAsset asset, CancellationToken token);

        /// <summary>
        /// Implement this method to reject an asset in review.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with no result. </returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetManagerExample.cs" region="RejectAsset"/>
        /// </example>
        Task RejectAssetAsync(IAsset asset, CancellationToken token);
    }
}
