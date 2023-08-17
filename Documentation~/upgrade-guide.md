# Upgrading

## [Unreleased]
- All instances where `IPagedResponse<T>` have been replaced with `IAsyncEnumerable<T>`.
  - The resulting enumeration must be awaited to get all the results. See the [use case for asset searches](use-case-search-assets.md) for an example.

## [0.2.0] - 2023-07-07
- `ServiceHostConfiguration` has been deprecated and replaced with `IServiceHostResolver`. 
  - Use `UnityServiceHostResolverFactory.Create()` to create a default `ServiceHostResolver`.
