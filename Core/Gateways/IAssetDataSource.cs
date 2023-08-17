using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Implement this interface to transform user facing data like <see cref="IAsset"/> into service DTOs
    /// </summary>
    interface IAssetDataSource
    {
        /// <summary>
        /// Implement this method to retrieve an <see cref="IAsset"/> of type <typeparamref name="TAsset"/>.
        /// </summary>
        /// <param name="project">The project in which the asset resides. </param>
        /// <param name="assetId">The id of the asset to retrieve. </param>
        /// <param name="assetVersion">The version number of the asset. </param>
        /// <param name="token">The cancellation token</param>
        /// <typeparam name="TAsset">A type which inherits from <see cref="IAsset"/>. </typeparam>
        /// <returns>A task whose result is a <typeparamref name="TAsset"/>. </returns>
        Task<TAsset> GetAssetAsync<TAsset>(IProject project, string assetId, int assetVersion, CancellationToken token)
            where TAsset : IAsset, new();

        /// <summary>
        /// Implement this method to retrieve a subset of <see cref="IAsset"/> of type <typeparamref name="TAsset"/>.
        /// </summary>
        /// <param name="project">The id of the project in which to search. </param>
        /// <param name="assetSearchFilter">An object defining the search criteria for retrieving a set of assets. </param>
        /// <param name="pagination">An object containing the necessary information create retrieve a subset of <see cref="IAsset"/>. </param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TAsset">A type which inherits from <see cref="IAsset"/>. </typeparam>
        /// <returns>A task whose result is a token for the next page and a collection of <see cref="IAsset"/>. </returns>
        IAsyncEnumerable<TAsset> ListAssetsAsync<TAsset>(IProject project, IAssetSearchFilter assetSearchFilter, Pagination pagination, CancellationToken cancellationToken)
            where TAsset : IAsset, new();

        /// <summary>
        /// Implement this method to retrieve a subset of <see cref="IAsset"/> of type <typeparamref name="TAsset"/> across specified projects.
        /// </summary>
        /// <param name="organization">The organization in which the projects are. </param>
        /// <param name="projects">The id of the projects in which to search. </param>
        /// <param name="assetSearchFilter">An object defining the search criteria for retrieving a set of assets. </param>
        /// <param name="pagination">An object containing the necessary information create retrieve a subset of <see cref="IAsset"/>. </param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TAsset">A type which inherits from <see cref="IAsset"/>. </typeparam>
        /// <returns>A task whose result is a token for the next page and a collection of <see cref="IAsset"/>. </returns>
        IAsyncEnumerable<TAsset> ListAssetsAsync<TAsset>(IOrganization organization, IEnumerable<IProject> projects, IAssetSearchFilter assetSearchFilter, Pagination pagination, CancellationToken cancellationToken)
            where TAsset : IAsset, new();

        /// <summary>
        /// Implement this method to retrieve the aggregate of assets that meet the search criteria.
        /// </summary>
        /// <param name="project">The id of the project in which to search. </param>
        /// <param name="assetSearchFilter">An object defining the search criteria. </param>
        /// <param name="parameters">An object containing the necessary information to </param>
        /// <param name="token">The cancellation token</param>
        /// <returns></returns>
        Task<Aggregation> GetAssetAggregateAsync(IProject project, IAssetSearchFilter assetSearchFilter, AggregationParameters parameters, CancellationToken token);

        /// <summary>
        /// Implement this method to retrieve the aggregate of assets across specified projects that meet the search criteria.
        /// </summary>
        /// <param name="organization">The organization in which the projects are. </param>
        /// <param name="projects">The id of the projects in which to search. </param>
        /// <param name="assetSearchFilter">An object defining the search criteria. </param>
        /// <param name="parameters">An object containing the necessary information to </param>
        /// <param name="token">The cancellation token</param>
        /// <returns></returns>
        Task<Aggregation> GetAssetAggregateAsync(IOrganization organization, IEnumerable<IProject> projects, IAssetSearchFilter assetSearchFilter, AggregationParameters parameters, CancellationToken token);

        /// <summary>
        /// Implement this method to create an asset.
        /// </summary>
        /// <param name="project">The project in which the asset resides. </param>
        /// <param name="assetCreation">The object containing the necessary information to create an <see cref="IAsset"/>. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is an <see cref="IAsset"/>. </returns>
        Task<IAsset> CreateAssetAsync(IProject project, IAssetCreation assetCreation, CancellationToken token);

        /// <summary>
        /// Implement this method to update an asset.
        /// </summary>
        /// <param name="project">The project in which the asset resides. </param>
        /// <param name="asset">The asset you want to update. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is the updated <see cref="IAsset"/>. </returns>
        Task<IAsset> UpdateAssetAsync(IProject project, IAsset asset, CancellationToken token);

        /// <summary>
        /// Implement this method to delete an asset.
        /// </summary>
        /// <param name="project">The project in which the asset resides. </param>
        /// <param name="assetId">The id of the asset to delete.</param>
        /// <param name="assetVersion">The version of the asset to delete.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with no result. </returns>
        Task DeleteAssetAsync(IProject project, string assetId, int assetVersion, CancellationToken token);

        /// <summary>
        /// Implement this method to get the asset download urls.
        /// </summary>
        /// <param name="project">The project in which the asset resides. </param>
        /// <param name="asset">The asset.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>The updated asset containing its download urls</returns>
        Task<IAsset> GetAssetDownloadUrlsAsync(IProject project, IAsset asset, CancellationToken token);

        /// <summary>
        /// Implement this method to get the asset collections.
        /// </summary>
        /// <param name="project">The project in which the asset resides. </param>
        /// <param name="asset">The asset.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>The updated asset containing its collections</returns>
        Task<IAsset> GetAssetCollectionsAsync(IProject project, IAsset asset, CancellationToken token);

        /// <summary>
        /// Implement this method to get the asset download urls.
        /// </summary>
        /// <param name="project">The project in which the asset resides. </param>
        /// <param name="assetId">The id of the asset.</param>
        /// <param name="assetVersion">The version of the asset.</param>
        /// <param name="destinationOrganizationId">The destination organization id.</param>
        /// <param name="destinationProjectId">The destination project id.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns></returns>
        Task LinkAnAssetToProjectAsync(IProject project, string assetId, int assetVersion, ulong destinationOrganizationId, string destinationProjectId, CancellationToken token);

        /// <summary>
        /// Implement this method to unlink the asset from the project.
        /// </summary>
        /// <param name="project">The project in which the asset resides. </param>
        /// <param name="assetId">The id of the asset.</param>
        /// <param name="assetVersion">The version of the asset.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns></returns>
        Task UnlinkAssetFromProjectAsync(IProject project, string assetId, int assetVersion, CancellationToken token);

        /// <summary>
        /// Implement this method to check if the project is an asset source project.
        /// </summary>
        /// <param name="project">The project in which the asset resides. </param>
        /// <param name="assetId">The id of the asset.</param>
        /// <param name="assetVersion">The version of the asset.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns></returns>
        Task<bool> CheckProjectIsAssetSourceProjectAsync(IProject project, string assetId, int assetVersion, CancellationToken token);

        /// <summary>
        /// Implement this method to publish an approved asset.
        /// </summary>
        /// <param name="project">The project in which the asset resides. </param>
        /// <param name="assetId">The id of the asset to delete.</param>
        /// <param name="assetVersion">The version of the asset to delete.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns></returns>
        Task<string> PublishApprovedAssetAsync(IProject project, string assetId, int assetVersion, CancellationToken token);

        /// <summary>
        /// Implement this method to withdraw an published asset.
        /// </summary>
        /// <param name="project">The project in which the asset resides. </param>
        /// <param name="assetId">The id of the asset to delete.</param>
        /// <param name="assetVersion">The version of the asset to delete.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns></returns>
        Task<string> WithdrawPublishedAssetAsync(IProject project, string assetId, int assetVersion, CancellationToken token);

        /// <summary>
        /// Implement this method to send an asset to review.
        /// </summary>
        /// <param name="project">The project in which the asset resides. </param>
        /// <param name="assetId">The id of the asset to delete.</param>
        /// <param name="assetVersion">The version of the asset to delete.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns></returns>
        Task<string> SendAssetToReviewAsync(IProject project, string assetId, int assetVersion, CancellationToken token);

        /// <summary>
        /// Implement this method to approve an asset in review.
        /// </summary>
        /// <param name="project">The project in which the asset resides. </param>
        /// <param name="assetId">The id of the asset to delete.</param>
        /// <param name="assetVersion">The version of the asset to delete.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns></returns>
        Task<string> ApproveAssetAsync(IProject project, string assetId, int assetVersion, CancellationToken token);

        /// <summary>
        /// Implement this method to reject an asset in review.
        /// </summary>
        /// <param name="project">The project in which the asset resides. </param>
        /// <param name="assetId">The id of the asset to delete.</param>
        /// <param name="assetVersion">The version of the asset to delete.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns></returns>
        Task<string> RejectAssetAsync(IProject project, string assetId, int assetVersion, CancellationToken token);
    }
}
