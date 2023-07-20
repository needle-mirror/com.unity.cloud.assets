using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This class contains all the information pertaining to an asset file.
    /// </summary>
    [DataContract]
    public class AssetFile : IAssetFile, IAssetAttachment
    {
        [DataMember(Name = "name")]
        string m_Name;
        [DataMember(Name = "description")]
        string m_Description;
        [DataMember(Name = "version")]
        int m_Version;
        [DataMember(Name = "taxonomy")]
        AssetTaxonomy m_Taxonomy;
        [DataMember(Name = "tags")]
        List<string> m_Tags;
        [DataMember(Name = "fileSize")]
        long m_FileSize;
        [DataMember(Name = "type")]
        string m_Type;
        [DataMember(Name = "details")]
        Dictionary<string, IDeserializable> m_Details;
        [DataMember(Name = "metadata")]
        Dictionary<string, IDeserializable> m_Metadata;
        [DataMember(Name = "id")]
        string m_ID;
        [DataMember(Name = "status")]
        string m_Status;
        [DataMember(Name = "statusDetails")]
        string m_StatusDetails;
        [DataMember(Name = "storage")]
        string m_StorageId;
        [DataMember(Name = "uploadUrl")]
        string m_UploadUrl;
        [DataMember(Name = "downloadUrl")]
        string m_DownloadUrl;
        [DataMember(Name = "assetId")]
        string m_AssetId;
        [DataMember(Name = "assetVersion")]
        int m_AssetVersion;

        public string Name
        {
            get => m_Name;
            set => m_Name = value;
        }

        public string Description
        {
            get => m_Description;
            set => m_Description = value;
        }

        public string Type
        {
            get => m_Type;
            set => m_Type = value;
        }

        public string Status
        {
            get => m_Status;
            set => m_Status = value;
        }

        public string StatusDetails
        {
            get => m_StatusDetails;
            set => m_StatusDetails = value;
        }

        public List<string> Tags
        {
            get => m_Tags;
            set => m_Tags = value;
        }

        public long FileSize
        {
            get => m_FileSize;
            set => m_FileSize = value;
        }

        public string Id
        {
            get => m_ID;
            set => m_ID = value;
        }

        public string UploadUrl
        {
            get => m_UploadUrl;
            set => m_UploadUrl = value;
        }

        public string DownloadUrl
        {
            get => m_DownloadUrl;
            set => m_DownloadUrl = value;
        }

        public string AssetId
        {
            get => m_AssetId;
            set => m_AssetId = value;
        }

        public int AssetVersion
        {
            get => m_AssetVersion;
            set => m_AssetVersion = value;
        }

        public string StorageId
        {
            get => m_StorageId;
            set => m_StorageId = value;
        }

        public Dictionary<string, IDeserializable> Details
        {
            get => m_Details;
            set => m_Details = value;
        }

        public Dictionary<string, IDeserializable> Metadata
        {
            get => m_Metadata;
            set => m_Metadata = value;
        }

        [JsonConstructor]
        public AssetFile()
        {
            m_Details = new Dictionary<string, IDeserializable>();
            m_Metadata = new Dictionary<string, IDeserializable>();
        }
    }
}
