using System.Net.Http;
using System.Text;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Insert Assets into a collection request.
    /// </summary>
    class InsertAssetsInCollectionRequest : AssetRequest
    {
        /// <summary>
        /// The path to the collection where the asset will be inserted.
        /// </summary>
        public string CollectionPath { get; }

        /// <summary>
        /// The DTO containing the asset to insert.
        /// </summary>
        public AssetsInCollectionDto AssetsInCollectionDto { get; }

        /// <summary>
        /// Insert Assets into a collection request.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization</param>
        /// <param name="projectId">ID of the project</param>
        /// <param name="collectionPath">The path to the collection</param>
        /// <param name="assetsInCollectionDto">The assets to insert to the collection</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        public InsertAssetsInCollectionRequest(ulong organizationId, string projectId, string collectionPath, AssetsInCollectionDto assetsInCollectionDto, string xCorrelationId = default(string))
            : base(organizationId, projectId, xCorrelationId)
        {
            CollectionPath = collectionPath;
            AssetsInCollectionDto = assetsInCollectionDto;

            m_PathAndQueryParams += $"/collections/{collectionPath}/assets";
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A </returns>
        public override HttpContent ConstructBody()
        {
            var body = IsolatedJsonConvert.SerializeObject(AssetsInCollectionDto);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
