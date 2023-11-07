using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This interface contains all the information about a cloud asset.
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    interface IAssetData : IAssetBaseData, IAuthoringData
    {
        /// <summary>
        /// The storage id of the asset.
        /// </summary>
        [DataMember(Name = "storageId")]
        string StorageId { get; }

        /// <summary>
        /// The id of the asset.
        /// </summary>
        [DataMember(Name = "assetId")]
        AssetId Id { get; }

        /// <summary>
        /// The version of the asset.
        /// </summary>
        [DataMember(Name = "assetVersion")]
        AssetVersion Version { get; }

        /// <summary>
        /// The tags of the asset.
        /// </summary>
        [DataMember(Name = "systemTags")]
        IEnumerable<string> SystemTags { get; set; }

        /// <summary>
        /// The labels of the asset.
        /// </summary>
        [DataMember(Name = "labels")]
        IEnumerable<string> Labels { get; set; }

        /// <summary>
        /// The status of the asset.
        /// </summary>
        [DataMember(Name = "status")]
        string Status { get; set; }

        /// <summary>
        /// Whether the asset is frozen.
        /// </summary>
        [DataMember(Name = "isFrozen")]
        bool IsFrozen { get; set; }

        /// <summary>
        /// The source id of the project the asset belongs to.
        /// </summary>
        [DataMember(Name = "sourceProjectId")]
        ProjectId SourceProjectId { get; set; }

        /// <summary>
        /// The project ids to which the asset is linked.
        /// </summary>
        [DataMember(Name = "projectIds")]
        IEnumerable<ProjectId> LinkedProjectIds { get; }

        /// <summary>
        /// The preview file ID of the asset.
        /// </summary>
        [DataMember(Name = "previewFile")]
        string PreviewFile { get; set; }

        /// <summary>
        /// The preview file ID of the asset.
        /// </summary>
        [DataMember(Name = "previewFileUrl")]
        string PreviewFileUrl { get; set; }

        /// <summary>
        /// The files associated with the asset's datasets.
        /// </summary>
        [DataMember(Name = "files")]
        IEnumerable<FileData> Files { get; set; }

        /// <summary>
        /// The datasets of the asset.
        /// </summary>
        [DataMember(Name = "datasets")]
        IEnumerable<DatasetData> Datasets { get; set; }

        /// <summary>
        /// The collections the asset belongs to
        /// </summary>
        [DataMember(Name = "collections")]
        IEnumerable<CollectionPath> Collections { get; set; }
    }
}
