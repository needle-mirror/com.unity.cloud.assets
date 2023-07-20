namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This information contains information pertaining to a unity user.
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// Implement this property to return the uid of the user.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Implement this property to return the genesis id of the user.
        /// </summary>
        string GenesisId { get; }

        /// <summary>
        /// Implement this property to return the name of the user.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Implement this property to return the email of the user.
        /// </summary>
        string Email { get; }
    }
}
