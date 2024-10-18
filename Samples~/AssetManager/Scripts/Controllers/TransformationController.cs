using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetManager
{
    public class TransformationController
    {
        class TransformationUI : VisualElement
        {
            readonly ITransformation m_Transformation;
            CancellationTokenSource m_CancellationTokenSource;

            public TransformationUI(ITransformation transformation)
            {
                m_Transformation = transformation;
                m_CancellationTokenSource = new CancellationTokenSource();

                style.flexDirection = FlexDirection.Row;
                style.alignItems = Align.Center;

                var progressBar = new ProgressBar
                {
                    title = GetProgressLabel(),
                    value = transformation.Status == TransformationStatus.Succeeded ? 100 : 0,
                    style = {flexGrow = 1}
                };
                progressBar.RegisterCallback<ClickEvent>(e =>
                {
                    e.StopPropagation();
                    if (transformation.Status is TransformationStatus.Failed or TransformationStatus.Error)
                    {
                        DialogService.ShowMessage("Error", transformation.ErrorMessage);
                    }
                });
                Add(progressBar);

                var cancelButton = new Button();
                cancelButton.AddToClassList("close-icon");
                cancelButton.RegisterCallback<ClickEvent>(OnCancel);
                Add(cancelButton);

                if (transformation.Status is TransformationStatus.Pending or TransformationStatus.Running)
                    _ = UpdateProgressBarAsync(progressBar, m_CancellationTokenSource.Token);
            }

            public void Cancel()
            {
                if (m_CancellationTokenSource != null)
                {
                    m_CancellationTokenSource.Cancel();
                    m_CancellationTokenSource.Dispose();
                    m_CancellationTokenSource = null;
                }
            }

            void OnCancel(ClickEvent _)
            {
                if (m_Transformation.Status is TransformationStatus.Pending or TransformationStatus.Running)
                {
                    m_Transformation.TerminateAsync(CancellationToken.None);
                    return;
                }

                Cancel();

                RemoveFromHierarchy();
            }

            string GetProgressLabel()
            {
                var workflowType = m_Transformation.WorkflowType switch
                {
                    WorkflowType.Data_Streaming => "Streamable",
                    WorkflowType.Thumbnail_Generation => "Thumbnail",
                    WorkflowType.Transcode_Video => "Video",
                    WorkflowType.GLB_Preview => "GLB",
                    WorkflowType.Metadata_Extraction => "Metadata",
                    WorkflowType.Generic_Polygon_Target => "Polygon",
                    WorkflowType.Custom => "Custom",
                    _ => "Unknown"
                };

                var status = m_Transformation.Status switch
                {
                    TransformationStatus.Pending => "Pending...",
                    TransformationStatus.Running => "Running",
                    TransformationStatus.Terminating => "Cancelling...",
                    TransformationStatus.Terminated => "Cancelled",
                    TransformationStatus.Succeeded => "Succeeded",
                    TransformationStatus.Failed => "Failed",
                    _ => "Unknown"
                };

                return $"{workflowType} - {status}";
            }

            async Task UpdateProgressBarAsync(AbstractProgressBar progressBar, CancellationToken cancellationToken)
            {
                var status = m_Transformation.Status;

                while (!cancellationToken.IsCancellationRequested)
                {
                    await m_Transformation.RefreshAsync(cancellationToken);

                    progressBar.value = m_Transformation.Progress;

                    if (m_Transformation.Status != status)
                    {
                        status = m_Transformation.Status;

                        progressBar.title = GetProgressLabel();

                        switch (status)
                        {
                            case TransformationStatus.Pending:
                            case TransformationStatus.Running:
                            case TransformationStatus.Terminating:
                                // Do nothing
                                break;

                            case TransformationStatus.Succeeded:
                                progressBar.value = 100;
                                DialogService.ShowMessage("Sucess", $"Transformation of type {m_Transformation.WorkflowType} succeeded.");
                                Cancel();
                                break;
                            case TransformationStatus.Failed:
                            case TransformationStatus.Error:
                                DialogService.ShowMessage("Error", $"Transformation of type {m_Transformation.WorkflowType} failed with message: {m_Transformation.ErrorMessage}");
                                Cancel();
                                break;
                            case TransformationStatus.Terminated:
                                DialogService.ShowMessage("Cancelled", $"Transformation of type {m_Transformation.WorkflowType} was cancelled.");
                                Cancel();
                                break;
                            case TransformationStatus.TimedOut:
                                DialogService.ShowMessage("Timeout", $"Transformation of type {m_Transformation.WorkflowType} timed out.");
                                Cancel();
                                break;
                            default:
                                Cancel();
                                break;
                        }

                        if (cancellationToken.IsCancellationRequested) break;
                    }

                    await Task.Delay(500, cancellationToken);
                }
            }
        }

        readonly ScrollView m_TransformationContainer;

        public TransformationController(VisualElement datasetPanel)
        {
            m_TransformationContainer = datasetPanel.Q<ScrollView>("TransformationInfo");
        }

        public void Clear()
        {
            foreach (var ui in m_TransformationContainer.Children().Select(x => x as TransformationUI))
            {
                ui?.Cancel();
            }

            m_TransformationContainer.Clear();
        }

        public async Task PopulateTransformationProgress(IDataset dataset)
        {
            var results = dataset.ListTransformationsAsync(Range.All, default);
            await foreach (var transformation in results)
            {
                AddTransformationProgress(transformation);
            }
        }

        public void AddTransformationProgress(ITransformation transformation)
        {
            var ui = new TransformationUI(transformation);
            m_TransformationContainer.Add(ui);
        }
    }
}
