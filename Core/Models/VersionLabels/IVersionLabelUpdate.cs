using System.Drawing;

namespace Unity.Cloud.Assets
{
    public interface IVersionLabelUpdate
    {
        /// <inheritdoc cref="IVersionLabel.Description"/>
        string Description { get; }

        /// <inheritdoc cref="IVersionLabel.DisplayColor"/>
        Color? DisplayColor { get; }
    }
}
