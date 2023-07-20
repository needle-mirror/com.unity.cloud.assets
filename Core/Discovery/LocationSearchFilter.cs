using System;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A structure for defining the criteria of an <see cref="AssetLocation"/> search request.
    /// </summary>
    public class LocationSearchFilter : ComplexSearchCriteria<AssetLocation>
    {
        /// <inheritdoc cref="AssetLocation.Name"/>
        public SearchCriteria<string> Name { get; } = new(nameof(AssetLocation.Name));
        /// <inheritdoc cref="AssetLocation.Coordinates"/>
        public SearchCriteria<string> Coordinates { get; } = new(nameof(AssetLocation.Coordinates));
        /// <inheritdoc cref="AssetLocation.Format"/>
        public SearchCriteria<AssetCoordinateFormat> Format { get; } = new(nameof(AssetLocation.Format));

        public override string SearchKey => nameof(IAsset.Location);
    }
}
