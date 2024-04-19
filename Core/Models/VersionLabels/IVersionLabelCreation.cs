using System.Drawing;

namespace Unity.Cloud.Assets
{
    public interface IVersionLabelCreation
    {
        /// <inheritdoc cref="IVersionLabel.Name"/>
        string Name { get; }

        /// <inheritdoc cref="IVersionLabel.Description"/>
        string Description { get; }

        /// <inheritdoc cref="IVersionLabel.DisplayColor"/>
        Color DisplayColor { get; }
    }
}
