using System;
using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This object contains all the information pertaining to an updated asset.
    /// </summary>
    [DataContract]
    class AssetUpdateData : AssetBaseData, IAssetUpdateData
    {
        /// <inheritdoc />
        public string PreviewFile { get; set; }
    }
}
