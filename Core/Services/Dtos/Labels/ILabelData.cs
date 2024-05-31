using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    interface ILabelData : ILabelBaseData, IAuthoringData
    {
        [DataMember(Name = "isSystemLabel")]
        bool IsSystemLabel { get; }

        [DataMember(Name = "isUserAssignable")]
        bool IsUserAssignable { get; }
    }
}
