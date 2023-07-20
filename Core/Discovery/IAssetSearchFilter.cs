using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A structure for defining the criteria of an <see cref="IAsset"/> search request.
    /// </summary>
    public interface IAssetSearchFilter
    {
        /// <summary>
        /// Returns the number of matches required for a search to be considered a match.
        /// </summary>
        int AnyQueryMinimumMatch { get; }

        /// <summary>
        /// Returns whether the current filter matches the asset being queried.
        /// </summary>
        /// <param name="asset">The <see cref="IAsset"/> to query for match. </param>
        /// <returns>True if the asset matches this search filter. </returns>
        bool IsMatch(IAsset asset);

        /// <summary>
        /// Includes all populated fields of the provided <see cref="IAsset"/> in the search.
        /// </summary>
        /// <param name="asset">An <see cref="IAsset"/></param>
        void Include(IAsset asset);

        /// <summary>
        /// Excludes all populated fields of the provided <see cref="IAsset"/> in the search.
        /// </summary>
        /// <param name="asset">An <see cref="IAsset"/></param>
        void Exclude(IAsset asset);

        /// <summary>
        /// Includes all populated fields of the provided <see cref="IAsset"/> as optional criteria in the search.
        /// </summary>
        /// <param name="asset">An <see cref="IAsset"/></param>
        void Any(IAsset asset);

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

        /// <summary>
        /// Returns the default organization for the search.
        /// </summary>
        /// <returns>An organization to be used in the search. </returns>
        IOrganization GetOrganizationToSearch();

        /// <summary>
        /// Returns the default project for the search.
        /// </summary>
        /// <returns>A project to be used in the search. </returns>
        IProject GetProjectToSearch();
    }
}
