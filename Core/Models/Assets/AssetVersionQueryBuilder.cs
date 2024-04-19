using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A class that builds and executes a query to return a set of asset versions.
    /// </summary>
    public sealed class AssetVersionQueryBuilder
    {
        readonly IAssetDataSource m_DataSource;
        readonly ProjectDescriptor m_ProjectDescriptor;
        readonly AssetId m_AssetId;

        IAssetSearchFilter m_Filter;
        string m_SortingField = "versionNumber";
        SortingOrder m_SortingOrder = SortingOrder.Ascending;
        Range m_Range = Range.All;

        internal AssetVersionQueryBuilder(IAssetDataSource dataSource, ProjectDescriptor projectDescriptor, AssetId assetId)
        {
            m_DataSource = dataSource;
            m_ProjectDescriptor = projectDescriptor;
            m_AssetId = assetId;
        }

        /// <summary>
        /// Sets the filter to be used when querying asset versions.
        /// </summary>
        /// <param name="filter">The query filter. </param>
        /// <returns>The calling <see cref="AssetVersionQueryBuilder"/>. </returns>
        public AssetVersionQueryBuilder SelectWhereMatchesFilter(IAssetSearchFilter filter)
        {
            m_Filter = filter;
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sortingField"></param>
        /// <param name="sortingOrder"></param>
        /// <returns>The calling <see cref="AssetVersionQueryBuilder"/>. </returns>
        public AssetVersionQueryBuilder OrderBy(string sortingField, SortingOrder sortingOrder = SortingOrder.Ascending)
        {
            m_SortingField = sortingField;
            m_SortingOrder = sortingOrder;
            return this;
        }

        /// <summary>
        /// Sets the range of results to return.
        /// </summary>
        /// <param name="range">The range of results. </param>
        /// <returns>The calling <see cref="AssetVersionQueryBuilder"/>. </returns>
        public AssetVersionQueryBuilder LimitTo(Range range)
        {
            m_Range = range;
            return this;
        }

        /// <summary>
        /// Executes the query and returns the versions of the specified <see cref="AssetId"/> which satisfy the criteria.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>An async enumeration of <see cref="IAsset"/> with the same <see cref="AssetId"/>. </returns>
        public async IAsyncEnumerable<IAsset> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var parameters = new SearchRequestParameters(FieldsFilter.DefaultAssetIncludes)
            {
                Filter = m_Filter?.From(),
                Pagination = new SearchRequestPagination(m_SortingField, m_SortingOrder),
                PaginationRange = m_Range
            };

            var results = m_DataSource.ListAssetVersionsAsync(m_ProjectDescriptor, m_AssetId, parameters, cancellationToken);
            await foreach (var result in results)
            {
                yield return result.From(m_DataSource, m_ProjectDescriptor, FieldsFilter.DefaultAssetIncludes);
            }
        }
    }
}
