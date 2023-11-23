using System;
using System.Runtime.Serialization;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    [DataContract]
    struct CreatedDatasetDto
    {
        [DataMember(Name = "datasetId")]
        public DatasetId DatasetId { get; set; }
    }
}
