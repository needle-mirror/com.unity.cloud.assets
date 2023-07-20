using System.Net.Http;
using System.Text;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents a create asset file request.
    /// </summary>
    class CreateAssetFileRequest : AssetRequest
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
        /// The asset file to create.
        /// </summary>
        public AssetFile assetFile { get; }

        /// <summary>
        /// Create Asset File Request Object.
        /// Create a single asset file.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization.</param>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file will linked to.</param>
        /// <param name="assetVersion">The version of the asset the file will linked to.</param>
        /// <param name="assetFile">The asset file to create.</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        public CreateAssetFileRequest(ulong organizationId, string projectId, string assetId, int assetVersion, AssetFile assetFile, string xCorrelationId = default(string))
            : base(organizationId, projectId, xCorrelationId)
        {
            AssetId = assetId;
            AssetVersion = assetVersion;

            m_PathAndQueryParams += $"/assets/{AssetId}/versions/{AssetVersion}/files";

            this.assetFile = assetFile;
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
