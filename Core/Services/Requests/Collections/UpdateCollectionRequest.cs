using System.Net.Http;
using System.Text;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Builds an API request which updates a collection.
    /// </summary>
    class UpdateCollectionRequest : CollectionRequest
    {
        /// <summary>
        /// Returns the collection.
        /// </summary>
        IAssetCollectionData AssetCollection { get; }

        /// <summary>
        /// Initializes and returns an API request for posting collection.
        /// </summary>
        /// <param name="projectId">ID of the project</param>
        /// <param name="collectionPath">The path to the collection</param>
        /// <param name="assetCollectionDto">The collection</param>
        /// <param name="xCorrelationId">Correlation id of the request</param>
        public UpdateCollectionRequest(ProjectId projectId, CollectionPath collectionPath, IAssetCollectionData assetCollectionDto, string xCorrelationId = default)
            : base(projectId, collectionPath, xCorrelationId)
        {
            AssetCollection = assetCollectionDto;
        }

        public override HttpContent ConstructBody()
        {
            var body = IsolatedJsonConvert.SerializeObject(AssetCollection, new CollectionPathStringConverter());
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
