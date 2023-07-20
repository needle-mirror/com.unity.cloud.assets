using System;
using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Author of the asset.
    /// </summary>
    [DataContract]
    public class AssetAuthor
    {
        /// <summary>
        /// Name of the author.
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Company of the author.
        /// </summary>
        [DataMember(Name = "company")]
        public string Company { get; set; }

        /// <summary>
        /// Email address of the author.
        /// </summary>
        [DataMember(Name = "emailAddress")]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Details of the author.
        /// </summary>
        [DataMember(Name = "details")]
        public string Details { get; set; }
    }
}
