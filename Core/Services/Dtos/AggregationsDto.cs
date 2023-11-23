using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    struct AggregationsDto
    {
        [DataMember(Name = "aggregations")]
        public AggregateDto[] Aggregations { get; set; }
    }

    struct AggregateDto
    {
        [DataMember(Name = "value")]
        public string Value { get; set; }

        [DataMember(Name = "count")]
        public int Count { get; set; }

        public AggregateDto(string value, int count)
        {
            Value = value;
            Count = count;
        }
    }
}
