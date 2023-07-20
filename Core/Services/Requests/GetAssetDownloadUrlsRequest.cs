namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents a get asset download urls request.
    /// </summary>
    class GetAssetDownloadUrlsRequest : AssetRequest
    {
        /// <summary>
        /// The id of the asset.
        /// </summary>
        public string AssetId { get; }
        /// <summary>
        /// The version of the asset.
        /// </summary>
        public int AssetVersion { get; }

        /// <summary>
        /// Get Asset Download Urls Request Object.
        /// Get a list of url for an Asset.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization.</param>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        /// <param name="assetVersion">The version of the asset the file is linked to.</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        public GetAssetDownloadUrlsRequest(ulong organizationId, string projectId, string assetId, int assetVersion, string xCorrelationId = default(string))
            : base(organizationId, projectId, xCorrelationId)
        {
            AssetId = assetId;
            AssetVersion = assetVersion;

            m_PathAndQueryParams += $"/assets/{AssetId}/versions/{AssetVersion}/download-urls";
        }
    }
}
