using System;
using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    struct CreatedDatasetDto
    {
        [DataMember(Name = "datasetId")]
        public string DatasetId { get; set; }
    }
}
