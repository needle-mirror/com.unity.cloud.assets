namespace Unity.Cloud.Assets
{
    public readonly struct AggregationParameters
    {
        public string AggregationField { get; }
        public int? ResultLimit { get; }

        public AggregationParameters(string aggregationField, int? resultLimit = null)
        {
            AggregationField = aggregationField;
            ResultLimit = resultLimit;
        }
    }
}
