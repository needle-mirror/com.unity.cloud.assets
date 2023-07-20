using System.Net.Http;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents a delete asset file request.
    /// </summary>
    class DeleteAssetFileRequest : AssetRequest
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
        /// The asset file id to delete.
        /// </summary>
        public string AssetFileId { get; }

        /// <summary>
        /// Delete Asset File Request Object.
        /// Delete a single asset file.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization.</param>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        /// <param name="assetVersion">The version of the asset the file is linked to.</param>
        /// <param name="assetFileId">The asset file id to delete.</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        public DeleteAssetFileRequest(ulong organizationId, string projectId, string assetId, int assetVersion, string assetFileId, string xCorrelationId = default(string))
            : base(organizationId, projectId, xCorrelationId)
        {
            AssetId = assetId;
            AssetVersion = assetVersion;
            AssetFileId = assetFileId;

            m_PathAndQueryParams += $"/assets/{AssetId}/versions/{AssetVersion}/files/{AssetFileId}";
        }

        public override HttpContent ConstructBody()
        {
            return null;
        }
    }
}
