namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents a check project is asset source project request.
    /// </summary>
    class CheckProjectIsAssetSourceProjectRequest : AssetRequest
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
        /// Check project is Asset source project Request Object.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization.</param>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        /// <param name="assetVersion">The version of the asset the file is linked to.</param>
        /// <param name="xCorrelationId">The correlation id.</param>
        public CheckProjectIsAssetSourceProjectRequest(ulong organizationId, string projectId, string assetId, int assetVersion, string xCorrelationId = default)
            : base(organizationId, projectId, xCorrelationId)
        {
            AssetId = assetId;
            AssetVersion = assetVersion;

            m_PathAndQueryParams += $"/assets/{AssetId}/versions/{AssetVersion}/is-source-project";
        }
    }
}
