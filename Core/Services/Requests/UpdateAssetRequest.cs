using System.Net.Http;
using System.Text;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents an update asset request.
    /// </summary>
    class UpdateAssetRequest : AssetRequest
    {
        /// <summary>
        /// The asset to update.
        /// </summary>
        public IAsset Asset { get; }

        /// <summary>
        /// Update Asset Request Object.
        /// Update a single asset.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization.</param>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="asset">The asset to update.</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        public UpdateAssetRequest(ulong organizationId, string projectId, IAsset asset, string xCorrelationId = default)
            : base(organizationId, projectId, xCorrelationId)
        {
            Asset = asset;

            m_PathAndQueryParams += $"/assets/{Asset.Id}/versions/{Asset.Version}";
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A </returns>
        public override HttpContent ConstructBody()
        {
            var body = IsolatedJsonConvert.SerializeObject(Asset);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
