#if !UC_EXCLUDE_SAMPLES && UNITY_EDITOR
#if !USE_UIELEMENTS
#error Missing dependency to com.unity.modules.uielements
#else

using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetDatabaseUploader.Editor
{
    public class AssetDatabaseUploaderSampleEditorUI : VisualElement
    {
        readonly AssetDatabaseUploaderSample m_AssetDatabaseUploaderSample;

        public bool CanBeRefreshed { get; private set; } = true;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serializedObject"></param>
        public AssetDatabaseUploaderSampleEditorUI(SerializedObject serializedObject)
        {
            m_AssetDatabaseUploaderSample = (AssetDatabaseUploaderSample) serializedObject.targetObject;

            schedule.Execute(() =>
            {
                if (CanBeRefreshed) DrawPlatformInitializationElements();
            }).Every(500);

            DrawUI();
        }

        void DrawUI()
        {
            Add(EditorUIUtils.CreateSpaceBox());

            var intField = new IntegerField("CancellationTokenTimeout (ms)")
            {
                value = m_AssetDatabaseUploaderSample.CancellationTokenTimeout
            };
            intField.RegisterValueChangedCallback((v) =>
            {
                m_AssetDatabaseUploaderSample.CancellationTokenTimeout = v.newValue;
            });
            Add(intField);

            DrawPlatformInitializationElements();
        }

        VisualElement m_PlatformInitStateSpaceElt;
        VisualElement m_PlatformInitStateElt;
        VisualElement m_PlatformInitBtnElt;

        void DrawPlatformInitializationElements()
        {
            if (m_PlatformInitStateSpaceElt != null) Remove(m_PlatformInitStateSpaceElt);

            m_PlatformInitStateSpaceElt = EditorUIUtils.CreateSpaceBox();
            Add(m_PlatformInitStateSpaceElt);

            if (m_PlatformInitStateElt != null) Remove(m_PlatformInitStateElt);

            m_PlatformInitStateElt = new Label($"Platform initialized = {AssetsEditorServices.IsInitialized}");
            Add(m_PlatformInitStateElt);

            if (m_PlatformInitBtnElt == null)
            {
                Add(EditorUIUtils.CreateSpaceBox());

                m_PlatformInitBtnElt = new Button(() => _ = InitializeAction())
                {
                    text = "Initialize Assets Platform Services"
                };
                Add(m_PlatformInitBtnElt);
            }

            m_PlatformInitBtnElt.SetEnabled(!AssetsEditorServices.IsInitialized);
        }

        async Task InitializeAction()
        {
            CanBeRefreshed = false;

            await m_AssetDatabaseUploaderSample.Initialize();

            CanBeRefreshed = true;
        }
    }
}
#endif
#endif
