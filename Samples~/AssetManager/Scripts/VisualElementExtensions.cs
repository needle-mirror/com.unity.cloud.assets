using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public static class VisualElementExtensions
    {
        public static void Show(this VisualElement element)
        {
            element.style.display = DisplayStyle.Flex;
        }

        public static void Hide(this VisualElement element)
        {
            element.style.display = DisplayStyle.None;
        }

        public static void ParseTags(this TextField textField, ICollection<string> tags, Action<string> addTag)
        {
            // If focused on tags text field and press enter, call "add new tag" (if not empty)
            if (Input.GetKey(KeyCode.Return) && !string.IsNullOrEmpty(textField.value))
            {
                var splitList = textField.value.Split(',');
                foreach (var tag in splitList)
                {
                    var trimmedTag = tag.Trim();
                    tags.Add(trimmedTag);
                    addTag(trimmedTag);
                }

                // clear the text field
                textField.SetValueWithoutNotify("");
            }
        }

        public static void AddTags(this Action<string> addTag, IEnumerable<string> tagsList)
        {
            var tags = tagsList?.ToList();

            if (tags == null) return;

            foreach (var tag in tags)
            {
                addTag(tag);
            }
        }

        public static void AddTag(this VisualElement container, string tag, ICollection<string> tags, VisualTreeAsset template)
        {
            var chip = template.Instantiate();
            chip.Q<Label>().text = tag;
            container.Add(chip);
            chip.Q<Button>().clicked += () =>
            {
                tags.Remove(tag);
                chip.RemoveFromHierarchy();
            };
        }
    }
}
