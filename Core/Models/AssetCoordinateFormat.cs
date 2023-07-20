using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// The asset coordinate format for the asset location.
    /// </summary>
    /// <value>The asset coordinate format for the asset location.</value>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AssetCoordinateFormat
    {
        /// <summary>
        /// Enum Unknown for value: unknown
        /// </summary>
        [EnumMember(Value = "unknown")]
        Unknown = 1,
        /// <summary>
        /// Enum Decimal degrees for value: decimal degrees
        /// </summary>
        [EnumMember(Value = "decimaldegrees")]
        Decimaldegrees = 2,
        /// <summary>
        /// Enum Degrees minute seconds for value: degrees minute seconds
        /// </summary>
        [EnumMember(Value = "degreesminuteseconds")]
        Degreesminuteseconds = 3,
        /// <summary>
        /// Enum Geohash for value: geohash
        /// </summary>
        [EnumMember(Value = "geohash")]
        Geohash = 4,
        /// <summary>
        /// Enum Utm for value: utm
        /// </summary>
        [EnumMember(Value = "utm")]
        Utm = 5
    }
}
