namespace Unity.Cloud.Assets
{
    enum ChangeAssetStatusAction
    {
        approve,
        publish,
        reject,
        review,
        withdraw
    }

    /// <summary>
    /// Represents a change asset's status request.
    /// </summary>
    class ChangeAssetStatusRequest : AssetRequest
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
        /// The status of the asset.
        /// </summary>
        public ChangeAssetStatusAction StatusAction { get; }

        /// <summary>
        /// Change the Asset's status Request Object.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization.</param>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        /// <param name="assetVersion">The version of the asset the file is linked to.</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        public ChangeAssetStatusRequest(ulong organizationId, string projectId, string assetId, int assetVersion, ChangeAssetStatusAction statusAction, string xCorrelationId = default(string))
            : base(organizationId, projectId, xCorrelationId)
        {
            AssetId = assetId;
            AssetVersion = assetVersion;
            StatusAction = statusAction;

            m_PathAndQueryParams += $"/assets/{AssetId}/versions/{AssetVersion}/{StatusAction.ToString()}";
        }
    }
}
