using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents a change asset's status request.
    /// </summary>
    class ChangeAssetStatusRequest : AssetRequest
    {
        /// <summary>
        /// Changes the asset's status Request Object.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        /// <param name="assetVersion">The version of the asset the file is linked to.</param>
        /// <param name="action"></param>
        public ChangeAssetStatusRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, AssetStatusAction action)
            : base(projectId, assetId, assetVersion)
        {
            var status = IsolatedSerialization.SerializeWithConverters(action, IsolatedSerialization.StringEnumConverter).Replace("\"", "");
            m_RequestUrl += $"/status/{status}";
        }
    }
}
