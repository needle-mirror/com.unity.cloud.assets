using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// An interface that provides all the methods to fetch an <see cref="IAsset"/>.
    /// </summary>
    public interface IAssetProvider
    {
        /// <summary>
        /// The name of the provider.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Implement this method to get a single <see cref="IAsset"/>.
        /// </summary>
        /// <param name="project">The id of the project the <paramref name="assetId"/> belongs to. </param>
        /// <param name="assetId">The id of the asset to retrieve. </param>
        /// <param name="assetVersion">The version number of the asset. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is the requested <see cref="IAsset"/>.</returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetProviderExample.cs" region="GetAsset"/>
        /// </example>
        Task<IAsset> GetAssetAsync(IProject project, string assetId, int assetVersion, CancellationToken token);

        /// <summary>
        /// Implement this method to get a single asset of type <typeparamref name="TAsset"/>.
        /// </summary>
        /// <param name="project">The id of the project the <paramref name="assetId"/> belongs to. </param>
        /// <param name="assetId">The id of the asset to retrieve. </param>
        /// <param name="assetVersion">The version number of the asset. </param>
        /// <typeparam name="TAsset">A type which inherits from <see cref="IAsset"/>. </typeparam>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is the requested <see cref="IAsset"/>.</returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetProviderExample.cs" region="GetAssetSpecifiedType"/>
        /// </example>
        Task<TAsset> GetAssetAsync<TAsset>(IProject project, string assetId, int assetVersion, CancellationToken token)
            where TAsset : IAsset, new();

        /// <summary>
        /// Implement this method to get a collection of assets that satisfy the search criteria.
        /// </summary>
        /// <param name="assetSearchFilter">The object containing the parameters for the asset search. </param>
        /// <param name="pagination">An object containing the necessary information return a range of <see cref="IAsset"/> matching the search criteria. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is an async enumeration of <see cref="IAsset"/>.</returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetProviderExample.cs" region="SearchForAssets"/>
        /// </example>
        IAsyncEnumerable<IAsset> SearchAsync(IAssetSearchFilter assetSearchFilter, Pagination pagination, CancellationToken token);

        /// <summary>
        /// Implement this method to get a collection of assets across specified projects that satisfy the search criteria.
        /// </summary>
        /// <param name="organization">The organization in which the projects are. </param>
        /// <param name="projects">The id of the projects in which to search. </param>
        /// <param name="assetSearchFilter">The object containing the parameters for the asset search. </param>
        /// <param name="pagination">An object containing the necessary information return a range of <see cref="IAsset"/> matching the search criteria. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is an async enumeration of <see cref="IAsset"/>.</returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetProviderExample.cs" region="SearchForAssets"/>
        /// </example>
        IAsyncEnumerable<IAsset> SearchAsync(IOrganization organization, IEnumerable<IProject> projects, IAssetSearchFilter assetSearchFilter, Pagination pagination, CancellationToken token);

        /// <summary>
        /// Implement this method to get a collection of assets that satisfy the search criteria.
        /// </summary>
        /// <param name="assetSearchFilter">The object containing the parameters for the asset search. </param>
        /// <param name="pagination">An object containing the necessary information return a range of <see cref="IAsset"/> matching the search criteria. </param>
        /// <param name="token">The cancellation token</param>
        /// <typeparam name="TAsset">A type which inherits from <see cref="IAsset"/>. </typeparam>
        /// <returns>A task whose result is an async enumeration of <see cref="IAsset"/>.</returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetProviderExample.cs" region="SearchForAssetSpecifiedType"/>
        /// </example>
        IAsyncEnumerable<TAsset> SearchAsync<TAsset>(IAssetSearchFilter assetSearchFilter, Pagination pagination, CancellationToken token)
            where TAsset : IAsset, new();

        /// <summary>
        /// Implement this method to get a collection of assets across specified projects that satisfy the search criteria.
        /// </summary>
        /// <param name="organization">The organization in which the projects are. </param>
        /// <param name="projects">The id of the projects in which to search. </param>
        /// <param name="assetSearchFilter">The object containing the parameters for the asset search. </param>
        /// <param name="pagination">An object containing the necessary information return a range of <see cref="IAsset"/> matching the search criteria. </param>
        /// <param name="token">The cancellation token</param>
        /// <typeparam name="TAsset">A type which inherits from <see cref="IAsset"/>. </typeparam>
        /// <returns>A task whose result is an async enumeration of <see cref="IAsset"/>.</returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetProviderExample.cs" region="SearchForAssetSpecifiedType"/>
        /// </example>
        IAsyncEnumerable<TAsset> SearchAsync<TAsset>(IOrganization organization, IEnumerable<IProject> projects, IAssetSearchFilter assetSearchFilter, Pagination pagination, CancellationToken token)
            where TAsset : IAsset, new();

        /// <summary>
        /// Implement this method to get a count of assets that satisfy the search criteria.
        /// </summary>
        /// <param name="assetSearchFilter">The object containing the parameters for the asset search. </param>
        /// <param name="parameters">The object containing the necessary information for aggregation. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is a count of assets that satisfy the <paramref name="assetSearchFilter"/>. </returns>
        Task<Aggregation> AggregateAsync(IAssetSearchFilter assetSearchFilter, AggregationParameters parameters, CancellationToken token);

        /// <summary>
        /// Implement this method to get a count of assets across specified projects that satisfy the search criteria.
        /// </summary>
        /// <param name="organization">The organization in which the projects are. </param>
        /// <param name="projects">The id of the projects in which to search. </param>
        /// <param name="assetSearchFilter">The object containing the parameters for the asset search. </param>
        /// <param name="parameters">The object containing the necessary information for aggregation. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is a count of assets that satisfy the <paramref name="assetSearchFilter"/>. </returns>
        Task<Aggregation> AggregateAsync(IOrganization organization, IEnumerable<IProject> projects, IAssetSearchFilter assetSearchFilter, AggregationParameters parameters, CancellationToken token);
    }
}
