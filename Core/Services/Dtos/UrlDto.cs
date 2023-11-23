using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    struct UrlDto
    {
        [DataMember(Name = "url")]
        public string Url { get; set; }
    }
}
