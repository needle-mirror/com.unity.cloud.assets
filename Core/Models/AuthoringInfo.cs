using System;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    public class AuthoringInfo
    {
        /// <summary>
        /// The id of the user who created.
        /// </summary>
        public UserId CreatedBy { get; }

        /// <summary>
        /// The date and time of creation.
        /// </summary>
        public DateTime Created { get; }

        /// <summary>
        /// The id of the user who updated.
        /// </summary>
        public UserId UpdatedBy { get; }

        /// <summary>
        /// The date and time of update.
        /// </summary>
        public DateTime Updated { get; }

        internal AuthoringInfo(string createdBy, DateTime? created, string updatedBy, DateTime? updated)
        {
            CreatedBy = new UserId(createdBy);
            Created = created ?? default;
            UpdatedBy = new UserId(updatedBy);
            Updated = updated ?? default;
        }
    }
}
