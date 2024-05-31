using System;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A class the defines the search criteria for a <see cref="ILabel"/> query.
    /// </summary>
    public sealed class LabelSearchFilter
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
