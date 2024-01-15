using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetManager
{
    public class AddMetadataPopupController : PopupController
    {
        readonly DropdownField m_DropdownField;
        readonly Label m_FieldInfo;

        readonly List<IFieldDefinition> m_FieldDefinitions = new();

        IFieldDefinition m_SelectedFieldDefinition;
        Action<IFieldDefinition> m_OnAdd;

        CancellationTokenSource m_FetchTokenSource;

        public AddMetadataPopupController(VisualElement root)
            : base(root, "AddMetadataPopup", null)
        {
            m_ActionButton.SetEnabled(false);

            m_FieldInfo = root.Q<Label>("FieldInfo");

            m_DropdownField = root.Q<DropdownField>();
            m_DropdownField.RegisterCallback<ChangeEvent<string>>(OnFieldDefinitionSelected);
        }

        public void ListFieldDefinitions(OrganizationId organizationId)
        {
            _ = RefreshAsync(organizationId);
        }

        public void Show(IEnumerable<string> existingKeys, Action<IFieldDefinition> onAdd)
        {
            var hashset = new HashSet<string>(existingKeys);

            m_OnAdd = onAdd;

            m_DropdownField.index = -1;
            m_DropdownField.choices = m_FieldDefinitions.Where(x => !hashset.Contains(x.Descriptor.FieldKey)).Select(x => x.DisplayName).ToList();

            m_FieldInfo.text = string.Empty;

            Show();
        }

        protected override void OnClicked()
        {
            m_OnAdd?.Invoke(m_SelectedFieldDefinition);
            base.OnClicked();
        }

        async Task RefreshAsync(OrganizationId organizationId)
        {
            if (m_FetchTokenSource != null)
            {
                m_FetchTokenSource.Cancel();
                m_FetchTokenSource.Dispose();
            }

            m_FetchTokenSource = new CancellationTokenSource();

            m_FieldDefinitions.Clear();

            var pagination = new Pagination(Range.All);
            var enumerable = PlatformServices.AssetRepository.ListFieldDefinitionsAsync(organizationId, pagination, false, m_FetchTokenSource.Token);
            await foreach (var fieldDefinition in enumerable)
            {
                m_FieldDefinitions.Add(fieldDefinition);
            }
        }

        void OnFieldDefinitionSelected(ChangeEvent<string> _)
        {
            if (m_DropdownField.index < 0)
            {
                m_ActionButton.SetEnabled(false);
                return;
            }

            m_ActionButton.SetEnabled(true);

            var fieldName = m_DropdownField.choices[m_DropdownField.index];
            m_SelectedFieldDefinition = m_FieldDefinitions.First(x => x.DisplayName == fieldName);
            m_FieldInfo.text = $"Key::{m_SelectedFieldDefinition.Descriptor.FieldKey}, {m_SelectedFieldDefinition.Type}";
        }
    }
}
