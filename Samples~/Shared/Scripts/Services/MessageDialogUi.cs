#if !UC_EXCLUDE_SAMPLES

namespace Unity.Cloud.Assets.Samples
{
    public class MessageDialogUi : DialogUi
    {
        void Awake()
        {
            m_DialogController ??= new MessageDialogController();
        }
    }
}
#endif
