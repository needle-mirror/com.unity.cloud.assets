using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    struct AggregationsDto
    {
        [DataMember(Name = "aggregations")]
        public AggregateDto[] Aggregations { get; set; }
    }

    [DataContract]
    class AggregateDto
    {
        [DataMember(Name = "value")]
        public object Value { get; set; }

        [DataMember(Name = "count")]
        public int Count { get; set; }
    }
}
