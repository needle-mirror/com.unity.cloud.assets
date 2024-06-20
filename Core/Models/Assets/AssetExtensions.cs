using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

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
        public static Task<IAsset> WithLatestVersionAsync(this IAsset asset, CancellationToken cancellationToken)
        {
            return asset.WithVersionAsync("Latest", cancellationToken);
        }

        /// <summary>
        /// Returns the version of the asset with the specified sequence number.
        /// </summary>
        /// <param name="asset">The asset to query. </param>
        /// <param name="frozenSequenceNumber">The sequence number of the version of the asset to fetch. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <exception cref="NotFoundException">If a version with the corresponding <paramref name="frozenSequenceNumber"/> is not found. </exception>
        /// <returns>A task whose result is the <see cref="IAsset"/> with the frozen version attributed to the specified sequence number. </returns>
        public static async Task<IAsset> WithVersionAsync(this IAsset asset, int frozenSequenceNumber, CancellationToken cancellationToken)
        {
            var filter = new AssetSearchFilter();
            filter.Include().FrozenSequenceNumber.WithValue(frozenSequenceNumber);

            var query = asset.QueryVersions()
                .SelectWhereMatchesFilter(filter)
                .LimitTo(new Range(0, 1))
                .ExecuteAsync(cancellationToken);

            IAsset version = null;

            var enumerator = query.GetAsyncEnumerator(cancellationToken);
            if (await enumerator.MoveNextAsync())
            {
                version = enumerator.Current;
            }

            await enumerator.DisposeAsync();

            if (version == null)
            {
                throw new NotFoundException($"Version {frozenSequenceNumber} not found for asset {asset.Descriptor.AssetId}");
            }

            return version;
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

        internal static AssetId SelectId(IAsset asset)
        {
            return asset.Descriptor.AssetId;
        }
    }
}
