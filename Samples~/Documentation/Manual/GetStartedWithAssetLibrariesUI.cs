namespace Unity.Cloud.Documentation.Assets
{
    #region Example

    using System;
    using Unity.Cloud.Identity;
    using UnityEngine;

    public class AssetLibrariesUI : BaseAssetUI<AssetLibrariesBehaviour>
    {
        protected virtual void Awake()
        {
            m_UI.Add(new AssetLibrarySelectionExampleUI(m_Behaviour));
            m_UI.Add(new AssetSelectionExampleUI(m_Behaviour));
            m_UI.Add(new UseCaseViewAssetExampleUI(m_Behaviour));
        }

        protected override void OnAuthenticationStateChanged(AuthenticationState obj)
        {
            if (obj == AuthenticationState.LoggedIn)
            {
                _ = m_Behaviour.GetAssetLibrariesAsync();
                _ = m_Behaviour.GetAssetLibraryJobsAsync();
            }
        }
    }

    #endregion
}
