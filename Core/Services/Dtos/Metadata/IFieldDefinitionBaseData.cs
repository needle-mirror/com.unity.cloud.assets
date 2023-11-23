using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    interface IFieldDefinitionBaseData
    {
        [DataMember(Name = "displayName")]
        string DisplayName { get; }

        [DataMember(Name = "acceptedValues")]
        string[] AcceptedValues { get; }
    }
}
