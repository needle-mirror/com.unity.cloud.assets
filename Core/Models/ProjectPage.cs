namespace Unity.Cloud.Assets
{
    class ProjectPage : PagedResponse<IProject>, IProjectPage
    {
        readonly IUsersDataSource m_UsersDataSource;

        /// <inheritdoc/>
        public IOrganization Organization { get; }

        /// <inheritdoc/>
        public string UserId { get; }

        internal ProjectPage(IUsersDataSource usersDataSource, IProject[] projects, IProjectPage previousPage = null)
            : this(usersDataSource, previousPage.Organization, previousPage.UserId, previousPage.Pagination, projects)
        {
            PreviousPage = previousPage;
            PageEndIndex = (PreviousPage?.PageEndIndex ?? -1) + projects.Length;
        }

        internal ProjectPage(IUsersDataSource usersDataSource, IOrganization organization, string userId, Pagination pagination, IProject[] projects)
            : base(projects, default, null, -1)
        {
            m_UsersDataSource = usersDataSource;

            Organization = organization;
            UserId = userId;
            Pagination = pagination;
        }
    }
}
