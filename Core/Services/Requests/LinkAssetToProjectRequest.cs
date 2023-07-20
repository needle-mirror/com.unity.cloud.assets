namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents a link an asset to project request.
    /// </summary>
    class LinkAssetToProjectRequest : AssetRequest
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
        /// The destination organization id.
        /// </summary>
        public ulong DestinationOrganizationId { get; }

        /// <summary>
        /// The destination project id.
        /// </summary>
        public string DestinationProjectId { get; }

        /// <summary>
        /// Link an Asset to a Project Request Object.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization.</param>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        /// <param name="assetVersion">The version of the asset the file is linked to.</param>
        /// <param name="destinationOrganizationId">The destination organization id.</param>
        /// <param name="destinationProjectId">The destination project id</param>
        public LinkAssetToProjectRequest(ulong organizationId, string projectId, string assetId, int assetVersion, ulong destinationOrganizationId, string destinationProjectId)
            : base(organizationId, projectId, default)
        {
            AssetId = assetId;
            AssetVersion = assetVersion;
            DestinationOrganizationId = destinationOrganizationId;
            DestinationProjectId = destinationProjectId;

            m_PathAndQueryParams += $"/assets/{AssetId}/versions/{AssetVersion}/link/organizations/{DestinationOrganizationId}/projects/{DestinationProjectId}";
        }
    }
}
