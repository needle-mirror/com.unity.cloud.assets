using System;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A class the defines the search criteria for a <see cref="IVersionLabel"/> query.
    /// </summary>
    public sealed class VersionLabelSearchFilter
    {
        /// <summary>
        /// Whether the results should include archived labels.
        /// </summary>
        public QueryParameter<bool?> IsArchived { get; } = new();

        /// <summary>
        /// Whether the results should include system labels.
        /// </summary>
        public QueryParameter<bool?> IsSystemLabel { get; } = new();
    }
}
