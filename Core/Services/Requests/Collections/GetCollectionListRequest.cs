using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Get the list of collections from a project.
    /// </summary>
    class GetCollectionListRequest : ProjectRequest
    {
        /// <summary>
        /// Initializes an API request to get the list of collections from a project.
        /// </summary>
        /// <param name="projectId">ID of the project</param>
        public GetCollectionListRequest(ProjectId projectId)
            : base(projectId)
        {
            m_RequestUrl += "/collections";
        }
    }
}
