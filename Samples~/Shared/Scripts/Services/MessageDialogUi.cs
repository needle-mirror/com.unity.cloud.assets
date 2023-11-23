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
