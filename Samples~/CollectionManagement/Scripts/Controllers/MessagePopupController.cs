using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.CollectionManagement
{
    public class MessagePopupController : PopupController
    {
        readonly Label m_Title;
        readonly Label m_Message;

        public MessagePopupController(VisualElement root)
            : base(root, "MessagePopup", null)
        {
            m_Title = m_PopupWindow.Q<Label>("Title");
            m_Message = m_PopupWindow.Q<Label>("Message");
        }

        public void ShowMessage(string title, string message)
        {
            m_Title.text = title;
            m_Message.text = message;
            Show();
        }
    }
}
