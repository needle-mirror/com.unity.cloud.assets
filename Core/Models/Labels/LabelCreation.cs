using System.Drawing;

namespace Unity.Cloud.Assets
{
    public class LabelCreation : ILabelCreation
    {
        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public string Description { get; set; }

        /// <inheritdoc />
        public Color DisplayColor { get; set; } = Color.White;
    }
}
