using System.Net.Http;
using System.Text;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents an update asset file request.
    /// </summary>
    class UpdateAssetFileRequest : AssetRequest
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
        /// The asset file to update.
        /// </summary>
        public AssetFile assetFile { get; }

        /// <summary>
        /// Update Asset File Request Object.
        /// Update a single asset file.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization.</param>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        /// <param name="assetVersion">The version of the asset the file is linked to.</param>
        /// <param name="assetFile">The asset file to update.</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        public UpdateAssetFileRequest(ulong organizationId, string projectId, string assetId, int assetVersion, AssetFile assetFile, string xCorrelationId = default(string))
            : base(organizationId, projectId, xCorrelationId)
        {
            AssetId = assetId;
            AssetVersion = assetVersion;
            this.assetFile = assetFile;

            m_PathAndQueryParams += $"/assets/{AssetId}/versions/{AssetVersion}/files/{this.assetFile.Id}";
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A </returns>
        public override HttpContent ConstructBody()
        {
            var body = IsolatedJsonConvert.SerializeObject(assetFile);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
