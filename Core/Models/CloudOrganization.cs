using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This class contains all the information pertaining to a cloud organization.
    /// </summary>
    [DataContract]
    class CloudOrganization : IOrganization
    {
        [DataMember(Name = "id")]
        string m_Id;

        [DataMember(Name = "genesisId")]
        string m_GenesisIdStr;
        ulong m_GenesisId;

        [DataMember(Name = "name")]
        string m_Name;

        /// <inheritdoc />
        public string Id
        {
            get => m_Id;
            set => m_Id = value;
        }

        /// <inheritdoc />
        public ulong GenesisId
        {
            get => m_GenesisId;
            set
            {
                m_GenesisId = value;
                m_GenesisIdStr = m_GenesisId.ToString();
            }
        }

        /// <inheritdoc />
        public string Name
        {
            get => m_Name;
            set => m_Name = value;
        }

        /// <inheritdoc />
        public List<IProject> Projects { get; } = new();

        [JsonConstructor]
        public CloudOrganization() { }

        internal void Initialize()
        {
            if (!ulong.TryParse(m_GenesisIdStr, out var genesisId))
            {
                throw new ArgumentException($"GenesisID of organization {Name} could not be parsed.");
            }

            m_GenesisId = genesisId;
        }
    }
}
