using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This class contains all the information pertaining to a cloud project.
    /// </summary>
    [DataContract]
    class AssetProject : IProject
    {
        [DataMember(Name = "id")]
        string m_Id;

        [DataMember(Name = "name")]
        string m_Name;

        [DataMember(Name = "metadata")]
        Dictionary<string, IDeserializable> m_Metadata;

        [DataMember(Name = "storageIds")]
        string[] m_StorageIds;

        [DataMember(Name = "status")]
        string m_StatusString;

        [DataMember(Name = "userCount")]
        int? m_UserCount;

        public IOrganization Organization { get; private set; }

        /// <inheritdoc />
        public string Id
        {
            get => m_Id;
            set => m_Id = value;
        }

        /// <inheritdoc />
        public string Name
        {
            get => m_Name;
            set => m_Name = value;
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, IDeserializable> Metadata => m_Metadata;

        /// <inheritdoc />
        public IReadOnlyCollection<string> StorageIds => m_StorageIds;

        /// <inheritdoc />
        public IProject.ProjectStatus Status { get; private set; } = IProject.ProjectStatus.unknown;

        /// <inheritdoc />
        public int UserCount { get; private set; }

        [JsonConstructor]
        public AssetProject() { }

        internal AssetProject(IOrganization organization, string id, string name = default, Dictionary<string, IDeserializable> metadata = null, string[] storageIds = null, string status = nameof(IProject.ProjectStatus.unknown), int? userCount = default)
        {
            Organization = organization;
            m_Id = id;
            m_Name = name;
            m_Metadata = metadata;
            m_StorageIds = storageIds;
            m_StatusString = status;
            m_UserCount = userCount;
        }

        internal void Initialize(IOrganization organization)
        {
            Organization = organization;

            if (Enum.TryParse<IProject.ProjectStatus>(m_StatusString, out var status))
            {
                Status = status;
            }

            UserCount = m_UserCount ?? 0;
        }
    }
}
