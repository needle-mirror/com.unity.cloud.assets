using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// The across projects search request parameters.
    /// </summary>
    [DataContract]
    internal class AcrossProjectsSearchRequestParameters : SearchRequestParameters
    {
        /// <summary>
        /// The across projects search request parameters.
        /// </summary>
        /// <param name="projectIds">project ids param</param>
        /// <param name="filter">filter param</param>
        /// <param name="resultFields">resultFields param</param>
        /// <param name="pagination">pagination param</param>
        /// <param name="includeThumbnailDownloadURLs">A flag indicating whether the response should have download URL&#39;s on files that are marked as thumbnails.</param>
        public AcrossProjectsSearchRequestParameters(IEnumerable<string> projectIds, SearchRequestFilter filter = default, SearchRequestResultFields resultFields = default, SearchRequestPagination pagination = default, bool includeThumbnailDownloadURLs = true)
            : base(filter, resultFields, pagination, includeThumbnailDownloadURLs)
        {
            ProjectIds = projectIds.ToArray();
        }

        /// <summary>
        /// Parameter project ids of AcrossProjectsSearchRequest
        /// </summary>
        [DataMember(Name = "projectIds", EmitDefaultValue = false)]
        public string[] ProjectIds { get; set; }
    }
}
