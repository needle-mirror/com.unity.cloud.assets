using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Cloud.Assets.Documentation.Management
{
    public class UseCasePublishWithdrawAssetExample
    {
        readonly UseCasePublishWithdrawAssetExampleBehaviour m_Behaviour = new();

        public void DisplayExample(IAsset asset)
        {
            m_Behaviour.Initialize(asset);
            AssetActions();
        }

        #region Example_UI

        protected virtual void AssetActions()
        {
            if (m_Behaviour.CurrentAsset == null)
            {
                GUILayout.Label(" ! No asset selected !");
                return;
            }

            if(string.Equals(m_Behaviour.CurrentAsset.Status, "Approved", StringComparison.OrdinalIgnoreCase))
            {
                if (GUILayout.Button("Publish"))
                {
                    _ = m_Behaviour.PublishAsset();
                }
                GUILayout.Space(5f);
            }

            if(string.Equals(m_Behaviour.CurrentAsset.Status, "Published", StringComparison.OrdinalIgnoreCase))
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

        // Member names should match with the names of the get-started behaviour snippets.
        public IAsset CurrentAsset;

        public void Initialize(IAsset asset)
        {
            CurrentAsset = asset;
        }

        #region Example_Behaviour_PublishAsset

        public async Task PublishAsset()
        {
            var cancellationTokenSrc = new CancellationTokenSource(k_DefaultCancellationTimeout);
            try
            {
                await PlatformServices.AssetManager.PublishApprovedAssetAsync(CurrentAsset, cancellationTokenSrc.Token);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to publish asset: {CurrentAsset.Name}. {e.Message}");
            }
        }

        #endregion

        #region Example_Behaviour_WithdrawAsset

        public async Task WithdrawAsset()
        {
            var cancellationTokenSrc = new CancellationTokenSource(k_DefaultCancellationTimeout);
            try
            {
                await PlatformServices.AssetManager.WithdrawPublishedAssetAsync(CurrentAsset, cancellationTokenSrc.Token);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to withdraw published asset: {CurrentAsset.Name}. {e.Message}");
            }
        }

        #endregion
    }
}
