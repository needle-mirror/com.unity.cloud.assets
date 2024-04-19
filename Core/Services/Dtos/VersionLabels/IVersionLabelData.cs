using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    interface IVersionLabelData : IVersionLabelBaseData, IAuthoringData
    {
        [DataMember(Name = "isSystemLabel")]
        bool IsSystemLabel { get; }

        [DataMember(Name = "isUserAssignable")]
        bool IsUserAssignable { get; }
    }
}
