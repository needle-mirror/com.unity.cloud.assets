#if UNITY_EDITOR
#if !USE_UIELEMENTS
#error Missing dependency to com.unity.modules.uielements
#else

using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetDatabaseUploader.Editor
{
    static class EditorUIUtils
    {
        internal static VisualElement CreateSpaceBox()
        {
            return new VisualElement
            {
                style =
                {
                    minHeight = 10
                }
            };
        }
    }
}
#endif
#endif
