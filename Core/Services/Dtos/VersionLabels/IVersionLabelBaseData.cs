using System.Drawing;
using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    interface IVersionLabelBaseData
    {
        [DataMember(Name = "name")]
        string Name { get; }

        [DataMember(Name = "description")]
        string Description { get; }

        Color? DisplayColor { get; }
    }
}
