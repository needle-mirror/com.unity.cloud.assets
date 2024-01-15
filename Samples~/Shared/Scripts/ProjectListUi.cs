using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using UnityEngine;

namespace Unity.Cloud.Assets.Samples
{
    public class ProjectListUi : ListUi<ProjectListController, object>
    {
        const string k_AllProjectId = "All";

        static readonly Pagination k_DefaultPagination = new(nameof(IAssetProject.Name), Range.All);

        CancellationTokenSource m_ListProjectsCancellationTokenSource = new();

        public event Action ProjectSelected;

        public IAssetProject SelectedProject { get; private set; }

        public bool IsAllProjectSelected { get; private set; }

        protected override string VisualElementName => "LeftPanel";
        protected override string EmptyListMessage => "No projects available.";

        public async Task Populate(IAssetRepository assetRepository, OrganizationId organizationId, bool includeAllProject)
        {
            Show();

            m_ListProjectsCancellationTokenSource.Cancel();
            m_ListProjectsCancellationTokenSource.Dispose();
            m_ListProjectsCancellationTokenSource = new CancellationTokenSource();

            var token = m_ListProjectsCancellationTokenSource.Token;

            var existingEntries = includeAllProject ? new[] {k_AllProjectId} : null;
            await UpdateList(existingEntries, GetProjectsAsync(assetRepository, organizationId, token), token);
        }

        static IAsyncEnumerable<IAssetProject> GetProjectsAsync(IAssetRepository assetRepository, OrganizationId organizationId, CancellationToken token)
        {
            try
            {
                return assetRepository.ListAssetProjectsAsync(organizationId, k_DefaultPagination, token);
            }
            catch (OperationCanceledException oe)
            {
                oe.LogException();
                return null;
            }
            catch (Exception e)
            {
                e.LogException();
                throw;
            }
        }

        protected override void OnSelectionChange(IEnumerable<object> selectedItems)
        {
            var selectedProject = selectedItems.FirstOrDefault();
            IsAllProjectSelected = selectedProject != null && selectedProject is not IAssetProject;
            SelectedProject = selectedProject as IAssetProject;

            Debug.Log($"Project Selected: {(IsAllProjectSelected ? "All" : SelectedProject?.Name ?? "None")}.");
            ProjectSelected?.Invoke();
        }

        public IEnumerable<IAssetProject> GetProjects()
        {
            return m_Entries.OfType<IAssetProject>();
        }
    }
}
