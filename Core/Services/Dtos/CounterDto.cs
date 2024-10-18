using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    class CounterDto
    {
        [DataMember(Name = "count")]
        public int Count { get; set; }
    }
}
