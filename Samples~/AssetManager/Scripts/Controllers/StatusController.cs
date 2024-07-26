using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetManager
{
    public class StatusController
    {
        const string k_StatusUnknown = "Unknown";

        readonly VisualElement m_StatusCircle;
        readonly Label m_StatusLabel;
        readonly Label m_LastEditLabel;

        public StatusController(VisualElement root)
        {
            m_StatusCircle = root.Q("StatusCircle");
            m_StatusLabel = root.Q<Label>("StatusNameLabel");
            m_LastEditLabel = root.Q<Label>("LastEditDate");

            Clear();
        }

        public void Update(string status, DateTime? editDate)
        {
            status = string.IsNullOrEmpty(status) ? k_StatusUnknown : status;

            m_StatusLabel.text = status;
            m_LastEditLabel.text = editDate.HasValue ? editDate.Value.ToString("MMM dd, yyyy h:mm tt GMT") : "unknown";

            if (status == k_StatusUnknown)
            {
                SetStatusColor(Color.gray);
            }
        }

        public void Clear()
        {
            m_StatusLabel.text = k_StatusUnknown;
            m_LastEditLabel.text = string.Empty;
        }

        public void SetStatusColor(Color color)
        {
            m_StatusCircle.style.unityBackgroundImageTintColor = color;
        }
    }
}
