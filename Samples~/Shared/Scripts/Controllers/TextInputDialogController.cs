#if !UC_EXCLUDE_SAMPLES
using System;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public class TextInputDialogController : DialogController, IDialogController<(string label, string text), IDialogResult<string>>
    {
        TextField m_TextField;
        Button m_CancelButton;

        TaskCompletionSource<IDialogResult<string>> m_TaskSource;

        public override void Init(VisualElement dialogPanel, string title)
        {
            base.Init(dialogPanel, title);

            m_TextField = m_DialogPanel.Q<TextField>("InputText");
            m_CancelButton = m_DialogPanel.Q<Button>("CancelButton");
        }

        public void OpenDialog((string label, string text) content)
        {
            m_TextField.label = content.label;
            m_TextField.value = content.text;

            OkClicked += CloseTextInput;
            m_CancelButton.clicked += CloseTextInput;

            ShowDialog();
        }

        public Task<IDialogResult<string>> OpenDialogAsync((string label, string text) content)
        {
            m_TaskSource = new TaskCompletionSource<IDialogResult<string>>();

            m_TextField.label = content.label;
            m_TextField.value = content.text;

            OkClicked += ConfirmTextInputResult;
            m_CancelButton.clicked += CancelTextInputResult;

            return m_TaskSource.Task;
        }

        Action ConfirmTextInputResult => () => CloseTextInputWithResult(true);

        Action CancelTextInputResult => () => CloseTextInputWithResult(false);

        void CloseTextInputWithResult(bool confirmed)
        {
            OkClicked -= ConfirmTextInputResult;
            m_CancelButton.clicked -= CancelTextInputResult;

            CloseDialog();

            m_TaskSource?.SetResult(new Result<string>(confirmed, m_TextField.value));
        }

        void CloseTextInput()
        {
            OkClicked -= CloseTextInput;
            m_CancelButton.clicked -= CloseTextInput;

            CloseDialog();
        }
    }
}
#endif
