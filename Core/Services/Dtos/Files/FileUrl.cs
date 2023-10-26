using System.Runtime.Serialization;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    [DataContract]
    struct FileUrl
    {
        [DataMember(Name = "datasetId")]
        public string DatasetId { get; set; }

        [DataMember(Name = "filePath")]
        public string Path { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }
    }
}
