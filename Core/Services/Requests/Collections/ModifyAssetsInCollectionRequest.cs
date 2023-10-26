using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Insert Assets into a collection request.
    /// </summary>
    class ModifyAssetsInCollectionRequest : CollectionRequest
    {
        /// <summary>
        /// The DTO containing the asset to insert.
        /// </summary>
        public IEnumerable<AssetId> AssetsInCollectionDto { get; }

        /// <summary>
        /// Insert Assets into a collection request.
        /// </summary>
        /// <param name="projectId">ID of the project</param>
        /// <param name="collectionPath">The path to the collection</param>
        /// <param name="assets">The assets to insert to the collection</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        public ModifyAssetsInCollectionRequest(ProjectId projectId, CollectionPath collectionPath, IEnumerable<AssetId> assets, string xCorrelationId = default)
            : base(projectId, collectionPath, xCorrelationId)
        {
            AssetsInCollectionDto = assets;

            m_PathAndQueryParams += $"/assets";
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A </returns>
        public override HttpContent ConstructBody()
        {
            string[] ids = AssetsInCollectionDto.Select(id => id.ToString()).ToArray();
            var serializedList = IsolatedJsonConvert.SerializeObject(ids);
            var body = $"{{\"assetIds\": {serializedList}}}";
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
