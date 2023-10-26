using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Asset's type accepted values.
    /// </summary>
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AssetType
    {
        [EnumMember(Value = "2D Asset")]
        Asset_2D,
        [EnumMember(Value = "3D Model")]
        Model_3D,
        [EnumMember(Value = "Audio")]
        Audio,
        [EnumMember(Value = "Material")]
        Material,
        [EnumMember(Value = "Other")]
        Other,
        [EnumMember(Value = "Script")]
        Script,
        [EnumMember(Value = "Video")]
        Video
    }

    public static class AssetTypeExtensions
    {
        /// <summary>
        /// Returns the string value of the AssetType.
        /// </summary>
        /// <param name="assetType"></param>
        /// <returns></returns>
        public static string GetValueAsString(this AssetType assetType)
        {
            switch (assetType)
            {
                case AssetType.Asset_2D:
                    return "2D Asset";
                case AssetType.Model_3D:
                    return "3D Model";
                case AssetType.Audio:
                    return "Audio";
                case AssetType.Material:
                    return "Material";
                case AssetType.Other:
                    return "Other";
                case AssetType.Script:
                    return "Script";
                case AssetType.Video:
                    return "Video";
                default:
                    return "Other";
            }
        }

        /// <summary>
        /// Returns the AssetType from the string value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static AssetType GetAssetTypeFromString(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return AssetType.Other;

            switch (value.Trim())
            {
                case var s when s.Equals("2D Asset", StringComparison.OrdinalIgnoreCase):
                    return AssetType.Asset_2D;
                case var s when s.Equals("3D Model", StringComparison.OrdinalIgnoreCase):
                case var s1 when s1.Equals("Model", StringComparison.OrdinalIgnoreCase)://To support old data format for 3D Model
                    return AssetType.Model_3D;
                case var s when s.Equals("Audio", StringComparison.OrdinalIgnoreCase):
                    return AssetType.Audio;
                case var s when s.Equals("Material", StringComparison.OrdinalIgnoreCase):
                    return AssetType.Material;
                case var s when s.Equals("Other", StringComparison.OrdinalIgnoreCase):
                    return AssetType.Other;
                case var s when s.Equals("Script", StringComparison.OrdinalIgnoreCase):
                    return AssetType.Script;
                case var s when s.Equals("Video", StringComparison.OrdinalIgnoreCase):
                    return AssetType.Video;
                default:
                    return AssetType.Other;
            }
        }

        /// <summary>
        /// Returns a list of all the AssetType values.
        /// </summary>
        /// <returns></returns>
        public static List<string> AssetTypeList()
        {
            var assetTypes = new List<string>();

            foreach (var value in Enum.GetValues(typeof(AssetType)))
            {
                assetTypes.Add(((AssetType)value).GetValueAsString());
            }

            return assetTypes;
        }
    }
}
