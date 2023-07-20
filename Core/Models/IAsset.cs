using System;
using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This interface contains all the information pertaining to a cloud asset.
    /// </summary>
    public interface IAsset
    {
        /// <summary>
        /// The name of the asset.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// The description of the asset.
        /// </summary>
        string Description { get; set; }
        /// <summary>
        /// The version of the asset.
        /// </summary>
        int Version { get; set; }

        /// <summary>
        /// The organization of the asset.
        /// </summary>
        IOrganization Organization { get; set; }

        /// <summary>
        /// The project of the asset.
        /// </summary>
        IProject Project { get; set; }

        /// <summary>
        /// The taxonomy of the asset.
        /// </summary>
        AssetTaxonomy Taxonomy { get; set; }
        /// <summary>
        /// The tags of the asset.
        /// </summary>
        List<string> Tags { get; set; }
        /// <summary>
        /// The origin of the asset.
        /// </summary>
        string Origin { get; set; }
        /// <summary>
        /// The short ID of the asset.
        /// </summary>
        string ShortId { get; set; }
        /// <summary>
        /// The version name of the asset.
        /// </summary>
        string VersionName { get; set; }
        /// <summary>
        /// The type of the asset.
        /// </summary>
        string Type { get; set; }
        /// <summary>
        /// The location of the asset.
        /// </summary>
        AssetLocation Location { get; set; }
        /// <summary>
        /// The categories of the asset.
        /// </summary>
        List<string> Categories { get; set; }
        /// <summary>
        /// The external ID of the asset.
        /// </summary>
        string ExternalId { get; set; }
        /// <summary>
        /// The author of the asset.
        /// </summary>
        AssetAuthor Author { get; set; }
        /// <summary>
        /// The preview file ID of the asset.
        /// </summary>
        string PreviewFileId { get; set; }
        /// <summary>
        /// The collections of the asset.
        /// </summary>
        IEnumerable<string> Collections { get;}
        /// <summary>
        /// The files of the asset.
        /// </summary>
        IEnumerable<IAssetFile> Files { get; }
        /// <summary>
        /// The attachments of the asset.
        /// </summary>
        IEnumerable<IAssetAttachment> Attachments { get; }
        /// <summary>
        /// The id of the asset.
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// The status of the asset.
        /// </summary>
        string Status { get; set; }
        /// <summary>
        /// The status details of the asset.
        /// </summary>
        string StatusDetails { get; set; }
        /// <summary>
        /// The created date of the asset.
        /// </summary>
        DateTime Created { get; set; }
        /// <summary>
        /// The created by of the asset.
        /// </summary>
        string CreatedBy { get; set; }
        /// <summary>
        /// The updated date of the asset.
        /// </summary>
        DateTime Updated { get; set; }
        /// <summary>
        /// The updated by of the asset.
        /// </summary>
        string UpdatedBy { get; set; }
        /// <summary>
        /// The storage id of the asset.
        /// </summary>
        string StorageId { get; }

        /// <summary>
        /// Implement this method to handle the addition of new files and attachments.
        /// </summary>
        /// <param name="assetFiles">An updated list of files. </param>
        /// <param name="assetAttachments">An updated list of attachements. </param>
        void OnFilesUpdated(IEnumerable<AssetFile> assetFiles, IEnumerable<AssetFile> assetAttachments);

        /// <summary>
        /// Implement this method to handle the addition of new collections.
        /// </summary>
        /// <param name="assetCollection">An updated list of collections. </param>
        void OnCollectionsUpdated(IEnumerable<AssetCollection> assetCollection);
    }
}
