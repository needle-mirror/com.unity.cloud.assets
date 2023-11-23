using System;

namespace Unity.Cloud.Assets
{
    public class AuthoringInfo
    {
        /// <summary>
        /// The id of the user who created.
        /// </summary>
        public string CreatedBy { get; }

        /// <summary>
        /// The date and time of creation.
        /// </summary>
        public DateTime Created { get; }

        /// <summary>
        /// The id of the user who updated.
        /// </summary>
        public string UpdatedBy { get; }

        /// <summary>
        /// The date and time of update.
        /// </summary>
        public DateTime Updated { get; }

        internal AuthoringInfo(string createdBy, DateTime? created, string updatedBy, DateTime? updated)
        {
            CreatedBy = createdBy;
            Created = created ?? default;
            UpdatedBy = updatedBy;
            Updated = updated ?? default;
        }
    }
}
