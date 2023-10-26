#if !UC_EXCLUDE_SAMPLES
using System;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public class MessageDialogController : DialogController, IDialogController<string, bool>
    {
        Label m_MessageLabel;
        TaskCompletionSource<bool> m_TaskSource;

        public override void Init(VisualElement dialogPanel, string title)
        {
            base.Init(dialogPanel, title);

            m_MessageLabel = m_DialogPanel.Q<Label>("Message");
        }

        public void OpenDialog(string content)
        {
            m_MessageLabel.text = content;

            OkClicked += CloseMessage;

            ShowDialog();
        }

        public Task<bool> OpenDialogAsync(string content)
        {
            m_TaskSource = new TaskCompletionSource<bool>();

            m_MessageLabel.text = content;
            
            OkClicked += CloseMessageWithResult;

            ShowDialog();

            return m_TaskSource.Task;
        }

        void CloseMessageWithResult()
        {
            OkClicked -= CloseMessageWithResult;

            CloseDialog();

            m_TaskSource.SetResult(true);
        }

        void CloseMessage()
        {
            OkClicked -= CloseMessage;

            CloseDialog();
        }
    }
}
#endif
