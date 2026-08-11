using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    class PaginationDto
    {
        [DataMember(Name = "total")]
        public int Total { get; set; }
    }
}
