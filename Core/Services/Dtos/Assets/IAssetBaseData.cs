using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This object contains all the information about an updated asset.
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    interface IAssetBaseData : IMetadataInfo
    {
        /// <summary>
        /// The name of the asset.
        /// </summary>
        [DataMember(Name = "name")]
        string Name { get; }

        /// <summary>
        /// The description of the asset.
        /// </summary>
        [DataMember(Name = "description")]
        string Description { get; }

        /// <summary>
        /// The tags of the asset.
        /// </summary>
        [DataMember(Name = "tags")]
        IEnumerable<string> Tags { get; }

        /// <summary>
        /// The type of the asset.
        /// </summary>
        [DataMember(Name = "primaryType")]
        AssetType Type { get; }
    }
}
