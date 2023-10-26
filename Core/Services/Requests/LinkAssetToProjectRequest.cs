using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents a link an asset to project request.
    /// </summary>
    class LinkAssetToProjectRequest : ProjectRequest
    {
        /// <summary>
        /// Link an Asset to a Project Request Object.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        /// <param name="destinationProjectId">The destination project id</param>
        /// <param name="xCorrelationId"></param>
        public LinkAssetToProjectRequest(ProjectId projectId, AssetId assetId, ProjectId destinationProjectId, string xCorrelationId = default)
            : base(projectId, xCorrelationId)
        {
            m_PathAndQueryParams += $"/assets/{assetId}/link/projects/{destinationProjectId}";
        }
    }
}
