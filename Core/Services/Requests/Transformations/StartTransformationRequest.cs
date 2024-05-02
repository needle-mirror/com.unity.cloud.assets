using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents a get transformation URL request.
    /// </summary>
    [DataContract]
    class StartTransformationRequest : DatasetRequest
    {
        [DataMember(Name = "inputFiles")]
        string[] m_InputFiles;

        public StartTransformationRequest(string workflowType, IEnumerable<string> inputFiles,
            ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId)
            : base(projectId, assetId, assetVersion, datasetId)
        {
            m_RequestUrl += $"/transformations/start/{workflowType}";

            m_InputFiles = inputFiles?.ToArray();
        }

        public override HttpContent ConstructBody()
        {
            var body = IsolatedSerialization.Serialize(this, IsolatedSerialization.defaultSettings);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
