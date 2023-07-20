using System.Net.Http;
using System.Text;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Builds an API request which creates a collection.
    /// </summary>
    class CreateCollectionRequest : AssetRequest
    {
        /// <summary>
        /// Returns the collection
        /// </summary>
        public IAssetCollection AssetCollection { get; }

        /// <summary>
        /// Initializes and returns an API request for posting collection.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization</param>
        /// <param name="projectId">ID of the project</param>
        /// <param name="assetCollectionDto">The collection</param>
        /// <param name="xCorrelationId">Correlation id of the request</param>
        public CreateCollectionRequest(ulong organizationId, string projectId, IAssetCollection assetCollectionDto, string xCorrelationId = default(string))
            : base(organizationId, projectId, xCorrelationId)
        {
            AssetCollection = assetCollectionDto;

            m_PathAndQueryParams += "/collections";
        }

        public override HttpContent ConstructBody()
        {
            var body = IsolatedJsonConvert.SerializeObject(AssetCollection);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
