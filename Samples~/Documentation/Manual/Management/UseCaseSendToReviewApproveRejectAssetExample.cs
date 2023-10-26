using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Cloud.Assets.Documentation.Management
{
    public class UseCaseSendToReviewApproveRejectAssetExample
    {
        readonly UseCaseSendToReviewApproveRejectAssetExampleBehaviour m_Behaviour = new();

        public void DisplayExample(IAsset asset)
        {
            m_Behaviour.Initialize(asset);
            AdditionalActions();
        }

        #region Example_UI

        protected virtual void AdditionalActions()
        {
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

        // Member names should match with the names of the get-started behaviour snippets.
        public IAsset CurrentAsset;

        public void Initialize(IAsset asset)
        {
            CurrentAsset = asset;
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
