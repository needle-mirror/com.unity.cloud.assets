using System.Net.Http;
using System.Text;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class UpdateDatasetRequest : DatasetRequest
    {
        IDatasetUpdateData Data { get; }

        public UpdateDatasetRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId, IDatasetUpdateData data, string xCorrelationId = null)
            : base(projectId, assetId, assetVersion, datasetId, xCorrelationId)
        {
            Data = data;
        }

        public override HttpContent ConstructBody()
        {
            var body = IsolatedJsonConvert.SerializeObject(Data);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
