#if UNITY_EDITOR
#if !USE_UIELEMENTS
#error Missing dependency to com.unity.modules.uielements
#else

using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetDatabaseUploader.Editor
{
    /// <summary>
    /// Unity editor for <see cref="OrgAndProjectSelector"/>
    /// </summary>
    [CustomEditor(typeof(OrgAndProjectSelector))]
    public class OrgAndProjectSelectorEditor : UnityEditor.Editor
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

            var editorUI = new OrgAndProjectSelectorEditorUI(serializedObject);
            m_VisualElementRoot.Add(editorUI);
        }
    }
}
#endif
#endif
