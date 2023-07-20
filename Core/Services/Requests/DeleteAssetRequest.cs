namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents a delete asset request.
    /// </summary>
    class DeleteAssetRequest : AssetRequest
    {
        /// <summary>
        /// The id of the asset the file will linked to.
        /// </summary>
        public string AssetId { get; }
        /// <summary>
        /// The version of the asset the file will linked to.
        /// </summary>
        public int AssetVersion { get; }

        /// <summary>
        /// Delete Asset Request Object.
        /// Delete a single asset.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization.</param>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset to delete.</param>
        /// <param name="assetVersion">The version of the asset to delete.</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        public DeleteAssetRequest(ulong organizationId, string projectId, string assetId, int assetVersion, string xCorrelationId = default)
            : base(organizationId, projectId, xCorrelationId)
        {
            AssetId = assetId;
            AssetVersion = assetVersion;

            m_PathAndQueryParams += $"/assets/{AssetId}/versions/{AssetVersion}";
        }
    }
}
