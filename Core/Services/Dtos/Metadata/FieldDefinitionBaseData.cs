using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    class FieldDefinitionBaseData : IFieldDefinitionBaseData
    {
        /// <inheritdoc />
        public string DisplayName { get; set; }

        /// <inheritdoc />
        public string[] AcceptedValues { get; set; }
    }
}
