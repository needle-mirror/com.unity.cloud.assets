using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents a create asset request.
    /// </summary>
    class CreateAssetRequest : ProjectRequest
    {
        /// <summary>
        /// The asset to create.
        /// </summary>
        public IAssetBaseData Asset { get; }

        /// <summary>
        /// Create Asset Request Object.
        /// Create a single asset.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="asset">The asset to create.</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        public CreateAssetRequest(ProjectId projectId, IAssetBaseData asset, string xCorrelationId = default)
            : base(projectId, xCorrelationId)
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
            JsonConverter[] converters = {
                new CollectionPathStringConverter()
            };

            var body = IsolatedJsonConvert.SerializeObject(Asset, converters);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
