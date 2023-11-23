using System.Collections.Generic;
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
        /// The accepted values of the field.
        /// <remarks>This is only required for field definitions of type <see cref="FieldDefinitionType.Selection"/>.</remarks>
        /// </summary>
        IEnumerable<string> AcceptedValues { get; }

        /// <summary>
        /// Whether the field can have multiple values.
        /// <remarks>This is only requred for field definitions of type <see cref="FieldDefinitionType.Selection"/>.</remarks>
        /// </summary>
        bool? Multiselection { get; }

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

        /// <summary>
        /// Appends the parameter list to the accepted values of the field.
        /// </summary>
        /// <param name="acceptedValues">An enumeration of accepted values. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task with no result. </returns>
        Task AddSelectionValuesAsync(IEnumerable<string> acceptedValues, CancellationToken cancellationToken);

        /// <summary>
        /// Removes the parameter list from the accepted values of the field.
        /// </summary>
        /// <param name="acceptedValues">An enumeration of accepted values. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task with no result. </returns>
        Task RemoveSelectionValuesAsync(IEnumerable<string> acceptedValues, CancellationToken cancellationToken);
    }
}
