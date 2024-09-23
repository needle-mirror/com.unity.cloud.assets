using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Assets;
using Unity.Cloud.Common;

namespace Unity.Cloud.Documentation.Assets
{
    public class UseCaseGetEntityExample
    {
        #region Example_GetProject

        public static async Task<IAssetProject> GetProjectAsync(IAssetRepository assetRepository, string organizationId, string projectId)
        {

            var projectDescriptor = new ProjectDescriptor(new OrganizationId(organizationId), new ProjectId(projectId));
            return await assetRepository.GetAssetProjectAsync(projectDescriptor, CancellationToken.None);
        }

        #endregion

        #region Example_GetAsset

        public static async Task<IAsset> GetAssetAsync(IAssetProject assetProject, string assetId)
        {
            return await assetProject.GetAssetAsync(new AssetId(assetId), CancellationToken.None);
        }

        #endregion

        #region Example_GetAssetAlternate

        public static async Task<IAsset> GetAssetAsyncByVersion(IAssetProject assetProject, string assetId, string versionId)
        {
            return await assetProject.GetAssetAsync(new AssetId(assetId), new AssetVersion(versionId), CancellationToken.None);
        }

        public static async Task<IAsset> GetAssetAsyncByLabel(IAssetProject assetProject, string assetId, string label)
        {
            return await assetProject.GetAssetAsync(new AssetId(assetId), label, CancellationToken.None);
        }

        #endregion

        #region Example_GetDataset

        public static async Task<IDataset> GetAssetAsync(IAsset asset, string datasetId)
        {
            return await asset.GetDatasetAsync(new DatasetId(datasetId), CancellationToken.None);
        }

        #endregion

        #region Example_GetFile

        public static async Task<IFile> GetFileAsync(IDataset dataset, string filePath)
        {
            return await dataset.GetFileAsync(filePath, CancellationToken.None);
        }

        #endregion

        #region Example_GetEntities

        public static async Task<IAsset> GetAssetAsync(IAssetRepository assetRepository, string organizationId, string projectId, string assetId, string assetVersion)
        {
            var projectDescriptor = new ProjectDescriptor(new OrganizationId(organizationId), new ProjectId(projectId));
            var assetDescriptor = new AssetDescriptor(projectDescriptor, new AssetId(assetId), new AssetVersion(assetVersion));

            return await assetRepository.GetAssetAsync(assetDescriptor, CancellationToken.None);
        }

        public static async Task<IAsset> GetAssetByLabelAsync(IAssetRepository assetRepository, string organizationId, string projectId, string assetId, string label)
        {
            var projectDescriptor = new ProjectDescriptor(new OrganizationId(organizationId), new ProjectId(projectId));

            return await assetRepository.GetAssetAsync(projectDescriptor, new AssetId(assetId), label, CancellationToken.None);
        }

        public static async Task<IDataset> GetDatasetAsync(IAssetRepository assetRepository, string organizationId, string projectId, string assetId, string assetVersion, string datasetId)
        {
            var projectDescriptor = new ProjectDescriptor(new OrganizationId(organizationId), new ProjectId(projectId));
            var assetDescriptor = new AssetDescriptor(projectDescriptor, new AssetId(assetId), new AssetVersion(assetVersion));
            var datasetDescriptor = new DatasetDescriptor(assetDescriptor, new DatasetId(datasetId));

            return await assetRepository.GetDatasetAsync(datasetDescriptor, CancellationToken.None);
        }

        public static async Task<IFile> GetFileAsync(IAssetRepository assetRepository, string organizationId, string projectId, string assetId, string assetVersion, string datasetId, string filePath)
        {
            var projectDescriptor = new ProjectDescriptor(new OrganizationId(organizationId), new ProjectId(projectId));
            var assetDescriptor = new AssetDescriptor(projectDescriptor, new AssetId(assetId), new AssetVersion(assetVersion));
            var datasetDescriptor = new DatasetDescriptor(assetDescriptor, new DatasetId(datasetId));
            var fileDescriptor = new FileDescriptor(datasetDescriptor, filePath);

            return await assetRepository.GetFileAsync(fileDescriptor, CancellationToken.None);
        }

        #endregion
    }
}
