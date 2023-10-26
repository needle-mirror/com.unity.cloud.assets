#if !UC_EXCLUDE_SAMPLES && UNITY_EDITOR
#if !USE_UIELEMENTS
#error Missing dependency to com.unity.modules.uielements
#else

using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetDatabaseUploader.Editor
{
    /// <summary>
    /// Unity editor for <see cref="AssetDatabaseUploaderSample"/>
    /// </summary>
    [CustomEditor(typeof(AssetDatabaseUploaderSample))]
    public class AssetDatabaseUploaderSampleEditor : UnityEditor.Editor
    {
        VisualElement m_VisualElementRoot;

        /// <inheritdoc />
        public override VisualElement CreateInspectorGUI()
        {
            CreateUI();

            return m_VisualElementRoot;
        }

        void CreateUI()
        {
            m_VisualElementRoot = new VisualElement();

            CreateSampleUI();
        }

        void CreateSampleUI()
        {
            var editorUI = new AssetDatabaseUploaderSampleEditorUI(serializedObject);
            m_VisualElementRoot.Add(editorUI);
        }
    }
}
#endif
#endif
