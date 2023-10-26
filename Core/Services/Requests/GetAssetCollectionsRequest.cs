using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents a get asset collections request.
    /// </summary>
    class GetAssetCollectionsRequest : ProjectRequest
    {
        /// <summary>
        /// Get Asset Collections Request Object.
        /// Get the collections of an Asset.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        /// <param name="assetVersion">The version of the asset the file is linked to.</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        public GetAssetCollectionsRequest(ProjectId projectId, AssetId assetId,string xCorrelationId = default)
            : base(projectId, xCorrelationId)
        {
            m_PathAndQueryParams += $"/assets/{assetId}/collections";
        }
    }
}
