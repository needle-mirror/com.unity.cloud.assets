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
        public LinkAssetToProjectRequest(ProjectId projectId, AssetId assetId, ProjectId destinationProjectId)
            : base(projectId)
        {
            m_RequestUrl += $"/assets/{assetId}/link/projects/{destinationProjectId}";
        }
    }
}
