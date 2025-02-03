using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Assets;
using UnityEngine;

namespace Unity.Cloud.Documentation.Assets
{
    public class UseCaseStartTransformationExampleBehaviour
    {
        #region Example_Behaviour_StartTransformation

        public async Task StartTransformationOnDataset(IDataset dataset, WorkflowType workflowType)
        {
            try
            {
                var creation = new TransformationCreation
                {
                    WorkflowType = workflowType
                };

                var transformationDescriptor = await dataset.StartTransformationLiteAsync(creation, CancellationToken.None);
                Debug.Log($"Transformation started: {transformationDescriptor.TransformationId}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to start transformation. {e}");
                throw;
            }
        }

        public async Task StartCustomTransformationOnDataset(IDataset dataset, string workflowName)
        {
            try
            {
                var creation = new TransformationCreation
                {
                    WorkflowType = WorkflowType.Custom,
                    CustomWorkflowName = workflowName
                };

                var transformationDescriptor = await dataset.StartTransformationLiteAsync(creation, CancellationToken.None);
                Debug.Log($"Transformation started: {transformationDescriptor.TransformationId}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to start transformation. {e}");
                throw;
            }
        }

        #endregion

        #region Example_Behaviour_GetTransformation

        public async Task StartTransformationOnDataset(IDataset dataset, TransformationId transformationId)
        {
            try
            {
                var transformation = await dataset.GetTransformationAsync(transformationId, CancellationToken.None);
                var transformationProperties = await transformation.GetPropertiesAsync(CancellationToken.None);
                Debug.Log($"Transformation {transformation.Descriptor.TransformationId} current status is {transformationProperties.Status}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to get transformation. {e}");
                throw;
            }
        }


        #endregion
    }
}
