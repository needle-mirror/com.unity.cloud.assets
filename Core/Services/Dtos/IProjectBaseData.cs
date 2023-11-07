using System.Runtime.Serialization;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This interface contains all the information about a cloud project.
    /// </summary>
    interface IProjectBaseData
    {
        /// <summary>
        /// The project name.
        /// </summary>
        [DataMember(Name = "name")]
        string Name { get; }

        /// <summary>
        /// The project metadata.
        /// </summary>
        [DataMember(Name = "metadata")]
        IDeserializable Metadata { get; }
    }
}
