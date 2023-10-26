using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// The across projects search request parameters.
    /// </summary>
    [DataContract]
    class AcrossProjectsSearchRequestParameters : SearchRequestParameters
    {
        /// <summary>
        /// The across projects search request parameters.
        /// </summary>
        /// <param name="projectIds">project ids param</param>
        /// <param name="filter">filter param</param>
        /// <param name="includeFields">The fields to be returned.</param>
        /// <param name="pagination">pagination param</param>
        public AcrossProjectsSearchRequestParameters(IEnumerable<string> projectIds, SearchRequestFilter filter = default, FieldsFilter includeFields = default, SearchRequestPagination pagination = default)
            : base(filter, includeFields, pagination)
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
