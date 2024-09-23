using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Base class for api requests on assets.
    /// </summary>
    class ProjectRequest : ApiRequest
    {
        /// <summary>
        /// AssetRequest Request Object.
        /// </summary>
        /// <param name="projectId">ID of the project</param>
        public ProjectRequest(ProjectId projectId)
        {
            m_RequestUrl = $"/projects/{projectId}";
        }

        public static ProjectRequest GetEnableProjectRequest(ProjectId projectId)
        {
            var request = new ProjectRequest(projectId);
            request.m_RequestUrl += "/enable";
            return request;
        }
    }
}
