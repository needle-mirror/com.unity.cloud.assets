using System;
using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    struct UploadUrlDto
    {
        [DataMember(Name = "uploadUrl")]
        public string UploadUrl { get; set; }
    }
}
