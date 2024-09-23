using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetManager
{
    public class RowItem : VisualElement
    {
        public RowItem(string elementName = default)
        {
            name = elementName;
            AddToClassList("row-item");
        }

        public Label AddLabel(string text, string labelName = default)
        {
            var label = new Label
            {
                name = labelName,
                text = text
            };
            label.AddToClassList("row-item__flex");
            Add(label);
            return label;
        }

        public Label AddLabel(string text, float width, string labelName = default)
        {
            var label = new Label
            {
                name = labelName,
                text = text,
                style = { width = width }
            };
            label.AddToClassList("row-item__fixed");
            Add(label);
            return label;
        }
    }
}
