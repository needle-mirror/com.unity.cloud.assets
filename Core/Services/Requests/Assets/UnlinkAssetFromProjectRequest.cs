using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents an unlink asset from project request.
    /// </summary>
    class UnlinkAssetFromProjectRequest : ProjectRequest
    {
        /// <summary>
        /// Unlink an Asset from a Project Request Object.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        public UnlinkAssetFromProjectRequest(ProjectId projectId, AssetId assetId)
            : base(projectId)
        {
            m_RequestUrl += $"/assets/{assetId}/unlink";
        }
    }
}
