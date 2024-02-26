using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Assets
{
    public static class ProjectExtensions
    {
        /// <summary>
        /// Returns the total count of assets in the specified projects based on the provided criteria.
        /// </summary>
        /// <param name="assetProject">The <see cref="IAssetProject"/>. </param>
        /// <param name="assetSearchFilter">The filter specifying the search criteria. Can be null. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an asset count. </returns>
        public static async Task<int> CountAssetsAsync(this IAssetProject assetProject, [AllowNull] IAssetSearchFilter assetSearchFilter, CancellationToken cancellationToken)
        {
            var result = await assetProject.GroupAndCountAssets().SelectWhereMatchesFilter(assetSearchFilter).ExecuteAsync(GroupableField.Type, cancellationToken);
            return result.Values.Sum();
        }

        /// <summary>
        /// Returns the collections of the project.
        /// </summary>
        /// <param name="assetProject">The <see cref="IAssetProject"/>. </param>
        /// <param name="range">The range of results to return. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns></returns>
        public static IAsyncEnumerable<IAssetCollection> ListCollectionsAsync(this IAssetProject assetProject, Range range, CancellationToken cancellationToken)
        {
            return assetProject.QueryCollections().LimitTo(range).ExecuteAsync(cancellationToken);
        }
    }
}
