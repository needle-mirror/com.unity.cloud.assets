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

    public class UseCaseSendToReviewApproveRejectAssetExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public UseCaseSendToReviewApproveRejectAssetExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseSendToReviewApproveRejectAssetExample : IAssetManagementUI
    {
        readonly UseCaseSendToReviewApproveRejectAssetExampleBehaviour m_Behaviour;

        public UseCaseSendToReviewApproveRejectAssetExample(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = new UseCaseSendToReviewApproveRejectAssetExampleBehaviour(behaviour);
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

            if (string.Equals(m_Behaviour.CurrentAsset.Status, "Draft", StringComparison.OrdinalIgnoreCase))
            {
                if (GUILayout.Button("Send to Review"))
                {
                    _ = m_Behaviour.SendAssetToReview();
                }

                GUILayout.Space(5f);
            }
            else if (string.Equals(m_Behaviour.CurrentAsset.Status, "InReview", StringComparison.OrdinalIgnoreCase))
            {
                if (GUILayout.Button("Approve in-review asset"))
                {
                    _ = m_Behaviour.ApproveInReviewAsset();
                }

                GUILayout.Space(5f);

                if (GUILayout.Button("Reject in-review asset"))
                {
                    _ = m_Behaviour.RejectInReviewAsset();
                }

                GUILayout.Space(5f);
            }
        }

        #endregion
    }

    class UseCaseSendToReviewApproveRejectAssetExampleBehaviour
    {
        const int k_DefaultCancellationTimeout = 5000;

        readonly AssetManagementBehaviour m_Behaviour;

        public bool IsProjectSelected => m_Behaviour.IsProjectSelected;
        public IAsset CurrentAsset => m_Behaviour.CurrentAsset;

        public UseCaseSendToReviewApproveRejectAssetExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_Behaviour_SendAssetToReview

        public async Task SendAssetToReview()
        {
            var cancellationTokenSrc = new CancellationTokenSource(k_DefaultCancellationTimeout);
            try
            {
                await CurrentAsset.SendToReviewAsync(cancellationTokenSrc.Token);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to send asset to review: {CurrentAsset.Name}. {e}");
            }
        }

        #endregion

        #region Example_Behaviour_ApproveInReviewAsset

        public async Task ApproveInReviewAsset()
        {
            var cancellationTokenSrc = new CancellationTokenSource(k_DefaultCancellationTimeout);
            try
            {
                await CurrentAsset.ApproveAsync(cancellationTokenSrc.Token);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to approve in-review asset: {CurrentAsset.Name}. {e}");
            }
        }

        #endregion

        #region Example_Behaviour_RejectInReviewAsset

        public async Task RejectInReviewAsset()
        {
            var cancellationTokenSrc = new CancellationTokenSource(k_DefaultCancellationTimeout);
            try
            {
                await CurrentAsset.RejectAsync(cancellationTokenSrc.Token);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to reject in-review asset: {CurrentAsset.Name}. {e}");
            }
        }

        #endregion
    }
}
