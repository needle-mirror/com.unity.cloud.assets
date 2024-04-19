using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A class that defines search criteria for an <see cref="ITransformation"/> query.
    /// </summary>
    public sealed class TransformationSearchFilter
    {
        /// <summary>
        /// Sets the asset id to use for the query.
        /// </summary>
        public QueryParameter<AssetId> AssetId { get; } = new(Common.AssetId.None);

        /// <summary>
        /// Sets the asset version to use for the query.
        /// </summary>
        public QueryParameter<AssetVersion> AssetVersion { get; } = new(Common.AssetVersion.None);

        /// <summary>
        /// Sets the dataset id to use for the query.
        /// </summary>
        public QueryParameter<DatasetId> DatasetId { get; } = new(Common.DatasetId.None);

        /// <summary>
        /// Sets the status to use for the query.
        /// </summary>
        public QueryParameter<TransformationStatus?> Status { get; } = new();

        /// <summary>
        /// Sets the user id to use for the query.
        /// </summary>
        public QueryParameter<UserId> UserId { get; } = new(Common.UserId.None);
    }
}
