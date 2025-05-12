using System.Collections.Generic;

namespace Unity.Cloud.Documentation.Assets
{

#pragma warning disable S3453 // Classes should not have only "private" constructors
#pragma warning disable S1144 // Unused private types or members should be removed
#pragma warning disable S1481 // Unused local variables should be removed

    class UseCaseSetupExamples
    {
        readonly AssetManagementBehaviour m_Behaviour = new();
        readonly List<IAssetManagementUI> m_UI = new();

        UseCaseSetupExamples()
        {
            #region UseCaseAssetUpdate

            m_UI.Add(new OrganizationSelectionExampleUI(m_Behaviour));
            m_UI.Add(new ProjectSelectionExampleUI(m_Behaviour));
            m_UI.Add(new AssetSelectionExampleUI(m_Behaviour));
            m_UI.Add(new UseCaseManageAssetExampleUI(m_Behaviour));

            #endregion

            #region UseCaseAggregateAssets

            m_UI.Add(new OrganizationSelectionExampleUI(m_Behaviour));
            m_UI.Add(new ProjectSelectionExampleUI(m_Behaviour));
            m_UI.Add(new UseCaseAggregationExampleUI(m_Behaviour));

            #endregion

            #region UseCaseAssetCollections

            m_UI.Add(new OrganizationSelectionExampleUI(m_Behaviour));
            m_UI.Add(new ProjectSelectionExampleUI(m_Behaviour));
            m_UI.Add(new AssetSelectionExampleUI(m_Behaviour));
            m_UI.Add(new UseCaseAssetCollectionExampleUI(m_Behaviour));

            #endregion

            #region UseCaseCreateDatasets

            m_UI.Add(new OrganizationSelectionExampleUI(m_Behaviour));
            m_UI.Add(new ProjectSelectionExampleUI(m_Behaviour));
            m_UI.Add(new AssetSelectionExampleUI(m_Behaviour));
            m_UI.Add(new UseCaseDatasetCreationExampleUI(m_Behaviour));

            #endregion

            #region UseCaseCreateFiles

            m_UI.Add(new OrganizationSelectionExampleUI(m_Behaviour));
            m_UI.Add(new ProjectSelectionExampleUI(m_Behaviour));
            m_UI.Add(new AssetSelectionExampleUI(m_Behaviour));
            m_UI.Add(new UseCaseFileCreationExampleUI(m_Behaviour));

            #endregion

            #region UseCaseAssetMetadata

            m_UI.Add(new OrganizationSelectionExampleUI(m_Behaviour));
            m_UI.Add(new ProjectSelectionExampleUI(m_Behaviour));
            m_UI.Add(new AssetSelectionExampleUI(m_Behaviour));
            m_UI.Add(new UseCaseAssetMetadataExampleUI(m_Behaviour));

            #endregion

            #region UseCaseManageCollections

            m_UI.Add(new OrganizationSelectionExampleUI(m_Behaviour));
            m_UI.Add(new ProjectSelectionExampleUI(m_Behaviour));
            m_UI.Add(new AssetSelectionExampleUI(m_Behaviour));
            m_UI.Add(new UseCaseManageCollectionsExampleUI(m_Behaviour));

            #endregion

            #region UseCaseManageFields

            m_UI.Add(new OrganizationSelectionExampleUI(m_Behaviour));
            m_UI.Add(new UseCaseManageFieldDefinitionsExampleUI(m_Behaviour));

            #endregion

            #region UseCaseModifyAcceptedValues

            m_UI.Add(new OrganizationSelectionExampleUI(m_Behaviour));
            m_UI.Add(new UseCaseFieldDefinitionsModifyAcceptedValuesExampleUI(m_Behaviour));

            #endregion

            #region UseCaseManageAssetStatus

            m_UI.Add(new OrganizationSelectionExampleUI(m_Behaviour));
            m_UI.Add(new ProjectSelectionExampleUI(m_Behaviour));
            m_UI.Add(new AssetSelectionExampleUI(m_Behaviour));
            m_UI.Add(new UseCaseManageAssetStatusExampleUI(m_Behaviour));

            #endregion

            #region UseCaseReplaceFiles

            m_UI.Add(new OrganizationSelectionExampleUI(m_Behaviour));
            m_UI.Add(new ProjectSelectionExampleUI(m_Behaviour));
            m_UI.Add(new AssetSelectionExampleUI(m_Behaviour));
#if UNITY_EDITOR && !UNITY_WEBGL
            m_UI.Add(new UseCaseFileReuploadExampleUI(m_Behaviour));
#endif

            #endregion

            #region UseCaseFileManagement

            m_UI.Add(new OrganizationSelectionExampleUI(m_Behaviour));
            m_UI.Add(new ProjectSelectionExampleUI(m_Behaviour));
            m_UI.Add(new AssetSelectionExampleUI(m_Behaviour));
#if UNITY_EDITOR && !UNITY_WEBGL
            m_UI.Add(new UseCaseFileManagementExampleUI(m_Behaviour));
#endif

            #endregion

            #region UseCaseUpdateDatasets

            m_UI.Add(new OrganizationSelectionExampleUI(m_Behaviour));
            m_UI.Add(new ProjectSelectionExampleUI(m_Behaviour));
            m_UI.Add(new AssetSelectionExampleUI(m_Behaviour));
            m_UI.Add(new UseCaseDatasetUpdateExampleUI(m_Behaviour));

            #endregion

            #region UseCaseVersionSearch

            m_UI.Add(new OrganizationSelectionExampleUI(m_Behaviour));
            m_UI.Add(new ProjectSelectionExampleUI(m_Behaviour));
            m_UI.Add(new AssetSelectionExampleUI(m_Behaviour));
            m_UI.Add(new UseCaseVersionSearchExampleUI(m_Behaviour));

            #endregion

            #region UseCaseManageAssetReferences

            m_UI.Add(new OrganizationSelectionExampleUI(m_Behaviour));
            m_UI.Add(new ProjectSelectionExampleUI(m_Behaviour));
            m_UI.Add(new AssetSelectionExampleUI(m_Behaviour));
            m_UI.Add(new UseCaseManageAssetReferencesExampleUI(m_Behaviour));

            #endregion
        }
    }

#pragma warning restore S1481 // Unused local variables should be removed
#pragma warning restore S1144 // Unused private types or members should be removed
#pragma warning restore S3453 // Classes should not have only "private" constructors

}
