#if !UC_EXCLUDE_SAMPLES && UNITY_EDITOR
#if !USE_UIELEMENTS
#error Missing dependency to com.unity.modules.uielements
#else

using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetDatabaseUploader.Editor
{
    /// <summary>
    /// Unity editor for <see cref="AssetsUploader"/>
    /// </summary>
    [CustomEditor(typeof(AssetsUploader))]
    public class AssetsUploaderEditor : UnityEditor.Editor
    {
        VisualElement m_VisualElementRoot;
        AssetsUploaderEditorUI m_ComponentEditorUI;

        /// <inheritdoc />
        public override VisualElement CreateInspectorGUI()
        {
            CreateUI();

            return m_VisualElementRoot;
        }

        void CreateUI()
        {
            m_VisualElementRoot = new VisualElement();

            m_ComponentEditorUI = new AssetsUploaderEditorUI(serializedObject);

            m_VisualElementRoot.Add(m_ComponentEditorUI);
        }
    }
}
#endif
#endif
