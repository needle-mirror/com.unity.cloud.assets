namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Base class for api requests on users.
    /// </summary>
    abstract class UsersRequest : ApiRequest
    {
        /// <summary>Accessor for genesis's organizationId </summary>
        public ulong OrganizationId { get; }

        /// <summary>Accessor for page </summary>
        public int? Page { get; }

        /// <summary>Accessor for pageSize </summary>
        public int? PageSize { get; }

        /// <summary>Accessor for enrichWithUsersCount </summary>
        public bool? EnrichWithUsersCount { get; }

        /// <summary>Accessor for xCorrelationId </summary>
        public string XCorrelationId { get; }

        /// <summary>
        /// UsersRequest Request Object.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="enrichWithUsersCount">A flag indicating whether the projects should have user count.</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        protected UsersRequest(ulong organizationId,
            int? page = default,
            int? pageSize = default,
            bool? enrichWithUsersCount = false,
            string xCorrelationId = default)
        {
            OrganizationId = organizationId;

            Page = page;
            PageSize = pageSize;
            EnrichWithUsersCount = enrichWithUsersCount;
            XCorrelationId = xCorrelationId;
        }
    }
}
