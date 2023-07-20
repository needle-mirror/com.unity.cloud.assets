using System;
using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A structure for defining the criteria of an <see cref="AssetFile"/> search request.
    /// </summary>
    public class FileSearchFilter : ComplexSearchCriteria<IAssetFile>
    {
        /// <inheritdoc cref="IAssetFile.Name"/>
        public SearchCriteria<string> Name { get; } = new(nameof(IAssetFile.Name));
        /// <inheritdoc cref="IAssetFile.Description"/>
        public SearchCriteria<string> Description { get; } = new(nameof(IAssetFile.Description));
        /// <inheritdoc cref="IAssetFile.Type"/>
        public SearchCriteria<string> Type { get; } = new(nameof(IAssetFile.Type));
        /// <inheritdoc cref="IAssetFile.Status"/>
        public SearchCriteria<string> Status { get; } = new(nameof(IAssetFile.Status));
        /// <inheritdoc cref="IAssetFile.StatusDetails"/>
        public SearchCriteria<string> StatusDetails { get; } = new(nameof(IAssetFile.StatusDetails));
        /// <inheritdoc cref="IAssetFile.Tags"/>
        public HashsetSearchCriteria<string> Tags { get; } = new(nameof(IAssetFile.Tags));
        /// <inheritdoc cref="IAssetFile.FileSize"/>
        public NullableSearchCriteria<long> FileSize { get; } = new(nameof(IAssetFile.FileSize));
        /// <inheritdoc cref="IAssetFile.Id"/>
        public SearchCriteria<string> Id { get; } = new(nameof(IAssetFile.Id));
        /// <inheritdoc cref="IAssetFile.UploadUrl"/>
        public SearchCriteria<string> UploadUrl { get; } = new(nameof(IAssetFile.UploadUrl));
        /// <inheritdoc cref="IAssetFile.DownloadUrl"/>
        public SearchCriteria<string> DownloadUrl { get; } = new(nameof(IAssetFile.DownloadUrl));
        /// <inheritdoc cref="IAssetFile.AssetId"/>
        public SearchCriteria<string> AssetId { get; } = new(nameof(IAssetFile.AssetId));
        /// <inheritdoc cref="IAssetFile.AssetVersion"/>
        public NullableSearchCriteria<int> AssetVersion { get; } = new(nameof(IAssetFile.AssetVersion));
        /// <inheritdoc cref="IAssetFile.StorageId"/>
        public SearchCriteria<string> StorageId { get; } = new(nameof(IAssetFile.StorageId));

        public override string SearchKey => nameof(IAsset.Files);
        private protected override Type InstantiatedType => typeof(AssetFile);

        private protected override void Include(object value)
        {
            if (value is IList<IAssetFile> {Count: > 0} files)
            {
                Include(files[0]);
            }

            base.Include(value);
        }

        private protected override void Exclude(object value)
        {
            if (value is IList<IAssetFile> {Count: > 0} files)
            {
                Exclude(files[0]);
            }

            base.Exclude(value);
        }

        private protected override void ForAny(object value)
        {
            if (value is List<IAssetFile> {Count: > 0} files)
            {
                ForAny(files[0]);
            }

            base.ForAny(value);
        }
    }
}
