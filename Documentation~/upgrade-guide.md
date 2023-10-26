# Upgrading

## [0.6.0]

To upgrade to the Assets package version 0.5.0, you must do the following:

### Use `IAssetRepository` as the single entry point for API calls

- Create an instance of an `IAssetRepository` implementation using `AssetRepositoryFactory.Create()`.
- Use the `IAssetRepository` instance to retrieve either an 'IProject' by listing projects with `IAssetRepository.ListProjectsAsync` or getting a single project with `IAssetRepository.GetProjectAsync`.
- Use the `IProject` instance to create, get, or delete assets and asset collections.
- Use the `IAsset` instance to create, get, or delete asset files.
- Use the `IAssetFile` instance to upload, download or update asset files.

## [0.3.0]
- All instances where `IPagedResponse<T>` have been replaced with `IAsyncEnumerable<T>`.
  - The resulting enumeration must be awaited to get all the results. See the [use case for asset searches](use-case-search-assets.md) for an example.

## [0.2.0] - 2023-07-07
- `ServiceHostConfiguration` has been deprecated and replaced with `IServiceHostResolver`. 
  - Use `UnityServiceHostResolverFactory.Create()` to create a default `ServiceHostResolver`.
