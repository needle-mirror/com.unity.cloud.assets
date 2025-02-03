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
            TransformationProperties m_Properties;

            public TransformationUI(ITransformation transformation)
            {
                m_Transformation = transformation;
                m_CancellationTokenSource = new CancellationTokenSource();

                style.flexDirection = FlexDirection.Row;
                style.alignItems = Align.Center;

                var progressBar = new ProgressBar
                {
                    title = m_Transformation.Descriptor.TransformationId.ToString(),
                    value = 0,
                    style = {flexGrow = 1}
                };
                Add(progressBar);

                var cancelButton = new Button();
                cancelButton.AddToClassList("close-icon");
                cancelButton.RegisterCallback<ClickEvent>(OnCancel);
                Add(cancelButton);

                _ = PopulateAsync();
            }

            async Task PopulateAsync()
            {
                m_Properties = await m_Transformation.GetPropertiesAsync(m_CancellationTokenSource.Token);

                var progressBar = this.Q<ProgressBar>();
                progressBar.title = GetProgressLabel(m_Properties);
                progressBar.value = m_Properties.Status == TransformationStatus.Succeeded ? 100 : 0;
                progressBar.RegisterCallback<ClickEvent>(e =>
                {
                    e.StopPropagation();
                    if (m_Properties.Status is TransformationStatus.Failed or TransformationStatus.Error)
                    {
                        DialogService.ShowMessage("Error", m_Properties.ErrorMessage);
                    }
                });

                if (m_Properties.Status is TransformationStatus.Pending or TransformationStatus.Running)
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
                if (m_Properties.Status is TransformationStatus.Pending or TransformationStatus.Running)
                {
                    m_Transformation.TerminateAsync(CancellationToken.None);
                    return;
                }

                Cancel();

                RemoveFromHierarchy();
            }

            static string GetProgressLabel(TransformationProperties properties)
            {
                var status = properties.Status switch
                {
                    TransformationStatus.Pending => "Pending...",
                    TransformationStatus.Running => "Running",
                    TransformationStatus.Terminating => "Cancelling...",
                    TransformationStatus.Terminated => "Cancelled",
                    TransformationStatus.Succeeded => "Succeeded",
                    TransformationStatus.Failed => "Failed",
                    _ => "Unknown"
                };

                return $"{properties.WorkflowName} - {status}";
            }

            async Task UpdateProgressBarAsync(AbstractProgressBar progressBar, CancellationToken cancellationToken)
            {
                m_Properties = await m_Transformation.GetPropertiesAsync(cancellationToken);

                var status = m_Properties.Status;

                while (!cancellationToken.IsCancellationRequested)
                {
                    await m_Transformation.RefreshAsync(cancellationToken);
                    m_Properties = await m_Transformation.GetPropertiesAsync(cancellationToken);

                    progressBar.value = m_Properties.Progress;

                    if (m_Properties.Status != status)
                    {
                        status = m_Properties.Status;

                        progressBar.title = GetProgressLabel(m_Properties);

                        switch (status)
                        {
                            case TransformationStatus.Pending:
                            case TransformationStatus.Running:
                            case TransformationStatus.Terminating:
                                // Do nothing
                                break;

                            case TransformationStatus.Succeeded:
                                progressBar.value = 100;
                                DialogService.ShowMessage("Sucess", $"Transformation of type {m_Properties.WorkflowName} succeeded.");
                                Cancel();
                                break;
                            case TransformationStatus.Failed:
                            case TransformationStatus.Error:
                                DialogService.ShowMessage("Error", $"Transformation of type {m_Properties.WorkflowName} failed with message: {m_Properties.ErrorMessage}");
                                Cancel();
                                break;
                            case TransformationStatus.Terminated:
                                DialogService.ShowMessage("Cancelled", $"Transformation of type {m_Properties.WorkflowName} was cancelled.");
                                Cancel();
                                break;
                            case TransformationStatus.TimedOut:
                                DialogService.ShowMessage("Timeout", $"Transformation of type {m_Properties.WorkflowName} timed out.");
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
