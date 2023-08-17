using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This is a base class containing the information pertaining to an asset.
    /// </summary>
    [DataContract(Name = "asset")]
    public class Asset : IAsset
    {
        [DataMember(Name = "name")]
        string m_Name;
        [DataMember(Name = "description")]
        string m_Description;
        [DataMember(Name = "type")]
        string m_Type;
        [DataMember(Name = "details")]
        Dictionary<string, IDeserializable> m_Details;
        [DataMember(Name = "metadata")]
        Dictionary<string, IDeserializable> m_Metadata;
        [DataMember(Name = "files")]
        protected internal List<AssetFile> m_Files;
        [DataMember(Name = "attachments")]
        protected internal List<AssetFile> m_Attachments;
        [DataMember(Name = "collections")]
        protected internal List<CollectionPath> m_Collections;

        /// <inheritdoc />
        public IProject Project { get; set; }

        /// <inheritdoc />
        public string Name
        {
            get => m_Name;
            set => m_Name = value ?? throw new ArgumentException(nameof(Name));
        }

        /// <inheritdoc />
        public string Description
        {
            get => m_Description;
            set => m_Description = value ?? throw new ArgumentException(nameof(Description));
        }

        /// <inheritdoc />
        [DataMember(Name = "version")]
        public int Version { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "taxonomy")]
        public AssetTaxonomy Taxonomy { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "tags")]
        public List<string> Tags { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "origin")]
        public string Origin { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "shortId")]
        public string ShortId { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "versionName")]
        public string VersionName { get; set; }

        /// <inheritdoc />
        public string Type
        {
            get => m_Type;
            set => m_Type = value ?? throw new ArgumentException(nameof(Type));
        }

        /// <inheritdoc />
        [DataMember(Name = "location")]
        public AssetLocation Location { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "categories")]
        public List<string> Categories { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "externalId")]
        public string ExternalId { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "author")]
        public AssetAuthor Author { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "previewFileId")]
        public string PreviewFileId { get; set; }

        /// <inheritdoc />
        public IEnumerable<CollectionPath> Collections => m_Collections;

        /// <inheritdoc />
        public IEnumerable<IAssetFile> Files => m_Files;

        /// <inheritdoc />
        public IEnumerable<IAssetAttachment> Attachments => m_Attachments;

        /// <inheritdoc />
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "status")]
        public string Status { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "statusDetails")]
        public string StatusDetails { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "created")]
        public DateTime Created { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "createdBy")]
        public string CreatedBy { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "updated")]
        public DateTime Updated { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "updatedBy")]
        public string UpdatedBy { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "storageId")]
        public string StorageId { get; set; }

        /// <inheritdoc />
        [DataMember(Name = "projectIds")]
        public List<string> ProjectIds { get; }

        /// <inheritdoc />
        [DataMember(Name = "sourceProjectId")]
        public string SourceProjectId { get; set; }

        [JsonConstructor]
        public Asset()
        {
            m_Attachments = new List<AssetFile>();
            m_Files = new List<AssetFile>();

            m_Details = new Dictionary<string, IDeserializable>();
            Tags = new List<string>();
            m_Collections = new List<CollectionPath>();
            Categories = new List<string>();
            m_Metadata = new Dictionary<string, IDeserializable>();
            ProjectIds = new List<string>();
        }

        /// <summary>
        /// Updates the files and attachments.
        /// </summary>
        /// <param name="assetFiles">An updated list of files. </param>
        /// <param name="assetAttachments">An updated list of attachments. </param>
        public void OnFilesUpdated(IEnumerable<AssetFile> assetFiles, IEnumerable<AssetFile> assetAttachments)
        {
            m_Files = assetFiles.ToList();
            m_Attachments = assetAttachments.ToList();
        }

        /// <summary>
        /// Updates the collection list.
        /// </summary>
        /// <param name="assetCollections">An updated list of collections. </param>
        public void OnCollectionsUpdated(IEnumerable<AssetCollection> assetCollections)
        {
            m_Collections = new List<CollectionPath>();

            foreach (var assetCollection in assetCollections)
            {
                if (assetCollection == null)
                {
                    continue;
                }
                m_Collections.Add(assetCollection.Name);
            }
        }
    }
}
