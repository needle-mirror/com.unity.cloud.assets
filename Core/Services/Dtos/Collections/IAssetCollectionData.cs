using System.Runtime.Serialization;
using Newtonsoft.Json;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This object contains the information about an asset collection.
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    interface IAssetCollectionData
    {
        /// <summary>
        /// The name of the collection.
        /// </summary>
        [DataMember(Name = "name")]
        string Name { get; set; }

        /// <summary>
        /// Describes the collection.
        /// </summary>
        [DataMember(Name = "description")]
        string Description { get; set; }

        /// <summary>
        /// Returns the path to the parent collection. The string can be empty.
        /// </summary>
        [DataMember(Name = "parentPath")]
        CollectionPath ParentPath { get; }

        /// <summary>
        /// Returns the full path to the collection.
        /// </summary>
        /// <returns>A path. </returns>
        string GetFullCollectionPath();
    }
}
