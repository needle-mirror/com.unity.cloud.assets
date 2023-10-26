#if UC_MOCK_ASSETS
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    partial class MockDataSource : IAssetDataSource
    {
        Dictionary<AssetDescriptor, List<AssetData>> m_Assets = new Dictionary<AssetDescriptor, List<AssetData>>();

        AssetData GetDefaultAsset(AssetDescriptor descriptor)
        {
            return new AssetData(descriptor.AssetId, descriptor.AssetVersion)
            {
                Name = k_DefaultName,
                Description = k_DefaultDescription,
                Tags = Array.Empty<string>(),
                Type = AssetType.Other,
                SystemMetadata = null,
                PortalMetadata = null,
                Metadata = null,

                Created = DateTime.UtcNow,
                CreatedBy = k_Author,
                Updated = DateTime.UtcNow,
                UpdatedBy = k_Author,
                PreviewFile = null,
                PreviewFileUrl = null,
                Files = Array.Empty<FileData>(),
                Datasets = Array.Empty<DatasetData>(),
                Status = "",
                Labels = new List<string>(),
                SystemTags = new List<string>(),
                IsFrozen = false,
                SourceProjectId = descriptor.ProjectId,
                LinkedProjectIds = Array.Empty<ProjectId>()
            };
        }

        AssetData EnsureAssetData(AssetDescriptor descriptor)
        {
            return EnsureAssetData(descriptor.ProjectDescriptor, descriptor.AssetId, descriptor.AssetVersion);
        }

        AssetData EnsureAssetData(ProjectDescriptor projectDescriptor, AssetId assetId, AssetVersion versionId)
        {
            var assetDescriptor = new AssetDescriptor(projectDescriptor, assetId, versionId);
            EnsureProjectData(projectDescriptor.OrganizationGenesisId, projectDescriptor.ProjectId);

            if (!m_Assets.TryGetValue(assetDescriptor, out var assetList))
            {
                assetList = new List<AssetData>();
                m_Assets.Add(assetDescriptor, assetList);
            }

            var assetData = assetList.Find(a => a.Version == versionId);
            if (assetData == null)
            {
                assetData = GetDefaultAsset(assetDescriptor);

                DatasetData dataset = GetDefaultDatasetData();
                assetData.Datasets = new DatasetData[] { dataset };
                var fileData1 = GetDefaultFile($"{k_DefaultName}_1");
                var fileData2 = GetDefaultFile($"{k_DefaultName}_2");
                fileData1.DatasetIds = new[] { dataset.DatasetId };
                fileData2.DatasetIds = new[] { dataset.DatasetId };
                assetData.Files = new FileData[] { fileData1, fileData2 };
                assetData.PreviewFile = fileData1.Path;
                assetList.Add(assetData);
            }

            return assetData;
        }

        /// <inheritdoc />
        public async Task<IAssetData> CreateAssetAsync(ProjectDescriptor projectDescriptor, IAssetCreation assetCreation, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var createdAsset = EnsureAssetData(projectDescriptor, new AssetId(Guid.NewGuid()), new AssetVersion(0));
            createdAsset.Name = assetCreation.Name;
            createdAsset.Description = assetCreation.Description;
            createdAsset.Tags = assetCreation.Tags;
            createdAsset.Type = assetCreation.Type;
            createdAsset.SystemMetadata = assetCreation.SystemMetadata;
            createdAsset.PortalMetadata = assetCreation.SystemMetadata;
            createdAsset.Metadata = assetCreation.Metadata;
            createdAsset.Collections = assetCreation.Collections;

            return createdAsset;
        }

        /// <inheritdoc />
        public async Task<IAssetData> GetAssetAsync(AssetDescriptor assetDescriptor, FieldsFilter includedFieldsFilter, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return EnsureAssetData(assetDescriptor);
        }

        /// <inheritdoc />
        public async Task<bool> CheckAssetBelongsToProjectAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            return m_Assets.TryGetValue(assetDescriptor, out var assetList)
                && assetList.Exists(asset => asset.LinkedProjectIds.Contains(assetDescriptor.ProjectId));
        }

        /// <inheritdoc />
        public async Task<bool> CheckIsProjectAssetSourceAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            return m_Assets.TryGetValue(assetDescriptor, out var assetList)
                && assetList.Exists(asset => asset.SourceProjectId == assetDescriptor.ProjectId);
        }

        /// <inheritdoc />
        public async Task LinkAssetToProjectAsync(AssetDescriptor assetDescriptor, ProjectDescriptor destinationProject, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            if (m_Assets.TryGetValue(assetDescriptor, out var assetList))
            {
                foreach (var asset in assetList)
                {
                    var items = new List<ProjectId>(asset.LinkedProjectIds ?? Array.Empty<ProjectId>());
                    items.Add(destinationProject.ProjectId);
                    asset.LinkedProjectIds = items.Distinct().ToList();
                }
            }
        }

        /// <inheritdoc />
        public async Task UnlinkAssetFromProjectAsync(AssetDescriptor assetDescriptor, ProjectDescriptor destinationProject, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            if (m_Assets.TryGetValue(assetDescriptor, out var assetList))
            {
                foreach (var asset in assetList)
                {
                    var items = new List<ProjectId>(asset.LinkedProjectIds?? Array.Empty<ProjectId>());
                    items.Remove(destinationProject.ProjectId);
                    asset.LinkedProjectIds = items.Distinct().ToList();
                }
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IAssetData> ListAssetsAsync(OrganizationId organizationId, IEnumerable<ProjectId> projectIds, IAssetSearchFilter assetSearchFilter
                                                                , Pagination pagination, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            List<IAssetData> all = new List<IAssetData>();
            foreach (var projectId in projectIds)
            {
                await foreach (var asset in ListAssetsAsync(new ProjectDescriptor(organizationId, projectId), assetSearchFilter, pagination, cancellationToken))
                {
                    all.Add(asset);
                }
            }

            var assetsArray = ListItems(all.Distinct().ToList(), pagination);
            foreach (var assetData in assetsArray)
            {
                yield return assetData;
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IAssetData> ListAssetsAsync(ProjectDescriptor projectDescriptor, IAssetSearchFilter assetSearchFilter, Pagination pagination, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            List<AssetData> all = new List<AssetData>();
            var pairs = m_Assets.Where(pair => pair.Key.ProjectDescriptor == projectDescriptor).ToArray();

            if (pairs.Length == 0)
            {
                var asset1 = EnsureAssetData(projectDescriptor, new AssetId(Guid.NewGuid()), new AssetVersion(0));
                asset1.Name = "Asset1";

                var asset2 = EnsureAssetData(projectDescriptor, new AssetId(Guid.NewGuid()), new AssetVersion(0));
                asset2.Name = "Asset2";
                pairs = m_Assets.Where(pair => pair.Key.ProjectDescriptor == projectDescriptor).ToArray();
            }

            foreach (var pair in pairs)
            {
                all.Add(pair.Value.OrderByDescending(x => int.Parse(x.Version.ToString())).FirstOrDefault());
            }

            var assetsArray = ListItems(all.Distinct().ToList(), pagination);
            foreach (var assetData in assetsArray)
            {
                yield return assetData;
            }
        }

        void SetStatus (AssetDescriptor assetDescriptor, string status)
        {
            var assetData = EnsureAssetData(assetDescriptor);
            assetData.Status = status;
        }

        /// <inheritdoc />
        public async Task ApproveAssetAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            SetStatus(assetDescriptor, ChangeAssetStatusAction.approved.ToString());
        }

        /// <inheritdoc />
        public async Task PublishApprovedAssetAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            SetStatus(assetDescriptor, ChangeAssetStatusAction.published.ToString());
        }

        /// <inheritdoc />
        public async Task RejectAssetAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            SetStatus(assetDescriptor, ChangeAssetStatusAction.rejected.ToString());
        }

        /// <inheritdoc />
        public async Task SendAssetToReviewAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            SetStatus(assetDescriptor, ChangeAssetStatusAction.inreview.ToString());
        }

        /// <inheritdoc />
        public async Task WithdrawPublishedAssetAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            SetStatus(assetDescriptor, ChangeAssetStatusAction.withdrawn.ToString());
        }

        /// <inheritdoc />
        public async Task RemoveAssetMetadataAsync(AssetDescriptor assetDescriptor, string metadataType, IEnumerable<string> keys, CancellationToken token)
        {
            await Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task UpdateAssetAsync(AssetDescriptor assetDescriptor, IAssetUpdateData data, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var asset = EnsureAssetData(assetDescriptor);
            asset.Name = data.Name;
            asset.Description = data.Description;
            asset.Tags = data.Tags;
            asset.Type = data.Type;
            asset.SystemMetadata = data.SystemMetadata;
            asset.PortalMetadata = data.SystemMetadata;
            asset.Metadata = data.Metadata;
            asset.PreviewFile = data.PreviewFile;
            if (!string.IsNullOrWhiteSpace(asset.PreviewFile))
            {
                var dataset = await GetDatasetBySystemTagAsync(assetDescriptor,"preview", default, cancellationToken);
                var descriptor = new DatasetDescriptor(assetDescriptor, dataset.DatasetId);
                asset.PreviewFileUrl = (await GetFileDownloadUrlAsync(new FileDescriptor(descriptor, data.PreviewFile), null, cancellationToken)).ToString();
            }
            else
            {
                asset.PreviewFileUrl = null;
            }
        }

        /// <inheritdoc />
        public async Task<Aggregation> GetAssetAggregateAsync(ProjectDescriptor projectDescriptor, IAssetSearchFilter assetSearchFilter, AggregationParameters parameters, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return new Aggregation(null);
        }

        /// <inheritdoc />
        public async Task<Aggregation> GetAssetAggregateAsync(OrganizationId organizationId, IEnumerable<ProjectId> projectIds, IAssetSearchFilter assetSearchFilter, AggregationParameters parameters, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return new Aggregation(null);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<AssetDownloadUrl>> GetAssetDownloadUrlsAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var asset = EnsureAssetData(assetDescriptor);

            var urls = new List<AssetDownloadUrl>();
            foreach (var file in asset.Files)
            {
                var datasetId = file.DatasetIds?.FirstOrDefault();
                if (datasetId != null)
                {
                    urls.Add(new AssetDownloadUrl()
                    {
                        FilePath = file.Path,
                        DownloadUrl = await GetFileDownloadUrlAsync(new FileDescriptor(new DatasetDescriptor(assetDescriptor, datasetId.Value), file.Path), file, cancellationToken)
                    });
                }
            }

            return urls;
        }
    }
}
#endif
