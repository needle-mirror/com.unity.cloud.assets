using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    struct StartedTransformationDto
    {
        [DataMember(Name = "transformationId")]
        public TransformationId TransformationId { get; set; }
    }
}
