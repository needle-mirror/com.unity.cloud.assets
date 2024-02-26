using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    struct PageTokenDto
    {
        [DataMember(Name = "next")]
        public string Token { get; set; }
    }
}
