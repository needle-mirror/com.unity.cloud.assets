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
                var label = element.Q<Label>();
                label.enableRichText = true;

                _ = PopulateItemAsync(label, m_List[i]);
            }

            static async Task PopulateItemAsync(Label label, IFieldDefinition fieldDefinition)
            {
                var properties = await fieldDefinition.GetPropertiesAsync(CancellationToken.None);

                var isDeleted = properties.IsDeleted;
                var strikethroughTag = isDeleted ? "<s>" : string.Empty;
                var strikethroughEndTag = isDeleted ? "</s>" : string.Empty;
                label.text = $"{strikethroughTag}{properties.DisplayName}{strikethroughEndTag}";
            }
        }

        readonly List<IFieldDefinition> m_FieldDefinitions = new();

        public event Action<IFieldDefinition> FieldDefinitionSelected;

        public IEnumerable<IFieldDefinition> FieldDefinitions => m_ListController.AllItems;

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
            m_ListController.ClearList();

            m_ListController.ApplyFilter(Filter);
            UpdateList(m_FieldDefinitions);

            FieldDefinitionSelected?.Invoke(null);
        }

        static IAsyncEnumerable<IFieldDefinition> GetFieldDefinitionsAsync(IAssetRepository assetRepository, OrganizationId organizationId)
        {
            try
            {
                return assetRepository.ListFieldDefinitionsAsync(organizationId, Range.All, default);
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
            FieldDefinitionSelected?.Invoke(selection as IFieldDefinition);
        }
    }
}
