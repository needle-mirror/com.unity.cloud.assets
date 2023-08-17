using System;
using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// The request to read the assets.
    /// </summary>
    [DataContract]
    internal class SearchRequestParameters
    {
        /// <summary>
        /// The request to read the assets.
        /// </summary>
        /// <param name="filter">filter param</param>
        /// <param name="resultFields">resultFields param</param>
        /// <param name="pagination">pagination param</param>
        /// <param name="includeThumbnailDownloadURLs">A flag indicating whether the response should have download URL&#39;s on files that are marked as thumbnails.</param>
        public SearchRequestParameters(SearchRequestFilter filter = default, SearchRequestResultFields resultFields = default, SearchRequestPagination pagination = default, bool includeThumbnailDownloadURLs = true)
        {
            Filter = filter;
            ResultFields = resultFields;
            Pagination = pagination;
            IncludeThumbnailDownloadURLs = includeThumbnailDownloadURLs;
        }

        /// <summary>
        /// Parameter filter of SearchRequest
        /// </summary>
        [DataMember(Name = "filter", EmitDefaultValue = false)]
        public SearchRequestFilter Filter{ get; }

        /// <summary>
        /// Parameter resultFields of SearchRequest
        /// </summary>
        [DataMember(Name = "resultFields", EmitDefaultValue = false)]
        public SearchRequestResultFields ResultFields{ get; }

        /// <summary>
        /// Parameter pagination of SearchRequest
        /// </summary>
        [DataMember(Name = "pagination", EmitDefaultValue = false)]
        public SearchRequestPagination Pagination{ get; }

        /// <summary>
        /// A flag indicating whether the response should have download URL&#39;s on files that are marked as thumbnails.
        /// </summary>
        [DataMember(Name = "includeThumbnailDownloadURLs", EmitDefaultValue = true)]
        public bool IncludeThumbnailDownloadURLs{ get; }
    }
}
