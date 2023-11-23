using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    enum ChangeAssetStatusAction
    {
        approved,
        published,
        rejected,
        inreview,
        withdrawn
    }

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
        /// <param name="statusAction"></param>
        public ChangeAssetStatusRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, ChangeAssetStatusAction statusAction)
            : base(projectId, assetId, assetVersion)
        {
            m_PathAndQueryParams += $"/status/{statusAction.ToString()}";
        }
    }
}
