using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents a change asset's status request.
    /// </summary>
    class AssetRequest : ProjectRequest
    {
        /// <summary>
        /// Change the Asset's status Request Object.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        /// <param name="assetVersion">The version of the asset the file is linked to.</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        public AssetRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, string xCorrelationId = default)
            : base(projectId, xCorrelationId)
        {
            m_PathAndQueryParams += $"/assets/{assetId}/versions/{assetVersion}";
        }
    }
}
