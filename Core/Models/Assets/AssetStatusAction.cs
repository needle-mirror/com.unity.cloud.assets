using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    public enum AssetStatusAction
    {
        [EnumMember(Value = "inreview")]
        SendForReview,
        [EnumMember(Value = "approved")]
        Approve,
        [EnumMember(Value = "rejected")]
        Reject,
        [EnumMember(Value = "published")]
        Publish,
        [EnumMember(Value = "withdrawn")]
        Withdraw
    }
}
