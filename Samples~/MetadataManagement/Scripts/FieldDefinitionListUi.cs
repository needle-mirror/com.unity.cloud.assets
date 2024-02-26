using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.MetadataManagement
{
    public class FieldDefinitionListUi : ListUi<FieldDefinitionListUi.FieldDefinitionListController, IFieldDefinition>
    {
        public class FieldDefinitionListController : ListController<IFieldDefinition>
        {
            protected override void OnBindItem(VisualElement element, int i)
            {
                var label = element.Q<Label>("ItemNameLabel");
                label.enableRichText = true;

                var fieldDefinition = m_List[i];
                var isDeleted = fieldDefinition.IsDeleted;
                var strikethroughTag = isDeleted ? "<s>" : string.Empty;
                var strikethroughEndTag = isDeleted ? "</s>" : string.Empty;
                label.text = $"{strikethroughTag}{fieldDefinition.DisplayName}{strikethroughEndTag}";
            }
        }

        readonly List<IFieldDefinition> m_FieldDefinitions = new();
        IFieldDefinition m_SelectedFieldDefinition;

        public event Action FieldDefinitionSelected;

        public IFieldDefinition SelectedFieldDefinition
        {
            get => m_SelectedFieldDefinition;
            private set
            {
                m_SelectedFieldDefinition = value;
                Debug.Log($"Field definition selected: {m_SelectedFieldDefinition?.DisplayName}");
                FieldDefinitionSelected?.Invoke();
            }
        }

        public IEnumerable<IFieldDefinition> FieldDefinitions => m_Entries;

        public Func<IFieldDefinition, bool> Filter { get; set; }

        protected override string VisualElementName => "LeftPanel";
        protected override string EmptyListMessage => "No field definitions defined.";

        public async Task Populate(IAssetRepository assetRepository, OrganizationId organizationId)
        {
            Show();

            var fieldsList = GetFieldDefinitionsAsync(assetRepository, organizationId);
            m_FieldDefinitions.Clear();
            await foreach (var field in fieldsList)
            {
                m_FieldDefinitions.Add(field);
            }

            Populate();
        }

        public void Populate()
        {
            var filteredList = (Filter == null ? m_FieldDefinitions : m_FieldDefinitions.Where(Filter)).ToArray();
            m_ListController.ClearList();
            UpdateList(filteredList);

            if (m_SelectedFieldDefinition != null && filteredList.All(x => x.Descriptor.FieldKey != m_SelectedFieldDefinition.Descriptor.FieldKey))
            {
                SelectedFieldDefinition = null;
            }
        }

        static IAsyncEnumerable<IFieldDefinition> GetFieldDefinitionsAsync(IAssetRepository assetRepository, OrganizationId organizationId)
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                return assetRepository.ListFieldDefinitionsAsync(organizationId, Range.All, cancellationTokenSource.Token);
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
            var selection = selectedItems.FirstOrDefault();
            SelectedFieldDefinition = selection as IFieldDefinition;
        }
    }
}
