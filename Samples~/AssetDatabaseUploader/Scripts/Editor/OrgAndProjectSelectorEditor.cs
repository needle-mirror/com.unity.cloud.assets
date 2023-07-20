#if !UC_EXCLUDE_SAMPLES && UNITY_EDITOR
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
        OrgAndProjectSelectorEditorUI m_ComponentEditorUI;


        /// <inheritdoc />
        public override VisualElement CreateInspectorGUI()
        {
            CreateUI();

            return m_VisualElementRoot;
        }

        void CreateUI()
        {
            m_VisualElementRoot = new VisualElement();
            //m_VisualElementRoot.styleSheets.Add(UIResources.EditorStyleSheet);

            m_ComponentEditorUI = new OrgAndProjectSelectorEditorUI(serializedObject);

            m_VisualElementRoot.Add(m_ComponentEditorUI);
        }
    }
}
#endif
#endif
