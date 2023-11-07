using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    ///  The across projects search and aggregate request parameters.
    /// </summary>
    [DataContract]
    class AcrossProjectsSearchAndAggregateRequestParameters : SearchAndAggregateRequestParameters
    {
        /// <summary>
        /// The across projects search and aggregate request parameters.
        /// </summary>
        /// <param name="projectIds">project ids param</param>
        /// <param name="filter">filter param</param>
        /// <param name="aggregateBy">The field that can be used in the aggregation.</param>
        /// <param name="maximumNumberOfItems">The maximum number of items to be returned.</param>
        public AcrossProjectsSearchAndAggregateRequestParameters(IEnumerable<ProjectId> projectIds, SearchRequestFilter filter = default, string aggregateBy = default, int? maximumNumberOfItems = default)
            : base(filter, aggregateBy, maximumNumberOfItems)
        {
            ProjectIds = projectIds.ToArray();
        }

        /// <summary>
        /// Parameter project ids of AcrossProjectsSearchAndAggregateRequest
        /// </summary>
        [DataMember(Name = "projectIds", EmitDefaultValue = false)]
        public ProjectId[] ProjectIds { get; set; }
    }
}
