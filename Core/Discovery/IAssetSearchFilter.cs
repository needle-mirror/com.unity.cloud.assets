using System;
using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A structure for defining the criteria of an <see cref="IAsset"/> search request.
    /// </summary>
    public interface IAssetSearchFilter
    {
        /// <summary>
        /// Returns the collections in which to search for assets.
        /// </summary>
        List<CollectionPath> Collections { get; }

        /// <summary>
        /// Returns the number of matches required for a search to be considered a match.
        /// </summary>
        int AnyQueryMinimumMatch { get; }

        /// <summary>
        /// Returns which fields of the results will be populated.
        /// </summary>
        FieldsFilter IncludedFields { get; }

        /// <summary>
        /// Gets the required search criteria of the filter.
        /// </summary>
        /// <returns>A dictionary containing the required search criteria. </returns>
        Dictionary<string, object> AccumulateIncludedCriteria();

        /// <summary>
        /// Gets the excluded search criteria of the filter.
        /// </summary>
        /// <returns>A dictionary containing the excluded search criteria. </returns>
        Dictionary<string, object> AccumulateExcludedCriteria();

        /// <summary>
        /// Gets the optional search criteria of the filter.
        /// </summary>
        /// <returns>A dictionary containing the optional search criteria. </returns>
        Dictionary<string, object> AccumulateAnyCriteria();
    }
}
