using System.Drawing;

namespace Unity.Cloud.Assets
{
    public class VersionLabelCreation : IVersionLabelCreation
    {
        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public string Description { get; set; }

        /// <inheritdoc />
        public Color DisplayColor { get; set; } = Color.White;
    }
}
