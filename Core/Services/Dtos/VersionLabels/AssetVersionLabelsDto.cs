using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    class AssetVersionLabelListDto
    {
        [DataMember(Name = "assetVersionLabels")]
        public AssetVersionLabelsDto[] AssetVersionLabels { get; set; }

        [DataMember(Name = "total")]
        public int Total { get; set; }
    }

    [DataContract]
    class AssetVersionLabelsDto
    {
        [DataMember(Name = "assetVersion")]
        public string AssetVersion { get; set; }

        [DataMember(Name = "labels")]
        public VersionLabelData[] Labels { get; set; }

        [DataMember(Name = "archivedLabels")]
        public VersionLabelData[] ArchivedLabels { get; set; }
    }
}
