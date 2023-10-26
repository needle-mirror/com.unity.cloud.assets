#if !UC_EXCLUDE_SAMPLES

namespace Unity.Cloud.Assets.Samples
{
    public class TextInputDialogUi : DialogUi
    {
        void Awake()
        {
            m_DialogController ??= new TextInputDialogController();
        }
    }
}
#endif
