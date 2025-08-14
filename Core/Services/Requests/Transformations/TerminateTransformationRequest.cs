using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class TerminateTransformationRequest : ProjectOrLibraryRequest
    {
        public TerminateTransformationRequest(ProjectId projectId, TransformationId transformationId)
            : base(projectId)
        {
            m_RequestUrl += $"/transformations/{transformationId}/termination";
        }
    }
}
