#if !UC_EXCLUDE_SAMPLES
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public abstract class DialogUi : MonoBehaviour
    {
        [SerializeField]
        VisualTreeAsset m_DialogTemplate;

        [SerializeField]
        StyleSheet m_DialogSheet;

        [HideInInspector]
        [SerializeReference]
        protected IDialogController m_DialogController;

        public IDialogController dialogController => m_DialogController;

        public void Initialize(VisualElement coveredElement, VisualElement parentElement, string title)
        {
            var templateContainer = m_DialogTemplate.Instantiate();
            var dialogPanel = templateContainer.Q<VisualElement>("DialogContainer");
            parentElement.Add(dialogPanel);

            parentElement.styleSheets.Add(m_DialogSheet);

            dialogController.Opened += () =>
            {
                coveredElement.style.display = DisplayStyle.None;
                parentElement.style.display = DisplayStyle.Flex;
            };
            dialogController.Closed += () =>
            {
                coveredElement.style.display = DisplayStyle.Flex;
                parentElement.style.display = DisplayStyle.None;
            };

            m_DialogController.Init(dialogPanel, title);
        }
    }
}
#endif
