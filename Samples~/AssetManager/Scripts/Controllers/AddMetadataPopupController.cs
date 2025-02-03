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
        readonly Dictionary<string, FieldDefinitionProperties> m_FieldDefinitionProperties = new();
        bool m_IsPopulated;

        string m_SelectedFieldDefinition;
        Action<IFieldDefinition, FieldDefinitionProperties> m_OnAdd;

        CancellationTokenSource m_FetchTokenSource;

        public AddMetadataPopupController(VisualElement root)
            : base(root, "AddMetadataPopup")
        {
            m_ActionButton.SetEnabled(false);

            m_FieldInfo = m_PopupWindow.Q<Label>("FieldInfo");

            m_DropdownField = m_PopupWindow.Q<DropdownField>();
            m_DropdownField.RegisterCallback<ChangeEvent<string>>(OnFieldDefinitionSelected);
        }

        public void ListFieldDefinitions(OrganizationId organizationId)
        {
            _ = RefreshAsync(organizationId);
        }

        public async Task<IFieldDefinition> GetFieldDefinitionAsync(string key)
        {
            while (!m_IsPopulated)
            {
                await Task.Yield();
            }

            return m_FieldDefinitions.FirstOrDefault(x => x.Descriptor.FieldKey == key);
        }

        public async Task<string> GetFieldDefinitionNameAsync(string key)
        {
            while (!m_IsPopulated)
            {
                await Task.Yield();
            }

            return m_FieldDefinitionProperties.GetValueOrDefault(key).DisplayName;
        }

        public void Show(IEnumerable<string> existingKeys, Action<IFieldDefinition, FieldDefinitionProperties> onAdd)
        {
            var hashset = new HashSet<string>(existingKeys);

            m_OnAdd = onAdd;

            m_DropdownField.index = -1;
            m_DropdownField.choices = m_FieldDefinitionProperties
                .Where(x => !hashset.Contains(x.Key))
                .Select(x => x.Value.DisplayName)
                .ToList();

            m_FieldInfo.text = string.Empty;

            Show();
        }

        protected override void OnClicked()
        {
            m_OnAdd?.Invoke(m_FieldDefinitions.FirstOrDefault(x => x.Descriptor.FieldKey == m_SelectedFieldDefinition),
                m_FieldDefinitionProperties.GetValueOrDefault(m_SelectedFieldDefinition));
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

            m_IsPopulated = false;

            m_FieldDefinitions.Clear();
            m_FieldDefinitionProperties.Clear();

            var searchFilter = new FieldDefinitionSearchFilter();
            searchFilter.Deleted.WhereEquals(false);

            var enumerable = PlatformServices.AssetRepository.QueryFieldDefinitions(organizationId)
                .SelectWhereMatchesFilter(searchFilter)
                .ExecuteAsync(m_FetchTokenSource.Token);
            await foreach (var fieldDefinition in enumerable)
            {
                m_FieldDefinitions.Add(fieldDefinition);

                var properties = await fieldDefinition.GetPropertiesAsync(m_FetchTokenSource.Token);
                m_FieldDefinitionProperties.Add(fieldDefinition.Descriptor.FieldKey, properties);
            }

            m_IsPopulated = true;
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
            var kvp = m_FieldDefinitionProperties.First(x => x.Value.DisplayName == fieldName);
            m_SelectedFieldDefinition = kvp.Key;
            m_FieldInfo.text = $"Key::{kvp.Key}, {kvp.Value.Type}";
        }
    }
}
