namespace Unity.Cloud.Documentation.Assets
{
#pragma warning disable S4487 // Unread "private" fields should be removed
#pragma warning disable S1186 // Methods should not be empty

    #region Example_UIClass

    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Unity.Cloud.Assets;
    using UnityEngine;

    public class UseCasePublishWithdrawAssetExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public UseCasePublishWithdrawAssetExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCasePublishWithdrawAssetExample : IAssetManagementUI
    {
        readonly UseCasePublishWithdrawAssetExampleBehaviour m_Behaviour;

        public UseCasePublishWithdrawAssetExample(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = new UseCasePublishWithdrawAssetExampleBehaviour(behaviour);
        }

        #region Example_UIContent

        public void OnGUI()
        {
            if (!m_Behaviour.IsProjectSelected) return;

            if (m_Behaviour.CurrentAsset == null)
            {
                GUILayout.Label(" ! No asset selected !");
                return;
            }

            if (string.Equals(m_Behaviour.CurrentAsset.Status, "Approved", StringComparison.OrdinalIgnoreCase))
            {
                if (GUILayout.Button("Publish"))
                {
                    _ = m_Behaviour.PublishAsset();
                }

                GUILayout.Space(5f);
            }

            if (string.Equals(m_Behaviour.CurrentAsset.Status, "Published", StringComparison.OrdinalIgnoreCase))
            {
                if (GUILayout.Button("Withdraw"))
                {
                    _ = m_Behaviour.WithdrawAsset();
                }

                GUILayout.Space(5f);
            }
        }

        #endregion
    }

    class UseCasePublishWithdrawAssetExampleBehaviour
    {
        const int k_DefaultCancellationTimeout = 5000;

        readonly AssetManagementBehaviour m_Behaviour;

        public bool IsProjectSelected => m_Behaviour.IsProjectSelected;
        public IAsset CurrentAsset => m_Behaviour.CurrentAsset;

        public UseCasePublishWithdrawAssetExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_Behaviour_PublishAsset

        public async Task PublishAsset()
        {
            var cancellationTokenSrc = new CancellationTokenSource(k_DefaultCancellationTimeout);
            try
            {
                await CurrentAsset.PublishAsync(cancellationTokenSrc.Token);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to publish asset: {CurrentAsset.Name}. {e}");
            }
        }

        #endregion

        #region Example_Behaviour_WithdrawAsset

        public async Task WithdrawAsset()
        {
            var cancellationTokenSrc = new CancellationTokenSource(k_DefaultCancellationTimeout);
            try
            {
                await CurrentAsset.WithdrawAsync(cancellationTokenSrc.Token);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to withdraw published asset: {CurrentAsset.Name}. {e}");
            }
        }

        #endregion
    }
}
