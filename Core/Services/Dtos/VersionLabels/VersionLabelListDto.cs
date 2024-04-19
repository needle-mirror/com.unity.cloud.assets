using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    class VersionLabelListDto
    {
        [DataMember(Name = "results")]
        public VersionLabelData[] Versionlabels { get; set; }

        [DataMember(Name = "total")]
        public int Total { get; set; }
    }
}
