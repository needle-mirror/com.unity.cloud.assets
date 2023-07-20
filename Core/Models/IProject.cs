using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This interface contains all the information pertaining to a cloud project.
    /// </summary>
    public interface IProject
    {
        public enum ProjectStatus
        {
            unknown,
            active,
            archived
        }

        /// <summary>
        /// The project ID.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The project name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The project metadata.
        /// </summary>
        IReadOnlyDictionary<string, IDeserializable> Metadata { get; }

        /// <summary>
        /// The project's storage IDs.
        /// </summary>
        IReadOnlyCollection<string> StorageIds { get; }

        /// <summary>
        /// The project's status.
        /// </summary>
        ProjectStatus Status { get; }

        /// <summary>
        /// The project's users count.
        /// </summary>
        int UserCount { get; }
    }
}
