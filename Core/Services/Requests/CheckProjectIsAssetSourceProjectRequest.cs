using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents a check project is asset source project request.
    /// </summary>
    class CheckProjectIsAssetSourceProjectRequest : ProjectRequest
    {
        /// <summary>
        /// Checks whether a project is an asset source project Request Object.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        public CheckProjectIsAssetSourceProjectRequest(ProjectId projectId, AssetId assetId)
            : base(projectId)
        {
            m_PathAndQueryParams += $"/assets/{assetId}/is-source-project";
        }
    }
}
