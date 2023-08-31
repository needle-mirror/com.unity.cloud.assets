#if !UC_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Cloud.Assets.Samples
{
    public class ProjectListUi : ListUi<ProjectListController, object>
    {
        const string k_AllProjectId = "All";

        static readonly Pagination k_DefaultPagination = new(nameof(IProject.Name), Range.All);

        public event Action ProjectSelected;

        IProject m_SelectedProject;

        CancellationTokenSource m_ListProjectsCancellationTokenSource = new();

        public IProject SelectedProject
        {
            get => m_SelectedProject;
            private set
            {
                m_SelectedProject = value;
                Debug.Log($"Project Selected: {m_SelectedProject?.Name}");
                ProjectSelected?.Invoke();
            }
        }

        public bool IsAllProjectSelected { get; private set; }

        protected override string VisualElementName => "ProjectsPanel";
        protected override string EmptyListMessage => "No projects available.";

        public async Task Populate(IOrganization organization, bool includeAllProject)
        {
            Show();

            m_ListProjectsCancellationTokenSource.Cancel();
            m_ListProjectsCancellationTokenSource.Dispose();
            m_ListProjectsCancellationTokenSource = new CancellationTokenSource();

            var token = m_ListProjectsCancellationTokenSource.Token;

            var existingEntries = includeAllProject ? new[] {k_AllProjectId} : null;
            await UpdateList(existingEntries, GetProjectsAsync(organization, token), token);
        }

        static IAsyncEnumerable<IProject> GetProjectsAsync(IOrganization organization, CancellationToken token)
        {
            try
            {
                return PlatformServices.ProjectProvider.ListProjectsAsync(organization, k_DefaultPagination, token);
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
            IsAllProjectSelected = selectedProject != null && selectedProject is not IProject;
            SelectedProject = selectedProject as IProject;
        }

        public IEnumerable<IProject> GetProjects()
        {
            return m_Entries.OfType<IProject>();
        }
    }
}
#endif
