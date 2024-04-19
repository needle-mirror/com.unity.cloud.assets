using System;
using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A structure for defining the criteria of an <see cref="IAsset"/> search request.
    /// </summary>
    public class AssetSearchFilter : IAssetSearchFilter
    {
        readonly AssetSearchCriteria m_Include = new();
        readonly AssetSearchCriteria m_Exclude = new();
        readonly AssetSearchCriteriaWithMinimumMatch m_Any = new();

        /// <inheritdoc />
        public QueryListParameter<CollectionPath> Collections { get; } = new();

        public AssetSearchCriteria Include() => m_Include;
        public AssetSearchCriteria Exclude() => m_Exclude;
        public AssetSearchCriteriaWithMinimumMatch Any() => m_Any;

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, object> AccumulateIncludedCriteria()
        {
            var criteria = new Dictionary<string, object>();

            m_Include.Include(criteria);

            return criteria.Count > 0 ? criteria : null;
        }

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, object> AccumulateExcludedCriteria()
        {
            var criteria = new Dictionary<string, object>();

            m_Exclude.Include(criteria);

            return criteria.Count > 0 ? criteria : null;
        }

        /// <inheritdoc/>
        public (IReadOnlyDictionary<string, object> criteria, int minimumMatches) AccumulateAnyCriteria()
        {
            var criteria = new Dictionary<string, object>();

            m_Any.Include(criteria);

            return (criteria.Count > 0 ? criteria : null, m_Any.MinimumMatch);
        }
    }
}
