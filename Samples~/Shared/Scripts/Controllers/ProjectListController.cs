using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public class ProjectListController : ListController<object>
    {
        protected override void OnBindItem(VisualElement element, int i)
        {
            if (m_List[i] is IAssetProject project)
            {
                _ = PopulateItemAsync(element, project);
            }
            else
            {
                element.Q<Label>().text = m_List[i].ToString();
            }
        }

        static async Task PopulateItemAsync(VisualElement element, IAssetProject project)
        {
            var properties = await project.GetPropertiesAsync(CancellationToken.None);

            element.Q<Label>().text = properties.Name;
        }
    }
}
