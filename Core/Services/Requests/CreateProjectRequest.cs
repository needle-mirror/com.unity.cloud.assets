using System.Net.Http;
using System.Text;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Base class for api requests on assets.
    /// </summary>
    class CreateProjectRequest : OrganizationRequest
    {
        public IProjectBaseData Data { get; }

        /// <summary>
        /// AssetRequest Request Object.
        /// </summary>
        /// <param name="organizationId">The organization id. </param>
        /// <param name="data">The object containting the necessary information to create a project. </param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        public CreateProjectRequest(OrganizationId organizationId, IProjectBaseData data, string xCorrelationId = default)
            : base(organizationId, xCorrelationId)
        {
            Data = data;

            m_PathAndQueryParams += $"/projects";
        }

        public override HttpContent ConstructBody()
        {
            var body = IsolatedJsonConvert.SerializeObject(Data);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
