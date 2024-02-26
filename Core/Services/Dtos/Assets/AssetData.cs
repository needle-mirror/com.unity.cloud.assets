using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    [DataContract]
    class AssetData : AssetBaseData, IAssetData
    {
        /// <inheritdoc />
        public AssetId Id { get; set; }

        /// <inheritdoc />
        public AssetVersion Version { get; set; }

        /// <inheritdoc />
        public IEnumerable<string> SystemTags { get; set; }

        /// <inheritdoc />
        public IEnumerable<string> Labels { get; set; }

        /// <inheritdoc />
        public string Status { get; set; }

        /// <inheritdoc />
        public bool IsFrozen { get; set; }

        /// <inheritdoc />
        public DateTime? Created { get; set; }

        /// <inheritdoc />
        public string CreatedBy { get; set; }

        /// <inheritdoc />
        public DateTime? Updated { get; set; }

        /// <inheritdoc />
        public string UpdatedBy { get; set; }

        /// <inheritdoc />
        public ProjectId SourceProjectId { get; set; } = ProjectId.None;

        /// <inheritdoc />
        public IEnumerable<ProjectId> LinkedProjectIds { get; set; }

        /// <inheritdoc />
        public string PreviewFile { get; set; }

        /// <inheritdoc />
        public string PreviewFileUrl { get; set; }

        /// <inheritdoc />
        public IEnumerable<FileData> Files { get; set; }

        /// <inheritdoc />
        public IEnumerable<DatasetData> Datasets { get; set; }

        /// <inheritdoc />
        public IEnumerable<CollectionPath> Collections { get; set; }

        internal AssetData()
            : this(AssetId.None, AssetVersion.None) { }

        public AssetData(string assetId, string assetVersion)
            : this(new AssetId(assetId), new AssetVersion(assetVersion)) { }

        internal AssetData(AssetId assetId, AssetVersion assetVersion)
        {
            Id = assetId;
            Version = assetVersion;
        }
    }
}
