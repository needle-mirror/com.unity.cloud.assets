using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    class StatusFlowPageDto
    {
        [DataMember(Name = "statusFlows")]
        public StatusFlowData[] StatusFlows { get; set; }

        [DataMember(Name = "total")]
        public int Total { get; set; }
    }
}
