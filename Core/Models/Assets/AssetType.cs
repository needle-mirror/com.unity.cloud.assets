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
            TryGetAssetTypeFromString(value, out var assetType);
            return assetType;
        }

        /// <summary>
        /// Returns the AssetType from the string value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="assetType"></param>
        /// <returns></returns>
        public static bool TryGetAssetTypeFromString(this string value, out AssetType assetType)
        {
            assetType = AssetType.Other;

            switch (value.Trim())
            {
                case var s when s.Equals("2D Asset", StringComparison.OrdinalIgnoreCase):
                    assetType = AssetType.Asset_2D;
                    break;
                case var s when s.Equals("3D Model", StringComparison.OrdinalIgnoreCase):
                case var s1 when s1.Equals("Model", StringComparison.OrdinalIgnoreCase)://To support old data format for 3D Model
                    assetType = AssetType.Model_3D;
                    break;
                case var s when s.Equals("Audio", StringComparison.OrdinalIgnoreCase):
                    assetType = AssetType.Audio;
                    break;
                case var s when s.Equals("Material", StringComparison.OrdinalIgnoreCase):
                    assetType = AssetType.Material;
                    break;
                case var s when s.Equals("Other", StringComparison.OrdinalIgnoreCase):
                    assetType = AssetType.Other;
                    break;
                case var s when s.Equals("Script", StringComparison.OrdinalIgnoreCase):
                    assetType = AssetType.Script;
                    break;
                case var s when s.Equals("Video", StringComparison.OrdinalIgnoreCase):
                    assetType = AssetType.Video;
                    break;
                default:
                    return false;
            }

            return true;
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
