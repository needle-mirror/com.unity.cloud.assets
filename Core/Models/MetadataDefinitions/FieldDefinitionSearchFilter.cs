namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A class that defines search criteria for an <see cref="IFieldDefinition"/> query.
    /// </summary>
    public sealed class FieldDefinitionSearchFilter
    {
        /// <summary>
        /// Sets whether to include deleted field definitions in the query.
        /// </summary>
        public QueryParameter<bool> Deleted { get; } = new(true);

        /// <summary>
        /// Sets whether to include field definitions that originate from the user or system in the query.
        /// </summary>
        public QueryParameter<FieldDefinitionOrigin?> FieldOrigin { get; } = new();
    }
}
