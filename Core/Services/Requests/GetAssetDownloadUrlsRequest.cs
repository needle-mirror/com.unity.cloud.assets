using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents a get asset download urls request.
    /// </summary>
    class GetAssetDownloadUrlsRequest : AssetRequest
    {
        /// <summary>
        /// Get Asset Download Urls Request Object.
        /// Get a list of url for an Asset.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        /// <param name="assetVersion">The version of the asset the file is linked to.</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        public GetAssetDownloadUrlsRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, string xCorrelationId = default(string))
            : base(projectId, assetId, assetVersion, xCorrelationId)
        {
            m_PathAndQueryParams += $"/download-urls";
        }
    }
}
