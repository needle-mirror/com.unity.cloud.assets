namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents a get asset collections request.
    /// </summary>
    class GetAssetCollectionsRequest : AssetRequest
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
        /// Get Asset Collections Request Object.
        /// Get the collections of an Asset.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization.</param>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        /// <param name="assetVersion">The version of the asset the file is linked to.</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        public GetAssetCollectionsRequest(ulong organizationId, string projectId, string assetId, int assetVersion, string xCorrelationId = default(string))
            : base(organizationId, projectId, xCorrelationId)
        {
            AssetId = assetId;
            AssetVersion = assetVersion;

            m_PathAndQueryParams += $"/assets/{assetId}/versions/{assetVersion}/collections";
        }
    }
}
