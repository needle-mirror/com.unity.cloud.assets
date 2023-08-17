using System;
using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// The request for searching for aggregations.
    /// </summary>
    [DataContract]
    internal class SearchAndAggregateRequestParameters
    {
        /// <summary>
        /// The request for searching for aggregations.
        /// </summary>
        /// <param name="filter">filter param</param>
        /// <param name="aggregateBy">The field that can be used in the aggregation.</param>
        /// <param name="maximumNumberOfItems">The maximum number of items to be returned.</param>
        public SearchAndAggregateRequestParameters(SearchRequestFilter filter = default, string aggregateBy = default, int? maximumNumberOfItems = default)
        {
            Filter = filter;
            AggregateBy = aggregateBy;
            MaximumNumberOfItems = maximumNumberOfItems;
        }

        /// <summary>
        /// Parameter filter of SearchAndAggregateRequest
        /// </summary>
        [DataMember(Name = "filter", EmitDefaultValue = false)]
        public SearchRequestFilter Filter{ get; }

        /// <summary>
        /// The field that can be used in the aggregation.
        /// </summary>
        [DataMember(Name = "aggregateBy", EmitDefaultValue = false)]
        public string AggregateBy{ get; }

        /// <summary>
        /// The maximum number of items to be returned.
        /// </summary>
        [DataMember(Name = "maximumNumberOfItems", EmitDefaultValue = false)]
        public int? MaximumNumberOfItems{ get; }
    }
}
