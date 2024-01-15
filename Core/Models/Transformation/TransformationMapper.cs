using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    static partial class EntityMapper
    {
        internal static void MapFrom(this TransformationEntity transformation, ITransformationData data)
        {
            transformation.OutputDatasetId = data.OutputDatasetId;
            transformation.LinkDatasetId = data.LinkDatasetId;
            transformation.InputFiles = data.InputFiles;
            transformation.WorkflowType = data.WorkflowType;
            transformation.Status = data.Status;
            transformation.ErrorMessage = data.ErrorMessage;
            transformation.CreatedOn = data.CreatedOn;
            transformation.UpdatedAt = data.UpdatedAt;
            transformation.StartedAt = data.StartedAt;
        }
    }
}
