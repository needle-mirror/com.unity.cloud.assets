namespace Unity.Cloud.Assets
{
    struct TransformationSearchData
    {
        public string AssetId { get; set; }
        public string AssetVersion { get; set; }
        public string DatasetId { get; set; }
        public string UserId { get; set; }
        public TransformationStatus? Status { get; set; }
    }
}
