using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Implement this interface to transform user facing data like <see cref="IAsset"/> and <see cref="IAssetPage"/> into service DTOs
    /// </summary>
    interface IAssetDataSource
    {
        /// <summary>
        /// Implement this method to retrieve an <see cref="IAsset"/> of type <typeparamref name="TAsset"/>.
        /// </summary>
        /// <param name="organization">The organization in which the <paramref name="project"/> resides. </param>
        /// <param name="project">The project in which the asset resides. It must exist within the <paramref name="organization"/>. </param>
        /// <param name="assetId">The id of the asset to retrieve. </param>
        /// <param name="assetVersion">The version number of the asset. </param>
        /// <param name="token">The cancellation token</param>
        /// <typeparam name="TAsset">A type which inherits from <see cref="IAsset"/>. </typeparam>
        /// <returns>A task whose result is a <typeparamref name="TAsset"/>. </returns>
        Task<TAsset> GetAssetAsync<TAsset>(IOrganization organization, IProject project, string assetId, int assetVersion, CancellationToken token)
            where TAsset : IAsset, new();

        /// <summary>
        /// Implement this method to retrieve a subset of <see cref="IAsset"/> of type <typeparamref name="TAsset"/>.
        /// </summary>
        /// <param name="organization">The organization in which the <paramref name="project"/> resides. </param>
        /// <param name="project">The id of the project in which to search. </param>
        /// <param name="assetSearchFilter">An object defining the search criteria for retrieving a set of assets. </param>
        /// <param name="pagination">An object containing the necessary information create an <see cref="IAssetPage"/>. </param>
        /// <param name="token"></param>
        /// <typeparam name="TAsset">A type which inherits from <see cref="IAsset"/>. </typeparam>
        /// <returns>A task whose result is an <see cref="IAssetPage"/>. </returns>
        Task<IAssetPage> GetAssetPageAsync<TAsset>(IOrganization organization, IProject project, IAssetSearchFilter assetSearchFilter, Pagination pagination, CancellationToken token)
            where TAsset : IAsset, new();

        /// <summary>
        /// Implement this method to retrieve the next subset of assets.
        /// </summary>
        /// <param name="assetPage">The previous subset of assets. </param>
        /// <param name="token">The cancellation token</param>
        /// <typeparam name="TAsset">A type which inherits from <see cref="IAsset"/>. </typeparam>
        /// <returns>A task whose result is an <see cref="IAssetPage"/>. </returns>
        Task<IAssetPage> GetNextAssetPageAsync<TAsset>(IAssetPage assetPage, CancellationToken token)
            where TAsset : IAsset, new();

        /// <summary>
        /// Implement this method to retrieve the aggregate of assets that meet the search criteria.
        /// </summary>
        /// <param name="organization">The organization in which the <paramref name="project"/> resides. </param>
        /// <param name="project">The id of the project in which to search. </param>
        /// <param name="assetSearchFilter">An object defining the search criteria. </param>
        /// <param name="parameters">An object containing the necessary information to </param>
        /// <param name="token">The cancellation token</param>
        /// <returns></returns>
        Task<Aggregation> GetAssetAggregateAsync(IOrganization organization, IProject project, IAssetSearchFilter assetSearchFilter, AggregationParameters parameters, CancellationToken token);

        /// <summary>
        /// Implement this method to create an asset.
        /// </summary>
        /// <param name="organization">The organization in which the <paramref name="project"/> resides. </param>
        /// <param name="project">The project in which the asset resides. It must exist within the <paramref name="organization"/>. </param>
        /// <param name="assetCreation">The object containing the necessary information to create an <see cref="IAsset"/>. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is an <see cref="IAsset"/>. </returns>
        Task<IAsset> CreateAssetAsync(IOrganization organization, IProject project, IAssetCreation assetCreation, CancellationToken token);

        /// <summary>
        /// Implement this method to update an asset.
        /// </summary>
        /// <param name="organization">The organization in which the <paramref name="project"/> resides. </param>
        /// <param name="project">The project in which the asset resides. It must exist within the <paramref name="organization"/>. </param>
        /// <param name="asset">The asset you want to update. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is the updated <see cref="IAsset"/>. </returns>
        Task<IAsset> UpdateAssetAsync(IOrganization organization, IProject project, IAsset asset, CancellationToken token);

        /// <summary>
        /// Implement this method to delete an asset.
        /// </summary>
        /// <param name="organization">The organization in which the <paramref name="project"/> resides. </param>
        /// <param name="project">The project in which the asset resides. It must exist within the <paramref name="organization"/>. </param>
        /// <param name="assetId">The id of the asset to delete.</param>
        /// <param name="assetVersion">The version of the asset to delete.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with no result. </returns>
        Task DeleteAssetAsync(IOrganization organization, IProject project, string assetId, int assetVersion, CancellationToken token);

        /// <summary>
        /// Implement this method to get the asset download urls.
        /// </summary>
        /// <param name="organization">The organization in which the <paramref name="project"/> resides. </param>
        /// <param name="project">The project in which the asset resides. It must exist within the <paramref name="organization"/>. </param>
        /// <param name="asset">The asset.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>The updated asset containing its download urls</returns>
        Task<IAsset> GetAssetDownloadUrlsAsync(IOrganization organization, IProject project, IAsset asset, CancellationToken token);

        /// <summary>
        /// Implement this method to get the asset collections.
        /// </summary>
        /// <param name="organization">The organization in which the <paramref name="project"/> resides. </param>
        /// <param name="project">The project in which the asset resides. It must exist within the <paramref name="organization"/>. </param>
        /// <param name="asset">The asset.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>The updated asset containing its collections</returns>
        Task<IAsset> GetAssetCollectionsAsync(IOrganization organization, IProject project, IAsset asset, CancellationToken token);

        /// <summary>
        /// Implement this method to get the asset download urls.
        /// </summary>
        /// <param name="organization">The organization in which the <paramref name="project"/> resides. </param>
        /// <param name="project">The project in which the asset resides. It must exist within the <paramref name="organization"/>. </param>
        /// <param name="assetId">The id of the asset.</param>
        /// <param name="assetVersion">The version of the asset.</param>
        /// <param name="destinationOrganizationId">The destination organization id.</param>
        /// <param name="destinationProjectId">The destination project id.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns></returns>
        Task LinkAnAssetToProjectAsync(IOrganization organization, IProject project, string assetId, int assetVersion, ulong destinationOrganizationId, string destinationProjectId, CancellationToken token);

        /// <summary>
        /// Implement this method to unlink the asset from the project.
        /// </summary>
        /// <param name="organization">The organization in which the <paramref name="project"/> resides. </param>
        /// <param name="project">The project in which the asset resides. It must exist within the <paramref name="organization"/>. </param>
        /// <param name="assetId">The id of the asset.</param>
        /// <param name="assetVersion">The version of the asset.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns></returns>
        Task UnlinkAssetFromProjectAsync(IOrganization organization, IProject project, string assetId, int assetVersion, CancellationToken token);

        /// <summary>
        /// Implement this method to check if the project is an asset source project.
        /// </summary>
        /// <param name="organization">The organization in which the <paramref name="project"/> resides. </param>
        /// <param name="project">The project in which the asset resides. It must exist within the <paramref name="organization"/>. </param>
        /// <param name="assetId">The id of the asset.</param>
        /// <param name="assetVersion">The version of the asset.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns></returns>
        Task<bool> CheckProjectIsAssetSourceProjectAsync(IOrganization organization, IProject project, string assetId, int assetVersion, CancellationToken token);

        /// <summary>
        /// Implement this method to publish an approved asset.
        /// </summary>
        /// <param name="organization">The organization in which the <paramref name="project"/> resides. </param>
        /// <param name="project">The project in which the asset resides. It must exist within the <paramref name="organization"/>. </param>
        /// <param name="assetId">The id of the asset to delete.</param>
        /// <param name="assetVersion">The version of the asset to delete.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns></returns>
        Task<string> PublishApprovedAssetAsync(IOrganization organization, IProject project, string assetId, int assetVersion, CancellationToken token);

        /// <summary>
        /// Implement this method to withdraw an published asset.
        /// </summary>
        /// <param name="organization">The organization in which the <paramref name="project"/> resides. </param>
        /// <param name="project">The project in which the asset resides. It must exist within the <paramref name="organization"/>. </param>
        /// <param name="assetId">The id of the asset to delete.</param>
        /// <param name="assetVersion">The version of the asset to delete.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns></returns>
        Task<string> WithdrawPublishedAssetAsync(IOrganization organization, IProject project, string assetId, int assetVersion, CancellationToken token);

        /// <summary>
        /// Implement this method to send an asset to review.
        /// </summary>
        /// <param name="organization">The organization in which the <paramref name="project"/> resides. </param>
        /// <param name="project">The project in which the asset resides. It must exist within the <paramref name="organization"/>. </param>
        /// <param name="assetId">The id of the asset to delete.</param>
        /// <param name="assetVersion">The version of the asset to delete.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns></returns>
        Task<string> SendAssetToReviewAsync(IOrganization organization, IProject project, string assetId, int assetVersion, CancellationToken token);

        /// <summary>
        /// Implement this method to approve an asset in review.
        /// </summary>
        /// <param name="organization">The organization in which the <paramref name="project"/> resides. </param>
        /// <param name="project">The project in which the asset resides. It must exist within the <paramref name="organization"/>. </param>
        /// <param name="assetId">The id of the asset to delete.</param>
        /// <param name="assetVersion">The version of the asset to delete.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns></returns>
        Task<string> ApproveAssetAsync(IOrganization organization, IProject project, string assetId, int assetVersion, CancellationToken token);

        /// <summary>
        /// Implement this method to reject an asset in review.
        /// </summary>
        /// <param name="organization">The organization in which the <paramref name="project"/> resides. </param>
        /// <param name="project">The project in which the asset resides. It must exist within the <paramref name="organization"/>. </param>
        /// <param name="assetId">The id of the asset to delete.</param>
        /// <param name="assetVersion">The version of the asset to delete.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns></returns>
        Task<string> RejectAssetAsync(IOrganization organization, IProject project, string assetId, int assetVersion, CancellationToken token);
    }
}
