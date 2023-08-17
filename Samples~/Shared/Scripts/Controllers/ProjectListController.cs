#if !UC_EXCLUDE_SAMPLES
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public class ProjectListController : ListController<IProject>
    {
        protected override void OnBindItem(VisualElement element, int i)
        {
            element.Q<Label>("ItemNameLabel").text = m_List[i].Name;
        }
    }
}
#endif
