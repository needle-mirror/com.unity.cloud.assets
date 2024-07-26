using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    struct StatusPredicateData
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}
