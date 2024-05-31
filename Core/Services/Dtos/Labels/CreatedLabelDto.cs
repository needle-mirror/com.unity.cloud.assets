using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    struct CreatedLabelDto
    {
        [DataMember(Name = "labelName")]
        public string Name { get; set; }
    }
}
