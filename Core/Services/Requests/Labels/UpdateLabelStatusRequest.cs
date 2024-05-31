using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class UpdateLabelStatusRequest : LabelRequest
    {
        public UpdateLabelStatusRequest(OrganizationId organizationId, string labelName, bool archive)
            : base(organizationId, labelName)
        {
            m_RequestUrl += archive ? "/archive" : "/unarchive";
        }
    }
}
