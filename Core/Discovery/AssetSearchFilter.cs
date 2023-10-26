using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A structure for defining the criteria of an <see cref="IAsset"/> search request.
    /// </summary>
    public class AssetSearchFilter : IAssetSearchFilter
    {
        /// <inheritdoc cref="AssetId"/>
        public IdSearchCriteria<AssetId> Id { get; } = new(nameof(AssetDescriptor.AssetId), "assetId");

        /// <inheritdoc cref="AssetVersion"/>
        public IdSearchCriteria<AssetVersion> Version { get; } = new(nameof(AssetDescriptor.AssetVersion), "assetVersion");

        /// <inheritdoc cref="IAsset.Name"/>
        public StringSearchCriteria Name { get; } = new(nameof(IAsset.Name), "name");

        /// <inheritdoc cref="IAsset.Description"/>
        public StringSearchCriteria Description { get; } = new(nameof(IAsset.Description), "description");

        /// <inheritdoc cref="IAsset.Type"/>
        public AssetTypeSearchCriteria Type { get; } = new(nameof(IAsset.Type), AssetTypeSearchCriteria.SearchKey);

        /// <inheritdoc cref="IAsset.Status"/>
        public SearchCriteria<string> Status { get; } = new(nameof(IAsset.Status), "status");

        /// <inheritdoc cref="IAsset.Tags"/>
        public HashsetSearchCriteria<string> Tags { get; } = new(nameof(IAsset.Tags), "tags");

        /// <inheritdoc cref="IAsset.SystemTags"/>
        public HashsetSearchCriteria<string> SystemTags { get; } = new(nameof(IAsset.SystemTags), "systemTags");

        /// <inheritdoc cref="IAsset.PortalMetadata"/>
        public DeserializableSearchCriteria PortalMetadata { get; } = new(nameof(IAsset.PortalMetadata), "portalMetadata");

        /// <inheritdoc cref="IAsset.Metadata"/>
        public MetadataSearchFilter Metadata { get; } = new(nameof(IAsset.Metadata), "metadata");

        /// <inheritdoc cref="IAsset.SystemMetadata"/>
        public MetadataSearchFilter SystemMetadata { get; } = new(nameof(IAsset.SystemMetadata), "systemMetadata");

        /// <inheritdoc cref="IAsset.PreviewFile"/>
        public SearchCriteria<string> PreviewFile { get; } = new(nameof(IAsset.PreviewFile), "previewFileId");

        /// <inheritdoc cref="IAsset.SourceProject"/>
        public IdSearchCriteria<ProjectId> SourceProjectId { get; } = new(nameof(IAsset.SourceProject), "sourceProjectId");

        /// <inheritdoc cref="IAsset.AuthoringInfo"/>
        public AuthoringInfoSearchFilter AuthoringInfo { get; } = new(nameof(IAsset.AuthoringInfo), string.Empty);

        /// <inheritdoc cref="IAsset.StorageId"/>
        public SearchCriteria<string> StorageId { get; } = new(nameof(IAsset.StorageId), "storageId");

        /// <inheritdoc cref="IFile"/>
        public FileSearchFilter Files { get; } = new(nameof(Asset.Files), "files");

        /// <inheritdoc cref="IDataset"/>
        public DatasetSearchFilter Datasets { get; } = new(nameof(Asset.Datasets), "datasets");

        /// <inheritdoc />
        public List<CollectionPath> Collections { get; } = new();

        /// <inheritdoc />
        public int AnyQueryMinimumMatch { get; set; } = 1;

        /// <inheritdoc />
        public FieldsFilter IncludedFields { get; set; }

        public IEnumerable<ISearchCriteria> AllCriteria { get; }

        /// <summary>
        /// Initializes and returns an instance of <see cref="AssetSearchFilter"/>
        /// </summary>
        public AssetSearchFilter()
        {
            AllCriteria = GetType()
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

            foreach (var criterion in AllCriteria)
            {
                var value = asset.GetPropertyValue(criterion.PropertyName);
                if (!Match(criterion, value)) return false;
            }

            var hasAnyRequirements = AccumulateAnyCriteria()?.Count > 0;

            return !hasAnyRequirements || isAny >= AnyQueryMinimumMatch;

            bool Match(ISearchCriteria criteria, object input)
            {
                isAny += criteria.IsAny(input) ? 1 : 0;
                return criteria.IsMatch(input);
            }
        }

        /// <summary>
        /// Includes all populated fields of the provided <see cref="IAsset"/> in the search.
        /// </summary>
        /// <param name="asset">An <see cref="IAsset"/></param>
        public void Include(IAsset asset)
        {
            foreach (var criterion in AllCriteria)
            {
                if (asset.TryGetPropertyValue(criterion.PropertyName, out var value))
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
                if (asset.TryGetPropertyValue(criterion.PropertyName, out var value))
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
                if (asset.TryGetPropertyValue(criterion.PropertyName, out var value))
                {
                    criterion.ForAny(value);
                }
            }
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
    }
}
