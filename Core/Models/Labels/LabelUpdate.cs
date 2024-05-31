using System.Drawing;

namespace Unity.Cloud.Assets
{
    public class LabelUpdate : ILabelUpdate
    {
        /// <inheritdoc />
        public string Description { get; set; }

        /// <inheritdoc />
        public Color? DisplayColor { get; set; }
    }
}
