using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This interface represents an organization.
    /// </summary>
    public interface IOrganization
    {
        /// <summary>
        /// The organization ID.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The organization's genesis ID.
        /// </summary>
        ulong GenesisId { get; }

        /// <summary>
        /// The organization name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The organization's projects.
        /// </summary>
        List<IProject> Projects { get; }
    }
}
