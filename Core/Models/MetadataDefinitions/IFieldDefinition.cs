using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    public interface IFieldDefinition
    {
        /// <summary>
        /// The descriptor for the field.
        /// </summary>
        FieldDefinitionDescriptor Descriptor { get; }

        /// <summary>
        /// The type of the field.
        /// </summary>
        FieldDefinitionType Type { get; }

        /// <summary>
        /// The status of the field.
        /// </summary>
        string Status { get; }

        /// <summary>
        /// The display name for the field.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// The creation and update information of the field.
        /// </summary>
        AuthoringInfo AuthoringInfo { get; }

        /// <summary>
        /// Refreshes the field to retrieve the latest values.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RefreshAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Syncronizes local changes to the field definition to the data source.
        /// </summary>
        /// <param name="definitionUpdate">The object containing the information to update. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task with no result. </returns>
        Task UpdateAsync(IFieldDefinitionUpdate definitionUpdate, CancellationToken cancellationToken);

    }
}
