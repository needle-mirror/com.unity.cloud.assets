using System;
using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// The location the asset has been defined.
    /// </summary>
    [DataContract]
    public class AssetLocation
    {
        /// <summary>
        /// The name of the location.
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// The coordinates for the location.
        /// </summary>
        [DataMember(Name = "coordinates")]
        public string Coordinates { get; set; }

        /// <summary>
        /// The asset coordinate format for the asset location.
        /// </summary>
        [DataMember(Name = "format")]
        public AssetCoordinateFormat Format { get; set; }
    }
}
