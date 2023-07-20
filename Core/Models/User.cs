namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This class contains all information pertaining to a unity user.
    /// </summary>
    class User : IUser
    {
        /// <inheritdoc />
        public string Id { get; set; }

        /// <inheritdoc />
        public string GenesisId { get; set; }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public string Email { get; set; }
    }
}
