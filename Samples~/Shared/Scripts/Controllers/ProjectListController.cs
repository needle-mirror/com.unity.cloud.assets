using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public class ProjectListController : ListController<object>
    {
        protected override void OnBindItem(VisualElement element, int i)
        {
            string label;
            if (m_List[i] is IAssetProject project)
            {
                label = project.Name;
            }
            else
            {
                label = m_List[i].ToString();
            }

            element.Q<Label>().text = label;
        }
    }
}
