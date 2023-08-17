#if !UC_EXCLUDE_SAMPLES
using System;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.CollectionManagement
{
    public class CollectionCreationController
    {
        public event Action<string> CollectionCreated;

        TextField m_InputField;
        Button m_CreateButton;

        public void Initialize(VisualElement uiDocumentRoot)
        {
            var creationContainer = uiDocumentRoot.Q<VisualElement>("CreateCollectionContainer");

            m_InputField = creationContainer.Q<TextField>("Input");
            m_InputField.RegisterCallback<InputEvent>(OnInputChanged);

            m_CreateButton = creationContainer.Q<Button>("Button");
            m_CreateButton.clicked += OnCreateClicked;

            m_CreateButton.SetEnabled(false);
        }

        public void Cleanup()
        {
            m_InputField.UnregisterCallback<InputEvent>(OnInputChanged);
            m_CreateButton.clicked -= OnCreateClicked;
        }

        void OnInputChanged(InputEvent evt)
        {
            var emptyString = string.IsNullOrWhiteSpace(evt.newData);
            m_CreateButton.SetEnabled(!emptyString);
        }

        void OnCreateClicked()
        {
            CollectionCreated?.Invoke(m_InputField.value);
        }
    }
}
#endif
