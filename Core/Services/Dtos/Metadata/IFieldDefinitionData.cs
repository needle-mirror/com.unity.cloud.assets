using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    interface IFieldDefinitionData : IFieldDefinitionCreateData, IAuthoringData
    {
        [DataMember(Name = "status")]
        string Status { get; }
    }
}
