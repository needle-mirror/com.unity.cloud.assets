using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents a change asset's status request.
    /// </summary>
    [DataContract]
    class CreateAssetVersionRequest : ProjectRequest
    {
        [DataMember(Name = "parentAssetVersion")]
        readonly string m_ParentVersion;

        /// <summary>
        /// Changes the asset's status Request Object.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        /// <param name="parentVersion">The version of the asset the file is linked to.</param>
        public CreateAssetVersionRequest(ProjectId projectId, AssetId assetId, AssetVersion? parentVersion)
            : base(projectId)
        {
            m_RequestUrl += $"/assets/{assetId}/versions";

            m_ParentVersion = parentVersion?.ToString();
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A </returns>
        public override HttpContent ConstructBody()
        {
            var body = IsolatedSerialization.Serialize(this, IsolatedSerialization.defaultSettings);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
