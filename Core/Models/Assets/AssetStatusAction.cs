using System;
using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [Obsolete("Use IStatus instead.")]
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
