using System.Net.Http;
using System.Text;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Builds an API request which creates a collection.
    /// </summary>
    class CreateCollectionRequest : ProjectRequest
    {
        /// <summary>
        /// Returns the collection
        /// </summary>
        IAssetCollectionData AssetCollection { get; }

        /// <summary>
        /// Initializes and returns an API request for posting collection.
        /// </summary>
        /// <param name="projectId">ID of the project</param>
        /// <param name="assetCollectionDto">The collection</param>
        /// <param name="xCorrelationId">Correlation id of the request</param>
        public CreateCollectionRequest(ProjectId projectId, IAssetCollectionData assetCollectionDto, string xCorrelationId = default)
            : base(projectId, xCorrelationId)
        {
            AssetCollection = assetCollectionDto;

            m_PathAndQueryParams += "/collections";
        }

        public override HttpContent ConstructBody()
        {
            var body = IsolatedJsonConvert.SerializeObject(AssetCollection, new CollectionPathStringConverter());
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
