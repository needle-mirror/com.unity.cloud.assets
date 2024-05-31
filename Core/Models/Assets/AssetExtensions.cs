using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Assets
{
    public static class AssetExtensions
    {
        /// <summary>
        /// Returns the latest version of the asset.
        /// </summary>
        /// <param name="asset">The asset to query. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an <see cref="IAsset"/>. </returns>
        public static async Task<IAsset> WithLatestVersionAsync(this IAsset asset, CancellationToken cancellationToken)
        {
            var query = asset.QueryAssetVersions()
                .OrderBy("versionNumber", SortingOrder.Descending)
                .LimitTo(new Range(0, 1))
                .ExecuteAsync(cancellationToken);

            IAsset assetVersion = null;

            var enumerator = query.GetAsyncEnumerator(cancellationToken);
            if (await enumerator.MoveNextAsync())
            {
                assetVersion = enumerator.Current;
            }
            await enumerator.DisposeAsync();

            return assetVersion;
        }

        /// <summary>
        /// Returns the Preview dataset for the asset.
        /// </summary>
        /// <param name="asset">The asset to query. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an <see cref="IDataset"/>. </returns>
        public static Task<IDataset> GetPreviewDatasetAsync(this IAsset asset, CancellationToken cancellationToken)
        {
            const string previewTag = "Preview";
            return GetDatasetAsync(asset, previewTag, cancellationToken);
        }

        /// <summary>
        /// Returns the Source dataset for the asset.
        /// </summary>
        /// <param name="asset">The asset to query. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an <see cref="IDataset"/>. </returns>
        public static Task<IDataset> GetSourceDatasetAsync(this IAsset asset, CancellationToken cancellationToken)
        {
            const string sourceTag = "Source";
            return GetDatasetAsync(asset, sourceTag, cancellationToken);
        }

        static async Task<IDataset> GetDatasetAsync(this IAsset asset, string systemTag, CancellationToken cancellationToken)
        {
            await foreach (var dataset in asset.ListDatasetsAsync(Range.All, cancellationToken))
            {
                if (dataset.SystemTags != null && dataset.SystemTags.Contains(systemTag))
                {
                    return dataset;
                }
            }

            return null;
        }
    }
}
