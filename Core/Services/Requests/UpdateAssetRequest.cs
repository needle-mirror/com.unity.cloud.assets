using System.Net.Http;
using System.Text;
using Unity.Cloud.Common;

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
        public IAssetBaseData Data { get; }

        /// <summary>
        /// Update Asset Request Object.
        /// Update a single asset.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="data"></param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        /// <param name="assetId"></param>
        /// <param name="assetVersion"></param>
        public UpdateAssetRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, IAssetBaseData data, string xCorrelationId = default)
            : base(projectId, assetId, assetVersion, xCorrelationId)
        {
            Data = data;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A </returns>
        public override HttpContent ConstructBody()
        {
            var body = IsolatedJsonConvert.SerializeObject(Data, SerializationUtilities.Converters);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
