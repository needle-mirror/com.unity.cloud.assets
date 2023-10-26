#if !UC_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Identity;
using UnityEngine;

namespace Unity.Cloud.Assets.Samples
{
    public class ProjectListUi : ListUi<ProjectListController, object>
    {
        const string k_AllProjectId = "All";

        static readonly Pagination k_DefaultPagination = new(nameof(IAssetProject.Name), Range.All);

        public event Action ProjectSelected;

        CancellationTokenSource m_ListProjectsCancellationTokenSource = new();

        public IAssetProject SelectedProject { get; private set; }

        public bool IsAllProjectSelected { get; private set; }

        protected override string VisualElementName => "ProjectsPanel";
        protected override string EmptyListMessage => "No projects available.";

        public async Task Populate(IAssetRepository assetRepository, IOrganization organization, bool includeAllProject)
        {
            Show();

            m_ListProjectsCancellationTokenSource.Cancel();
            m_ListProjectsCancellationTokenSource.Dispose();
            m_ListProjectsCancellationTokenSource = new CancellationTokenSource();

            var token = m_ListProjectsCancellationTokenSource.Token;

            var existingEntries = includeAllProject ? new[] {k_AllProjectId} : null;
            await UpdateList(existingEntries, GetProjectsAsync(assetRepository, organization, token), token);
        }

        static IAsyncEnumerable<IAssetProject> GetProjectsAsync(IAssetRepository assetRepository, IOrganization organization, CancellationToken token)
        {
            try
            {
                return assetRepository.ListAssetProjectsAsync(organization.Id, k_DefaultPagination, token);
            }
            catch (OperationCanceledException oe)
            {
                Debug.LogException(oe);
                throw;
            }
            catch (AggregateException e)
            {
                Debug.LogException(e.InnerException);
                throw;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        protected override void OnSelectionChange(IEnumerable<object> selectedItems)
        {
            var selectedProject = selectedItems.FirstOrDefault();
            IsAllProjectSelected = selectedProject != null && selectedProject is not IAssetProject;
            SelectedProject = selectedProject as IAssetProject;

            Debug.Log($"Project Selected: {SelectedProject?.Name}");
            ProjectSelected?.Invoke();
        }

        public IEnumerable<IAssetProject> GetProjects()
        {
            return m_Entries.OfType<IAssetProject>();
        }
    }
}
#endif
