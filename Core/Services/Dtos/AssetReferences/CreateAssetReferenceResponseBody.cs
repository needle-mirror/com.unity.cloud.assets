using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    struct CreateAssetReferenceResponseBody
    {
        [DataMember(Name = "referenceId")]
        public string ReferenceId { get; set; }
    }
}
