using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    [DataContract]
    class AddFileReferenceRequest : FileRequest
    {
        [DataMember(Name="targetDatasetId")]
        DatasetId m_DatasetId;

        public AddFileReferenceRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId, string filePath, DatasetId targetDatasetId, string xCorrelationId = default)
            : base(projectId, assetId, assetVersion, datasetId, filePath, xCorrelationId)
        {
            m_PathAndQueryParams += "/reference";
            m_DatasetId = targetDatasetId;
        }

        public override HttpContent ConstructBody()
        {
            var body = IsolatedJsonConvert.SerializeObject(this, SerializationUtilities.DatasetIdConverter);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
