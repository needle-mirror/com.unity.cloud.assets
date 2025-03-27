using System.Threading;

namespace Unity.Cloud.Documentation.Assets
{
#pragma warning disable S4487 // Unread "private" fields should be removed
#pragma warning disable S1186 // Methods should not be empty

    #region Example_UIClass

    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;
    using Unity.Cloud.Assets;
    using Unity.Cloud.Common;

    public class UseCaseManageTransformationsExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public UseCaseManageTransformationsExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseManageTransformationsExample : IAssetManagementUI
    {
        readonly UseCaseManageTransformationsExampleBehaviour m_Behaviour;

        public UseCaseManageTransformationsExample(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = new UseCaseManageTransformationsExampleBehaviour(behaviour);
        }

        #region Example_UIContent

        IAssetProject m_SelectedProject;
        Vector2 m_ScrollPosition;

        public void OnGUI()
        {
            if (!m_Behaviour.IsProjectSelected)
            {
                _ = m_Behaviour.SelectTransformation(null);
                return;
            }

            if (m_SelectedProject != m_Behaviour.CurrentProject)
            {
                m_SelectedProject = m_Behaviour.CurrentProject;
                _ = m_Behaviour.ListTransformationsAsync();
            }

            GUILayout.BeginVertical();

            if (GUILayout.Button("Refresh"))
            {
                _ = m_Behaviour.ListTransformationsAsync();
            }

            GUILayout.Space(15);

            GUILayout.Label($"{m_Behaviour.Transformations.Count}:");
            m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);

            foreach (var transformation in m_Behaviour.Transformations)
            {
                if (!m_Behaviour.TransformationNames.TryGetValue(transformation.Descriptor.TransformationId, out var name))
                {
                    name = transformation.Descriptor.TransformationId.ToString();
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label(name);

                GUI.enabled = transformation != m_Behaviour.CurrentTransformation;

                if (GUILayout.Button("Select"))
                {
                    _ = m_Behaviour.SelectTransformation(transformation);
                }

                GUI.enabled = true;

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            GUILayout.EndVertical();

            if (m_Behaviour.CurrentTransformation == null)
            {
                GUILayout.Label("No transformation selected");
            }
            else if (m_Behaviour.CurrentTransformationProperties == null)
            {
                GUILayout.Label("Loading...");
            }
            else
            {
                DisplayTransformation(m_Behaviour.CurrentTransformation.Descriptor.TransformationId, m_Behaviour.CurrentTransformationProperties.Value);
            }
        }

        void DisplayTransformation(TransformationId transformationId, TransformationProperties properties)
        {
            GUILayout.BeginVertical();

            GUILayout.Label(m_Behaviour.TransformationOwner);
            GUILayout.Label(transformationId.ToString());
            GUILayout.Label(properties.WorkflowName);
            GUILayout.Label(properties.Status.ToString());
            if (!string.IsNullOrEmpty(properties.ErrorMessage))
                GUILayout.Label(properties.ErrorMessage);
            GUILayout.Label("Created: " + properties.Created);
            GUILayout.Label("Updated: " + properties.Updated);
            GUILayout.Label("Started: " + properties.Started);

            GUI.enabled = properties.Status is TransformationStatus.Queued or TransformationStatus.Pending or TransformationStatus.Running;

            if (GUILayout.Button("Cancel"))
            {
                _ = m_Behaviour.CurrentTransformation.TerminateAsync(CancellationToken.None);
            }

            GUI.enabled = true;

            GUILayout.EndVertical();
        }

        #endregion
    }

    class UseCaseManageTransformationsExampleBehaviour
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public bool IsProjectSelected => m_Behaviour.IsProjectSelected;
        public IAssetProject CurrentProject => m_Behaviour.CurrentProject;

        public UseCaseManageTransformationsExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_Behaviour_ListTransformations

        public List<ITransformation> Transformations { get; } = new();
        public Dictionary<TransformationId, string> TransformationNames { get; } = new();
        public ITransformation CurrentTransformation { get; private set; }
        public TransformationProperties? CurrentTransformationProperties { get; private set; }
        public string TransformationOwner { get; private set; }

        public async Task ListTransformationsAsync()
        {
            var selectedTransformation = CurrentTransformation;
            CurrentTransformation = null;
            CurrentTransformationProperties = null;
            Transformations.Clear();
            TransformationNames.Clear();

            try
            {
                var filter = new TransformationSearchFilter();
                // filter.Status.WhereEquals(TransformationStatus.Queued);

                var query = CurrentProject.QueryTransformations()
                    .SelectWhereMatchesFilter(filter)
                    .ExecuteAsync(default);

                await foreach (var result in query)
                {
                    Transformations.Add(result);

                    var properties = await result.GetPropertiesAsync(CancellationToken.None);

                    TransformationNames.Add(result.Descriptor.TransformationId, $"{properties.WorkflowType} - {properties.Status}");

                    if (selectedTransformation != null && selectedTransformation.Descriptor == result.Descriptor)
                    {
                        CurrentTransformation = selectedTransformation;
                        CurrentTransformationProperties = properties;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public async Task SelectTransformation(ITransformation transformation)
        {
            CurrentTransformation = transformation;
            CurrentTransformationProperties = null;
            TransformationOwner = transformation?.Descriptor.AssetId.ToString();
            if (CurrentTransformation != null)
            {
                CurrentTransformationProperties = await CurrentTransformation.GetPropertiesAsync(CancellationToken.None);

                try
                {
                    var asset = await CurrentProject.GetAssetAsync(CurrentTransformation.Descriptor.AssetId, CurrentTransformation.Descriptor.AssetVersion, CancellationToken.None);
                    var assetProperties = await asset.GetPropertiesAsync(CancellationToken.None);
                    TransformationOwner = $"{assetProperties.Name} ({asset.Descriptor.AssetId})";
                }
                catch (NotFoundException)
                {
                    TransformationOwner = "Invalid";
                }
            }
        }

        #endregion
    }
}
