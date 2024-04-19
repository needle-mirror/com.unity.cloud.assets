using System.Drawing;

namespace Unity.Cloud.Assets
{
    public class VersionLabelUpdate : IVersionLabelUpdate
    {
        /// <inheritdoc />
        public string Description { get; set; }

        /// <inheritdoc />
        public Color? DisplayColor { get; set; }
    }
}
