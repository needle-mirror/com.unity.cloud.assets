namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents a get asset file url request.
    /// </summary>
    class GetAssetFileUrlRequest : AssetRequest
    {
        /// <summary>
        /// The id of the asset the file is linked to.
        /// </summary>
        public string AssetId { get; }
        /// <summary>
        /// The version of the asset the file is linked to.
        /// </summary>
        public int AssetVersion { get; }
        /// <summary>
        /// The asset file id url to get.
        /// </summary>
        public string AssetFileId { get; }

        /// <summary>
        /// Get Asset File Url Request Object.
        /// Get a single asset file url.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization.</param>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        /// <param name="assetVersion">The version of the asset the file is linked to.</param>
        /// <param name="assetFileId">The asset file id url to get.</param>
        /// <param name="urlType">The asset file's url type</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        public GetAssetFileUrlRequest(ulong organizationId, string projectId, string assetId, int assetVersion, string assetFileId, AssetFileUrlType urlType, string xCorrelationId = default(string))
            : base(organizationId, projectId, xCorrelationId)
        {
            AssetId = assetId;
            AssetVersion = assetVersion;
            AssetFileId = assetFileId;

            m_PathAndQueryParams += $"/assets/{AssetId}/versions/{AssetVersion}/files/{AssetFileId}/url?urlType={urlType.ToString().ToLower()}";
        }
    }
}
