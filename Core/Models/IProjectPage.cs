namespace Unity.Cloud.Assets
{
    public interface IProjectPage : IPagedResponse<IProject>
    {
        /// <summary>
        /// Implement this property to return the organization id of the page.
        /// </summary>
        IOrganization Organization { get; }

        /// <summary>
        /// Implement this property to return the user id of the page.
        /// </summary>
        string UserId { get; }
    }
}
