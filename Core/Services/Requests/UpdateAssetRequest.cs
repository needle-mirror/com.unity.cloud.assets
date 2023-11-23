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
        IAssetBaseData Data { get; }

        /// <summary>
        /// Update Asset Request Object.
        /// Update a single asset.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="data"></param>
        /// <param name="assetId"></param>
        /// <param name="assetVersion"></param>
        public UpdateAssetRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, IAssetBaseData data)
            : base(projectId, assetId, assetVersion)
        {
            Data = data;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A </returns>
        public override HttpContent ConstructBody()
        {
            var body = IsolatedSerialization.SerializeWithDefaultConverters(Data);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
