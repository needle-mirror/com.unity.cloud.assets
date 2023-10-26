using System.Runtime.Serialization;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This interface contains all the information pertaining to a cloud project.
    /// </summary>
    interface IProjectData : IProjectBaseData
    {
        /// <summary>
        /// The project ID.
        /// </summary>
        [DataMember(Name = "id")]
        ProjectId Id { get; }
    }
}
