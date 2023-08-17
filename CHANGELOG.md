# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.4.0] - 2023-08-17

### Added
- Added `IOrganization` property to `IProject`
- Added `AssetServiceConfiguration` parameter to `CloudAssetProvider` and `CloudAssetManager` constructors
- Added `SearchAsync` of `IAssetProvider` allow search across projects
- Updated Asset Database Uploader to keep the extension of source file in the Asset file name.
- Added search across all projects to Discovery sample
- New documentation.

### Removed
- [Breaking] Removed `IOrganization` parameter from all API methods that also have an `IProject` parameter.

## [0.3.0] - 2023-08-03

### Added
- Added asset manager sample to allow publish or saving of an asset.
- Added asset collection management sample to allow creation, deletion, and updating of asset collections.

### Changed
- Updated Asset Database Uploader to keep the extension of source file in the Asset file name.
- Added headers to requests.
- [Breaking] `SearchAsync` of `IAssetProvider` now returns an `IAsyncEnumerable<IAsset>`.
- [Breaking] `GetCurrentUserProjectList` of `IProjectProvider` renamed to `ListProjectsAsync` and now returns an `IAsyncEnumerable<IProject>`.

### Removed
- [Breaking] Removed `GetProjectsByOrganizationAndUserIdsAsync` from `IProjectProvider`
- [Breaking] Removed `IPagedResponse<T>`, `IAssetPage`, and `IProjectPage`.
- [Breaking] Removed `Projects` list property from `IOrganization`.

## [0.2.1] - 2023-07-20

### Added
- Added Send asset to review, Approve asset in review and Reject asset in review requests in `IAssetManager`
- Added Check on GetAssetByIdAndVersionRequest.IncludeThumbnailDownloadURLs value before adding it to the query parameters
- Adds search value selection to Discovery sample

### Changed
- Updated miscellaneous existing documentation pages.

### Fixed
- Fixed a bug where the `GetAssetFileUrlAsync` of `CloudFileAssetManager` was failing and no url was returned.

## [0.2.0] - 2023-07-06

### Added
- UnityEditor uploader Sample.
- [Breaking] Added `IAssetSearchFilter` interface to allow for more complex search filters and replaced references to the implemented `AssetSearchFilter` class with the interface.
- Exposed an abstract `AssetPage` for extension.
- Added overload for `SearchAsync` in `ICloudAssetProvider` that takes an `IAsset` type parameter.
- New method to get asset collections in AssetManager
- New documentation.
- Added `IOrganization` and `IProject` properties to `IAssetCollection`.

### Changed
- Updated sample to show thumbnails.
- Added `IAsset` type parameter to `IAssetPage` `GetNextAsync` method.
- Updated documentation for getting started pathways.
- [Breaking] Replaced the `TryGetValue` function in `Aggregation` with the `Values` property.
- Updated sample to show asset collections.
- Updated documentation's Getting started pages.
- [Breaking] Replaced the `IAssetFile` parameter of `CreateAssetFileAsync` in `IAssetFileManager` with an `IAssetFileCreation` object.
- Added an `IAssetFile` return value to `CreateAssetFileAsync` in `IAssetFileManager`.
- [Breaking] Changed the return value of task `UploadAssetFileAsync` from `IAssetFileManager` to `bool`.
- [Breaking] Uses of `ServiceHostConfiguration` have been replaced for `IServiceHostResolver`.
- Updated all references to Common's `IHttpClient.SendAsync` to match its new signature.
- [Breaking] Renamed `IAssetCollectionController` to `IAssetCollectionManager`.
- [Breaking] Removed the return values of `InsertAssetsToCollectionAsync` and `RemoveAssetsFromCollectionAsync` in `IAssetCollectionManager`.
- [Breaking] Removed the `IOrganization` and `IProject` parameters from `UpdateCollectionAsync`, `DeleteCollectionAsync`, and `MoveCollectionAsync` in `IAssetCollectionManager`.

### Removed
- [Breaking] Removed the `AggregationFields` property from `Aggregation`.

## [0.1.0] - 2023-06-22

### Changed
- Upgrade to Moq 2.0.0-pre.2
- Removed default values for `ServiceEnvironment` in documentation.