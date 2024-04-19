using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    [DataContract]
    class AssignVersionLabelRequest : AssetRequest
    {
        [DataMember(Name = "labelNames")]
        readonly string[] m_VersionLabels;

        public AssignVersionLabelRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, bool assign, IEnumerable<string> versionLabels)
            : base(projectId, assetId, assetVersion)
        {
            m_RequestUrl += $"/labels/{(assign ? "assign" : "unassign")}";

            m_VersionLabels = versionLabels.ToArray();
        }

        public override HttpContent ConstructBody()
        {
            var body = IsolatedSerialization.Serialize(this, IsolatedSerialization.defaultSettings);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
