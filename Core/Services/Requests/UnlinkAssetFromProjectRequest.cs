namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents an unlink asset from project request.
    /// </summary>
    class UnlinkAssetFromProjectRequest : AssetRequest
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
        /// Unlink an Asset from a Project Request Object.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization.</param>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        /// <param name="assetVersion">The version of the asset the file is linked to.</param>
        /// <param name="xCorrelationId">The correlation id.</param>
        public UnlinkAssetFromProjectRequest(ulong organizationId, string projectId, string assetId, int assetVersion, string xCorrelationId = default)
            : base(organizationId, projectId, xCorrelationId)
        {
            AssetId = assetId;
            AssetVersion = assetVersion;

            m_PathAndQueryParams += $"/assets/{AssetId}/versions/{AssetVersion}/unlink";
        }
    }
}
