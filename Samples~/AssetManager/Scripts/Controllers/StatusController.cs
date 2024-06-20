using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetManager
{
    public class StatusController
    {
        readonly VisualElement m_StatusCircle;
        readonly Label m_StatusLabel;
        readonly Label m_LastEditLabel;

        public StatusController(VisualElement root)
        {
            m_StatusCircle = root.Q("StatusCircle");
            m_StatusLabel = root.Q<Label>("StatusNameLabel");
            m_LastEditLabel = root.Q<Label>("LastEditDate");
        }

        public void Update(string status, DateTime? editDate)
        {
            m_StatusLabel.text = string.IsNullOrEmpty(status) ? "Unknown" : status;
            m_LastEditLabel.text = editDate.HasValue ? editDate.Value.ToString("MMM dd, yyyy h:mm tt GMT") : "unknown";
        }

        public void Clear()
        {
            m_StatusLabel.text = string.Empty;
            m_LastEditLabel.text = string.Empty;
        }

        public void SetStatusColor(Color color)
        {
            m_StatusCircle.style.unityBackgroundImageTintColor = color;
        }
    }
}
