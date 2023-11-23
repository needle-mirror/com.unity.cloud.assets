using System;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public interface IDialogController
    {
        event Action Opened;

        event Action Closed;

        void Init(VisualElement dialogPanel, string title);
    }

    public interface IDialogController<T, U> : IDialogController
    {
        void OpenDialog(T content);

        Task<U> OpenDialogAsync(T content);
    }
}
