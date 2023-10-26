#if !UC_EXCLUDE_SAMPLES

namespace Unity.Cloud.Assets.Samples
{
    public interface IDialogResult<out T>
    {
        bool IsConfirmed { get; }
        T Content { get; }
    }

}
#endif
