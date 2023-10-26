using System;
using System.Net.Http;
using System.Text;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class CreateDatasetRequest : AssetRequest
    {
        IDatasetBaseData DatasetData { get; }

        public CreateDatasetRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, IDatasetBaseData datasetInfo, string xCorrelationId = default)
            : base(projectId, assetId, assetVersion, xCorrelationId)
        {
            m_PathAndQueryParams += $"/datasets";

            DatasetData = datasetInfo;
        }

        /// <inheritdoc/>
        public override HttpContent ConstructBody()
        {
            var body = IsolatedJsonConvert.SerializeObject(DatasetData, SerializationUtilities.DatasetIdConverter);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
