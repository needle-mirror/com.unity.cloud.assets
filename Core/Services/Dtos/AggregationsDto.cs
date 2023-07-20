using Newtonsoft.Json;

namespace Unity.Cloud.Assets
{
    struct AggregationsDto
    {
        [JsonProperty("aggregations")]
        public AggregateDto[] Aggregations { get; set; }
    }

    struct AggregateDto
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        public AggregateDto(string value, int count)
        {
            Value = value;
            Count = count;
        }
    }
}
