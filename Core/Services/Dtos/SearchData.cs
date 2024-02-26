namespace Unity.Cloud.Assets
{
    class SearchData
    {
        /// <summary>
        /// Defines the search criteria for retrieving a set of assets.
        /// </summary>
        public IAssetSearchFilter AssetSearchFilter { get; set; } = new AssetSearchFilter();

        /// <summary>
        /// Defines which fields of the results will be populated.
        /// </summary>
        public FieldsFilter IncludedFields { get; set; }

        /// <summary>
        /// Defines the range and order of the results.
        /// </summary>
        public PaginationData Pagination { get; set; }
    }

    class AggregationData
    {
        /// <summary>
        /// Defines the search criteria for retrieving a set of assets.
        /// </summary>
        public IAssetSearchFilter AssetSearchFilter { get; set; } = new AssetSearchFilter();

        /// <summary>
        ///
        /// </summary>
        public string AggregationField { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int? ResultLimit { get; set; }
    }
}
