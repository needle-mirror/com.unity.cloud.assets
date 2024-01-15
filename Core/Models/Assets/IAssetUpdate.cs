using System;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This is a base class containing the information necessary to update an asset.
    /// </summary>
    public interface IAssetUpdate : IAssetInfo
    {
        /// <summary>
        /// The preview file path of the asset.
        /// </summary>
        string PreviewFile { get; set; }
    }
}
