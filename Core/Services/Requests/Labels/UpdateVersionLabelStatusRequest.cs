using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class UpdateVersionLabelStatusRequest : VersionLabelRequest
    {
        public UpdateVersionLabelStatusRequest(OrganizationId organizationId, string labelName, bool archive)
            : base(organizationId, labelName)
        {
            m_RequestUrl += archive ? "/archive" : "/unarchive";
        }
    }
}
