using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Assets;
using Unity.Cloud.Common;
using UnityEngine;

namespace Unity.Cloud.Documentation.Assets
{
    public class UseCaseCacheConfigurationExampleBehaviour
    {
        #region Example_Behaviour_SetAssetRepositoryCacheConfiguration

        public IAssetRepository StartTransformationOnDataset(IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver)
        {
            var cacheConfiguration = new AssetRepositoryCacheConfiguration
            {
                FieldDefinitionCacheConfiguration = new FieldDefinitionCacheConfiguration(),
                LabelCacheConfiguration = new LabelCacheConfiguration(),
                AssetProjectCacheConfiguration = new AssetProjectCacheConfiguration(),
                AssetCollectionCacheConfiguration = new AssetCollectionCacheConfiguration(),
                AssetCacheConfiguration = new AssetCacheConfiguration
                {
                    DatasetCacheConfiguration = new DatasetCacheConfiguration
                    {
                        FileCacheConfiguration = new FileCacheConfiguration()
                    }
                },
                TransformationCacheConfiguration = new TransformationCacheConfiguration()
            };

            return AssetRepositoryFactory.Create(serviceHttpClient, serviceHostResolver, cacheConfiguration);
        }

        #endregion

        #region Example_Behaviour_CachingStrategies

        public async Task SearchLatestAssetsAsync(IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver)
        {
            // Create an asset repository with no caching by default - setup for asset repositoy should only happen once.
            var assetRepository = AssetRepositoryFactory.Create(serviceHttpClient, serviceHostResolver, AssetRepositoryCacheConfiguration.NoCaching);

            // Call acts synchronously
            var project = await assetRepository.GetAssetProjectAsync(new ProjectDescriptor(new OrganizationId("organization-id"), new ProjectId("project-id")), CancellationToken.None);

            // Setup a search query to get all assets with the label "latest"
            var searchFilter = new AssetSearchFilter();
            searchFilter.Include().Labels.WithValue("latest");

            // Setup a cache configuration to cache properties, metadata, and the dataset list
            var datasetCacheConfiguration = new DatasetCacheConfiguration
            {
                CacheProperties = true,
                FileCacheConfiguration = FileCacheConfiguration.NoCaching
            };

            var assetCacheConfiguration = new AssetCacheConfiguration
            {
                CacheProperties = true,
                CacheMetadata = true,
                CacheDatasetList = true,
                DatasetCacheConfiguration = datasetCacheConfiguration
            };

            var query = project.QueryAssets()
                .SelectWhereMatchesFilter(searchFilter)
                .WithCacheConfiguration(assetCacheConfiguration)
                .ExecuteAsync(CancellationToken.None);

            // Http call to get a paginated result.
            await foreach (var asset in query)
            {
                // Call acts synchronously since properties were cached by the request.
                var properties = await asset.GetPropertiesAsync(CancellationToken.None);

                Debug.Log(properties.Name);
                Debug.Log(string.Join(", ", properties.Tags));
                Debug.Log(string.Join(", ", properties.Labels));

                // etc.

                // Metadata will enumerate synchronously since it was cached by the request.
                var metadataQuery = asset.Metadata.Query().ExecuteAsync(CancellationToken.None);
                await foreach (var metadata in metadataQuery)
                {
                    Debug.Log($"{metadata.Key}::{metadata.Value.ValueType}");
                }

                // Dataset list will enumerate synchronously since it was cached by the request.
                await foreach (var dataset in asset.ListDatasetsAsync(Range.All, CancellationToken.None))
                {
                    // Call acts synchronously since properties were cached by the request.
                    var datasetProperties = await dataset.GetPropertiesAsync(CancellationToken.None);

                    Debug.Log(datasetProperties.Name);

                    // etc.
                }
            }
        }

        public async Task GetFileInfoAsync(IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver)
        {
            // Create an asset repository with no caching by default - setup for asset repositoy should only happen once.
            var assetRepository = AssetRepositoryFactory.Create(serviceHttpClient, serviceHostResolver, AssetRepositoryCacheConfiguration.NoCaching);

            // Call acts synchronously since we are not caching any properties.
            var project = await assetRepository.GetAssetProjectAsync(new ProjectDescriptor( /* project info here */), CancellationToken.None);

            // Call will act asynchronously because we need to fetch the `AssetVersion` of the asset.
            var asset = await project.GetAssetAsync(new AssetId("asset-id"), "Latest", CancellationToken.None);

            // Call acts synchronously since we are not caching any properties.
            var dataset = await asset.GetDatasetAsync(new DatasetId("dataset-id"), CancellationToken.None);

            //
            // Scenario 1. No caching
            //

            // Call acts synchronously since we are not caching any properties.
            var file = await dataset.GetFileAsync("file-path", CancellationToken.None);

            // Call is asynchronous since we have not cached any properties.
            var properties = await file.GetPropertiesAsync(CancellationToken.None);
            Debug.Log(properties.SizeBytes);
            Debug.Log(string.Join(", ", properties.Tags));

            // etc.

            // Call is asynchronous since we have not cached any properties.
            var downloadUrl = await file.GetDownloadUrlAsync(CancellationToken.None);
            Debug.Log(downloadUrl);

            // Call is asynchronous since we have not cached any properties.
            var previewUrl = await file.GetPreviewUrlAsync(CancellationToken.None);
            Debug.Log(previewUrl);

            //
            // Scenario 2. Cache all
            //

            var fileCacheConfiguration = new FileCacheConfiguration
            {
                CacheProperties = true,
                CacheDownloadUrl = true,
                CachePreviewUrl = true
            };

            var datasetCacheConfiguration = new DatasetCacheConfiguration
            {
                FileCacheConfiguration = fileCacheConfiguration
            };

            // Here we fetch a new dataset with the specified configuration.
            // Call acts synchronously since we are not caching anything in the dataset directly.
            dataset = await dataset.WithCacheConfigurationAsync(datasetCacheConfiguration, CancellationToken.None);

            // Call acts asynchronously since the new configuration requires caching of certain fields.
            file = await dataset.GetFileAsync("file-path", CancellationToken.None);

            // Call is synchronously since we have cached the properties.
            properties = await file.GetPropertiesAsync(CancellationToken.None);
            Debug.Log(properties.SizeBytes);
            Debug.Log(string.Join(", ", properties.Tags));

            // etc.

            // Call is synchronously since we have cached the download url.
            downloadUrl = await file.GetDownloadUrlAsync(CancellationToken.None);
            Debug.Log(downloadUrl);

            // Call is synchronously since we have cached the preview url.
            previewUrl = await file.GetPreviewUrlAsync(CancellationToken.None);
            Debug.Log(previewUrl);
        }

        #endregion


        public AssetCacheConfiguration CacheConfigurationSetup1()
        {
#region Example_Behaviour_LimitationsOption1

AssetCacheConfiguration assetCacheConfiguration = new AssetCacheConfiguration
{
    CacheProperties = true,
    CacheDatasetList = true,
    DatasetCacheConfiguration = new DatasetCacheConfiguration
    {
        CacheProperties = true,
        CacheFileList = false,
        FileCacheConfiguration = new FileCacheConfiguration
        {
            CacheProperties = true
        }
    }
};

#endregion

return assetCacheConfiguration;
        }

        public AssetCacheConfiguration CacheConfigurationSetup2()
        {
#region Example_Behaviour_LimitationsOption2

AssetCacheConfiguration assetCacheConfiguration = new AssetCacheConfiguration
{
    CacheProperties = true,
    CacheDatasetList = false,
    DatasetCacheConfiguration = new DatasetCacheConfiguration
    {
        CacheProperties = true,
        CacheFileList = true,
        FileCacheConfiguration = new FileCacheConfiguration
        {
            CacheProperties = true
        }
    }
};

#endregion

return assetCacheConfiguration;
        }

#region Example_Behaviour_WithCacheConfiguration

public async IAsyncEnumerable<Uri> ListFileDownloadUrlsAsync(IAsset asset, [EnumeratorCancellation] CancellationToken cancellationToken)
{
    AssetCacheConfiguration cacheConfiguration = new AssetCacheConfiguration()
    {
        CacheProperties = false,
        CacheMetadata = false,
        CacheSystemMetadata = false,
        CacheDatasetList = true,
        DatasetCacheConfiguration = new DatasetCacheConfiguration()
        {
            CacheProperties = false,
            CacheMetadata = false,
            CacheFileList = true,
            FileCacheConfiguration = new FileCacheConfiguration()
            {
                CacheProperties = false,
                CacheMetadata = false,
                CacheDownloadUrl = true
            }
        }
    };

    // This will trigger a HTTP call and populate asset information including the dataset list, file list, and download url for each file.
    IAsset assetWithCachedFileUrls = await asset.WithCacheConfigurationAsync(cacheConfiguration, cancellationToken);

    // This will not require a HTTP call because the dataset list has been cached.
    await foreach (IDataset dataset in assetWithCachedFileUrls.ListDatasetsAsync(Range.All, cancellationToken))
    {
        // This will not require a HTTP call because the file list has been cached.
        await foreach (IFile file in dataset.ListFilesAsync(Range.All, cancellationToken))
        {
            // This will not require a HTTP call because the download url has been cached.
            yield return await file.GetDownloadUrlAsync(cancellationToken);
        }
    }
}

#endregion
    }
}
