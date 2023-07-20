using System;
using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This interface contains all the information pertaining to a cloud asset file.
    /// </summary>
    public interface IAssetFile
    {
        /// <summary>
        /// The name of the asset file.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// The description of the asset file.
        /// </summary>
        string Description { get; set; }
        /// <summary>
        /// The type of the asset file.
        /// </summary>
        string Type { get; set; }
        /// <summary>
        /// The status of the asset.
        /// </summary>
        string Status { get; set; }
        /// <summary>
        /// The status details of the asset.
        /// </summary>
        string StatusDetails { get; set; }
        /// <summary>
        /// The tags of the asset file.
        /// </summary>
        List<string> Tags { get; set; }
        /// <summary>
        /// The file size of the asset file.
        /// </summary>
        long FileSize { get; set; }
        /// <summary>
        /// The id of the asset file.
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// The upload url of the asset linked to this file.
        /// </summary>
        string UploadUrl { get; set; }
        /// <summary>
        /// The download url of the asset linked to this file.
        /// </summary>
        string DownloadUrl { get; set; }
        /// <summary>
        /// The id of the asset linked to this file.
        /// </summary>
        string AssetId { get; set; }
        /// <summary>
        /// The version of the asset linked to this file.
        /// </summary>
        int AssetVersion { get; set; }
        /// <summary>
        /// The storage id of the asset file.
        /// </summary>
        string StorageId { get; set; }
    }
}
