using System;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A structure for defining the criteria of an <see cref="AssetTaxonomy"/> search request.
    /// </summary>
    public class TaxonomySearchFilter : ComplexSearchCriteria<AssetTaxonomy>
    {
        /// <inheritdoc cref="AssetTaxonomy.Level1"/>
        public SearchCriteria<string> Level1 { get; } = new(nameof(AssetTaxonomy.Level1));
        /// <inheritdoc cref="AssetTaxonomy.Level2"/>
        public SearchCriteria<string> Level2 { get; } = new(nameof(AssetTaxonomy.Level2));
        /// <inheritdoc cref="AssetTaxonomy.Level3"/>
        public SearchCriteria<string> Level3 { get; } = new(nameof(AssetTaxonomy.Level3));
        /// <inheritdoc cref="AssetTaxonomy.Level4"/>
        public SearchCriteria<string> Level4 { get; } = new(nameof(AssetTaxonomy.Level4));
        /// <inheritdoc cref="AssetTaxonomy.Level5"/>
        public SearchCriteria<string> Level5 { get; } = new(nameof(AssetTaxonomy.Level5));

        public override string SearchKey => nameof(IAsset.Taxonomy);
    }
}
