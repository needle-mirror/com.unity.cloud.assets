namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A class that defines search criteria for an <see cref="AssetUpdateHistory"/> query.
    /// </summary>
    public sealed class AssetUpdateHistorySearchFilter
    {
        /// <summary>
        /// Sets whether to include dataset and file entries in the query.
        /// </summary>
        public QueryParameter<bool> IncludeDatasetsAndFiles { get; } = new();
    }
}
