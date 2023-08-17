using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A structure for defining the criteria of an <see cref="IAsset"/> search request.
    /// </summary>
    public class AssetSearchFilter : IAssetSearchFilter
    {
        readonly List<ISearchCriteria> m_AllCriterias;
        readonly List<ISearchCriteria> m_UserCriterias;

        /// <inheritdoc cref="IAsset.Name"/>
        public StringSearchCriteria Name { get; } = new(nameof(IAsset.Name));

        /// <inheritdoc cref="IAsset.Description"/>
        public StringSearchCriteria Description { get; } = new(nameof(IAsset.Description));

        /// <inheritdoc cref="IAsset.Type"/>
        public SearchCriteria<string> Type { get; } = new(nameof(IAsset.Type));

        /// <inheritdoc cref="IAsset.Id"/>
        public SearchCriteria<string> Id { get; } = new(nameof(IAsset.Id));

        /// <inheritdoc cref="IAsset.ShortId"/>
        public SearchCriteria<string> ShortId { get; } = new(nameof(IAsset.ShortId));

        /// <inheritdoc cref="IAsset.ExternalId"/>
        public SearchCriteria<string> ExternalId { get; } = new(nameof(IAsset.ExternalId));

        /// <inheritdoc cref="IAsset.StorageId"/>
        public SearchCriteria<string> StorageId { get; } = new(nameof(IAsset.StorageId));

        /// <inheritdoc cref="IAsset.Version"/>
        public NullableSearchCriteria<int> Version { get; } = new(nameof(IAsset.Version));

        /// <inheritdoc cref="IAsset.VersionName"/>
        public SearchCriteria<string> VersionName { get; } = new(nameof(IAsset.VersionName));

        /// <inheritdoc cref="IAsset.Status"/>
        public SearchCriteria<string> Status { get; } = new(nameof(IAsset.Status));

        /// <inheritdoc cref="IAsset.StatusDetails"/>
        public SearchCriteria<string> StatusDetails { get; } = new(nameof(IAsset.StatusDetails));

        /// <inheritdoc cref="IAsset.Origin"/>
        public SearchCriteria<string> Origin { get; } = new(nameof(IAsset.Origin));

        /// <inheritdoc cref="IAsset.Location"/>
        public LocationSearchFilter Location { get; } = new();

        /// <inheritdoc cref="IAsset.Taxonomy"/>
        public TaxonomySearchFilter Taxonomy { get; } = new();

        /// <inheritdoc cref="IAsset.Tags"/>
        public HashsetSearchCriteria<string> Tags { get; } = new(nameof(IAsset.Tags));

        /// <inheritdoc cref="IAsset.Categories"/>
        public HashsetSearchCriteria<string> Categories { get; } = new(nameof(IAsset.Categories));

        /// <inheritdoc cref="IAsset.PreviewFileId"/>
        public SearchCriteria<string> PreviewFileId { get; } = new(nameof(IAsset.PreviewFileId));

        /// <inheritdoc cref="IAsset.Collections"/>
        public HashsetSearchCriteria<CollectionPath> Collections { get; } = new(nameof(IAsset.Collections));

        /// <inheritdoc cref="IAsset.Author"/>
        public AuthorSearchFilter Author { get; } = new();

        /// <inheritdoc cref="IAsset.Created"/>
        public NullableSearchCriteria<DateTime> Created { get; } = new(nameof(IAsset.Created));

        /// <inheritdoc cref="IAsset.CreatedBy"/>
        public SearchCriteria<string> CreatedBy { get; } = new(nameof(IAsset.CreatedBy));

        /// <inheritdoc cref="IAsset.Updated"/>
        public NullableSearchCriteria<DateTime> Updated { get; } = new(nameof(IAsset.Updated));

        /// <inheritdoc cref="IAsset.UpdatedBy"/>
        public SearchCriteria<string> UpdatedBy { get; } = new(nameof(IAsset.UpdatedBy));

        /// <inheritdoc cref="IAsset.Files"/>
        public FileSearchFilter Files { get; } = new();

        /// <inheritdoc cref="IAsset.Attachments"/>
        public AttachmentSearchFilter Attachments { get; } = new();

        /// <inheritdoc cref="IAsset.Project"/>
        public ProjectSearchFilter Project { get; } = new();

        /// <inheritdoc cref="IAsset.ProjectIds"/>
        public HashsetSearchCriteria<string> ProjectIds { get; } = new(nameof(IAsset.ProjectIds));

        /// <inheritdoc cref="IAsset.SourceProjectId"/>
        public SearchCriteria<string> SourceProjectId { get; } = new(nameof(IAsset.SourceProjectId));

        public int AnyQueryMinimumMatch { get; set; } = 1;

        public IEnumerable<ISearchCriteria> AllCriteria => m_AllCriterias.Concat(m_UserCriterias);

        public AssetSearchFilter(IProject project)
            : this()
        {
            Project.Include(project);
        }

        /// <summary>
        /// Initializes and returns an instance of <see cref="AssetSearchFilter"/>
        /// </summary>
        internal AssetSearchFilter()
        {
            m_UserCriterias = new List<ISearchCriteria>();

            m_AllCriterias = GetType()
                .GetProperties()
                .Where(x => typeof(ISearchCriteria).IsAssignableFrom(x.PropertyType))
                .Select(x => x.GetValue(this) as ISearchCriteria)
                .ToList();
        }

        /// <summary>
        /// Returns whether the current filter matches the asset being queried.
        /// </summary>
        /// <param name="asset">The <see cref="IAsset"/> to query for match. </param>
        /// <returns>True if the asset matches this search filter. </returns>
        public bool IsMatch(IAsset asset)
        {
            if (asset == null)
            {
                return false;
            }

            var isAny = 0;

            bool Match(ISearchCriteria criteria, object input)
            {
                isAny += criteria.IsAny(input) ? 1 : 0;
                return criteria.IsMatch(input);
            }

            foreach (var criterion in AllCriteria)
            {
                var value = asset.GetPropertyValue(criterion.SearchKey);
                if (!Match(criterion, value)) return false;
            }

            var hasAnyRequirements = AccumulateAnyCriteria()?.Count > 0;

            return !hasAnyRequirements || isAny >= AnyQueryMinimumMatch;
        }

        /// <summary>
        /// Includes all populated fields of the provided <see cref="IAsset"/> in the search.
        /// </summary>
        /// <param name="asset">An <see cref="IAsset"/></param>
        public void Include(IAsset asset)
        {
            foreach (var criterion in AllCriteria)
            {
                if (asset.TryGetPropertyValue(criterion.SearchKey, out var value))
                {
                    criterion.Include(value);
                }
            }
        }

        /// <summary>
        /// Excludes all populated fields of the provided <see cref="IAsset"/> in the search.
        /// </summary>
        /// <param name="asset">An <see cref="IAsset"/></param>
        public void Exclude(IAsset asset)
        {
            foreach (var criterion in AllCriteria)
            {
                if (asset.TryGetPropertyValue(criterion.SearchKey, out var value))
                {
                    criterion.Exclude(value);
                }
            }
        }

        /// <summary>
        /// Includes any populated fields of the provided <see cref="IAsset"/> in the search.
        /// </summary>
        /// <param name="asset">An <see cref="IAsset"/></param>
        public void Any(IAsset asset)
        {
            foreach (var criterion in AllCriteria)
            {
                if (asset.TryGetPropertyValue(criterion.SearchKey, out var value))
                {
                    criterion.ForAny(value);
                }
            }
        }


        /// <summary>
        /// Adds a <see cref="ISearchCriteria"/> to the search.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool AddUserCriteria(ISearchCriteria criteria)
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            if (m_UserCriterias.Contains(criteria))
                return false;

            m_UserCriterias.Add(criteria);
            return true;
        }

        /// <summary>
        /// Removes a <see cref="ISearchCriteria"/> from the search.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool RemoveUserCriteria(ISearchCriteria criteria)
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            return m_UserCriterias.Remove(criteria);
        }

        /// <inheritdoc/>
        public Dictionary<string, object> AccumulateIncludedCriteria()
        {
            var criteria = new Dictionary<string, object>();

            foreach (var criterion in AllCriteria)
            {
                criterion.Include(criteria);
            }

            return criteria.Count > 0 ? criteria : null;
        }

        /// <inheritdoc/>
        public Dictionary<string, object> AccumulateExcludedCriteria()
        {
            var criteria = new Dictionary<string, object>();

            foreach (var criterion in AllCriteria)
            {
                criterion.Exclude(criteria);
            }

            return criteria.Count > 0 ? criteria : null;
        }

        /// <inheritdoc/>
        public Dictionary<string, object> AccumulateAnyCriteria()
        {
            var criteria = new Dictionary<string, object>();

            foreach (var criterion in AllCriteria)
            {
                criterion.ForAny(criteria);
            }

            return criteria.Count > 0 ? criteria : null;
        }

        /// <inheritdoc/>
        public IProject GetProjectToSearch() => Project.GetProject();
    }
}
