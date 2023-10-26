using System;
using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This object contains all the information pertaining to an updated asset.
    /// </summary>
    interface IAssetUpdateData : IAssetBaseData
    {
        /// <summary>
        /// The preview file ID of the asset.
        /// </summary>
        [DataMember(Name = "previewFilePath")]
        string PreviewFile { get; }
    }
}
