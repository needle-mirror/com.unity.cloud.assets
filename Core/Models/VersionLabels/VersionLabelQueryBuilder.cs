using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A class that builds and executes a query to return a set of version labels.
    /// </summary>
    public sealed class VersionLabelQueryBuilder
    {
        readonly IAssetDataSource m_DataSource;
        readonly OrganizationId m_OrganizationId;

        VersionLabelSearchFilter m_Filter;
        Range m_Range = Range.All;

        internal VersionLabelQueryBuilder(IAssetDataSource dataSource, OrganizationId organizationId)
        {
            m_DataSource = dataSource;
            m_OrganizationId = organizationId;
        }

        /// <summary>
        /// Sets the filter to be used when querying version labels.
        /// </summary>
        /// <param name="filter">The query filter. </param>
        /// <returns>The calling <see cref="VersionLabelQueryBuilder"/>. </returns>
        public VersionLabelQueryBuilder SelectWhereMatchesFilter(VersionLabelSearchFilter filter)
        {
            m_Filter = filter;
            return this;
        }

        /// <summary>
        /// Sets the range of results to return.
        /// </summary>
        /// <param name="range">The range of results. </param>
        /// <returns>The calling <see cref="VersionLabelQueryBuilder"/>. </returns>
        public VersionLabelQueryBuilder LimitTo(Range range)
        {
            m_Range = range;
            return this;
        }

        /// <summary>
        /// Executes the query and returns the version labels that satisfy the criteria.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>An async enumeration of <see cref="IVersionLabel"/>. </returns>
        public async IAsyncEnumerable<IVersionLabel> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            m_Filter ??= new VersionLabelSearchFilter();

            var pagination = new PaginationData
            {
                Range = m_Range
            };

            var results = m_DataSource.ListVersionLabelsAsync(m_OrganizationId,
                pagination,
                m_Filter.IsArchived.GetValue(),
                m_Filter.IsSystemLabel.GetValue(),
                cancellationToken);
            await foreach (var result in results)
            {
                yield return result.From(m_DataSource, m_OrganizationId);
            }
        }
    }
}
