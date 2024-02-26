using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetManager
{
    public class TransformationController
    {
        class TransformationUI
        {
            readonly ITransformation m_Transformation;
            readonly VisualElement m_UI;
            readonly Action<TransformationUI> m_OnCancel;
            CancellationTokenSource m_CancellationTokenSource;

            public TransformationUI(ITransformation transformation, VisualElement ui, Action<TransformationUI> onCancel = null)
            {
                m_Transformation = transformation;
                m_CancellationTokenSource = new CancellationTokenSource();
                m_UI = ui;
                m_OnCancel = onCancel;

                var progressBar = ui.Q<ProgressBar>();
                progressBar.title = GetProgressLabel();
                progressBar.value = transformation.Status == TransformationStatus.Succeeded ? 100 : 0;

                var cancelButton = ui.Q<Button>();
                cancelButton.RegisterCallback<ClickEvent>(OnCancel);

                if (transformation.Status is TransformationStatus.Pending or TransformationStatus.Running)
                    _ = UpdateProgressBarAsync(ui.Q<ProgressBar>(), m_CancellationTokenSource.Token);
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
                Cancel();

                m_UI.RemoveFromHierarchy();
                m_OnCancel?.Invoke(this);
            }

            string GetProgressLabel()
            {
                var workflowType = m_Transformation.WorkflowType switch
                {
                    WorkflowType.Data_Streaming => "Data Streaming",
                    WorkflowType.Thumbnail_Generation => "Thumbnail",
                    WorkflowType.Transcode_Video => "Video",
                    WorkflowType.GLB_Preview => "GLB",
                    _ => "Unknown"
                };

                var status = m_Transformation.Status switch
                {
                    TransformationStatus.Pending => "Pending...",
                    TransformationStatus.Running => "Running",
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

        readonly VisualTreeAsset m_TransformationTemplate;
        readonly ScrollView m_TransformationContainer;

        readonly List<TransformationUI> m_TransformationUis = new();

        public TransformationController(VisualElement datasetPanel)
        {
            var transformationTemplate = datasetPanel.Q<TemplateContainer>("TransformationProgressBar");
            m_TransformationTemplate = transformationTemplate.templateSource;

            m_TransformationContainer = datasetPanel.Q<ScrollView>("TransformationInfo");
        }

        public void Clear()
        {
            m_TransformationContainer.Clear();

            foreach (var ui in m_TransformationUis)
            {
                ui.Cancel();
            }
            m_TransformationUis.Clear();
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
            var ui = m_TransformationTemplate.Instantiate();
            var progressUi = new TransformationUI(transformation, ui, OnCancel);

            m_TransformationContainer.Add(ui);
            m_TransformationUis.Add(progressUi);
        }

        void OnCancel(TransformationUI ui)
        {
            m_TransformationUis.Remove(ui);
        }
    }
}
