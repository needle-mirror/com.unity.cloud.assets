using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    class AssetStatusDto
    {
        [DataMember(Name = "statusFlow")]
        public StatusFlowData StatusFlow { get; set; }

        [DataMember(Name = "currentStatus")]
        public string CurrentStatusId { get; set; }
    }
}
