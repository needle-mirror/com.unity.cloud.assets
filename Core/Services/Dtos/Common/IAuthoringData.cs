using System;
using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    interface IAuthoringData
    {
        [DataMember(Name = "createdBy")]
        string CreatedBy => null;

        [DataMember(Name = "created")]
        DateTime? Created => null;

        [DataMember(Name = "updatedBy")]
        string UpdatedBy => null;

        [DataMember(Name = "updated")]
        DateTime? Updated => null;
    }
}
