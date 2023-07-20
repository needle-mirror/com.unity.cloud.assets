using System.Net.Http;
using System.Text;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Builds an API request which updates a collection.
    /// </summary>
    class UpdateCollectionRequest : CollectionRequest
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
        /// <param name="collectionPath">The path to the collection</param>
        /// <param name="assetCollectionDto">The collection</param>
        /// <param name="xCorrelationId">Correlation id of the request</param>
        public UpdateCollectionRequest(ulong organizationId, string projectId, string collectionPath, IAssetCollection assetCollectionDto, string xCorrelationId = default(string))
            : base(organizationId, projectId, collectionPath, xCorrelationId)
        {
            AssetCollection = assetCollectionDto;
        }

        public override HttpContent ConstructBody()
        {
            var body = IsolatedJsonConvert.SerializeObject(AssetCollection);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
