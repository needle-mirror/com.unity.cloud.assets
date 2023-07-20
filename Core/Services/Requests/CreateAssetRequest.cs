using System.Net.Http;
using System.Text;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents a create asset request.
    /// </summary>
    class CreateAssetRequest : AssetRequest
    {
        /// <summary>
        /// The asset to create.
        /// </summary>
        public IAsset Asset { get; }

        /// <summary>
        /// Create Asset Request Object.
        /// Create a single asset.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization.</param>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="asset">The asset to create.</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        public CreateAssetRequest(ulong organizationId, string projectId, IAsset asset, string xCorrelationId = default)
            : base(organizationId, projectId, xCorrelationId)
        {
            Asset = asset;

            m_PathAndQueryParams += "/assets";
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
