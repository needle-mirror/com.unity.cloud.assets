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

        public static void Show(this VisualElement element, bool show)
        {
            element.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
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

        public static void AddTag(this VisualElement container, string tag, ICollection<string> tags, VisualTreeAsset template, bool canRemove)
        {
            var chip = template.Instantiate();
            chip.Q<Label>().text = tag;
            container.Add(chip);

            var removeButton = chip.Q<Button>();
            removeButton.Show(canRemove);
            if (canRemove)
            {
                removeButton.clicked += () =>
                {
                    tags?.Remove(tag);
                    chip.RemoveFromHierarchy();
                };
            }
        }

        public static string GetVersionText(this IAsset asset, bool includeParentVersion = false)
        {
            var versionText = asset.State switch
            {
                AssetState.Frozen => $"Ver. {asset.FrozenSequenceNumber}",
                _ => includeParentVersion ? $"Pending from Ver. {asset.ParentFrozenSequenceNumber}" : "Pending"
            };

            var version = asset.Descriptor.AssetVersion.ToString();
            if (version.Length > 8)
            {
                version = version[..8];
            }

            return $"{versionText}\n<color=#888888>{version}</color>";
        }
    }
}
