using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// The request to read the assets.
    /// </summary>
    [DataContract]
    class SearchRequestParameters
    {
        /// <summary>
        /// The request to read the assets.
        /// </summary>
        /// <param name="filter">filter param</param>
        /// <param name="includeFields">The fields to be returned.</param>
        /// <param name="pagination">pagination param</param>
        public SearchRequestParameters(SearchRequestFilter filter = default, FieldsFilter includeFields = default, SearchRequestPagination pagination = default)
        {
            Filter = filter;
            includeFields?.Parse(AddIncludeField);
            Pagination = pagination;
        }

        /// <summary>
        /// Parameter filter of SearchRequest
        /// </summary>
        [DataMember(Name = "filter", EmitDefaultValue = false)]
        public SearchRequestFilter Filter { get; }

        /// <summary>
        /// The fields to be returned.
        /// </summary>
        [DataMember(Name = "includeFields", EmitDefaultValue = false)]
        public List<string> IncludeFields { get; } = new();

        /// <summary>
        /// Parameter pagination of SearchRequest
        /// </summary>
        [DataMember(Name = "pagination", EmitDefaultValue = false)]
        public SearchRequestPagination Pagination { get; }

        void AddIncludeField(string field)
        {
            IncludeFields.Add(field);
        }
    }
}
