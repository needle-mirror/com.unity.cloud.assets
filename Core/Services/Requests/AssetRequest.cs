using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents a change asset's status request.
    /// </summary>
    class AssetRequest : ProjectRequest
    {
        /// <summary>
        /// Changes the asset's status Request Object.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        /// <param name="assetVersion">The version of the asset the file is linked to.</param>
        public AssetRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion)
            : base(projectId)
        {
            m_PathAndQueryParams += $"/assets/{assetId}/versions/{assetVersion}";
        }
    }
}
