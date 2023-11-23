using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    struct PageTokenDto
    {
        [DataMember(Name = "nextPaginationToken")]
        public string Token { get; set; }
    }
}
