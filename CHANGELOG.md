# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.0.0-exp.2] - 2023-11-07

### Added
- Added `PreviewFileUrl` to `IAsset` to expose the preview file url of the asset.

### Changed
- Updated LICENSE.md file.
- Improved information in README
- Update `ListProjectsAsync`, `GetProjectAsync`and `CreateProjectAsync` to call public api endpoints

### Fixes
- Fix SearchBarController to include Asset fields on search actions.
- Fixed potential null reference exception in in the `ThumbnailController` class used in the `Asset Discovery` sample.

### Removed
- [Breaking] Removed `UploadAsync` and `GetUploadUrlAsync` methods from `IFile`. Overwriting file content is not supported.
- Removed inapplicable notices in documentation.
- [Breaking] Removed `GetPreviewFileDownloadUrlAsync` from `IAsset`. Use `GetDownloadUrlAsync` of `IFile` instead.

### Fixed
- Fixed missing version header.
- Fix `DatasetEntity.ListFilesAsync` to show all files fields information and included `Status` as a default field.

## [1.0.0-exp.1] - 2023-10-26

### Added
- `RefreshAsync` method added to `IAsset` to refresh the asset data. This is useful for fetching additional data that is not populated by default.
- `PreviewUrl` property added to `IFile` to expose the preview url of the file.
- `ListAssetCollectionsAsync` and `GetAssetCollectionAsync` methods added to `IAssetRepository`.
- Added error popup to Asset Collection sample when creation fails.
- Added error message to Asset Collection sample during creation and edit when a collection with the same name already exists.

### Changed
- [Breaking] `GetDatasetAsync` and `GetFileAsync` methods of `IAssetRepository` now require a `DatasetFields` and `FileFields` parameter respectively.
- [Breaking] `Id` in `IAssetProject` replaced by `Descriptor` of type `ProjectDescriptor`.
- Refactored `AssetDataSource` to match other packages.
- Change minimal Unity version to 2022.3

### Fixes
- Asset Collection sample list selection now allows de-selection of item.
- Remove usage of system.web for encoding urls.
- Remove Cancellation tokens timeout from Asset Management sample to allow big file to be uploaded.
- Fix file size display to correspond correctly on the unit.
- Add checks on UI buttons in the Asset Management sample to prevent multiple clicks
- Add UI message to give feedback after an action like (publish asset, save asset)
- Added filtering of already included assets for the 'Add to Collection' asset list in Asset Collection sample.
- `IDataset` refreshes its data when `UpdateAsync` and `RemoveFileAsync` are called.
- Fix `IAsset.UnlinkFromProjectAsync` unlinking context project instead of the one passed in parameter.

### Removed
- Removing empty and unused directories and scripts.
- Removed `IAssetHttpClient`, its implementations and its tests.

## [0.7.0] - 2023-10-18

### Added
- `IDataset` to expose dataset operations.
- Default thumbnails in asset discovery sample
- `SerializeIdentifiers` method added to `IAsset` to allow for serialization.
  - `DeserializeAssetIdentifiers` added to `IAssetRepository` to deserialize identifiers into a usable `AssetDescriptor`.
- `Serialize` method added to `IAsset` to allow for serialization.
  - `DeserializeAsset` added to `IAssetRepository` to deserialize an asset from a JSON string.
- [Breaking] `FieldsFilter` added to `GetAssetAsync` operations and to the `IAssetSearchFilter` to define which `IAsset` fields are populated.
- `MockDataSource` class added. `UC_MOCK_ASSETS` symbol must be defined to use the `MockDataSource` instead of `AssetDataSource`.
- `GetFileUrl` added to `IDataset` to get the a file download url.
- `ConditionalSearchCriteria`, `DatasetSearchFilter`, `MetadataSearchFilter`
- `AssetType` enum to get predefined Asset's type supported values.
- `IsVisible` property added to `IDataset`, `IDatasetUpdate`, `IDatasetUpdateData`, `Dataset`, `DatasetUpdate`, `DatasetUpdateData` and `DatasetSearchFilter`.
- `WorkflowName` property added to `DatasetEntity`, `IDatasetData` and `DatasetData`.
- `IFile.Userchecksum` property
- `InvalidateUrls` method added to `IFile` to clean up the cached download and upload urls of the file.
- `LinkedDatasetIds` property added to `IFile`.
- `Descriptor` in `IAsset`.
- `LinkedProjects` property added to `IAsset`.
- `Descriptor` in `IDataset`.
- `Descriptor` in `IFile`.
- `WithProject` method to `IAsset` to switch between projects.
- `WithDataset` method to `IFile` to switch between datasets.

### Changed
- Changed the discovery sample to show smaller thumbnails by using an image resizer service.
- [Breaking] Migration to v1 of the Assets API.
  - [Breaking] `IFile` replaces `IAssetFile` for file operations.
  - `IAsset` exposes `IDataset` and `IFile`.
- [Breaking] New `AuthoringInfo` struct encapsulates `Created`, `CreatedBy`, `Updated`, and `UpdatedBy` properties.
- Changed `MockDataSource` to return 2 files in mocked `DatasetData.FileOrder`.
- [Breaking] Updated `AssetSearchCriteria` properties for parity with searchable fields of `IAsset`.
- [Breaking] Renamed `IProject` to `IAssetProject` to avoid conflicts with Identity's `IProject`.
- [Breaking] Remove the AssetVersionId and AssetVersionDescriptor structs and replace by AssetVersion in the AssetDescriptor.
- [Breaking] Changed `IAsset.Type` property type from `string` to `AssetType` enum.
- Updated `Asset Discovery`, `Asset Manager`, `Asset database uploader` samples to use the dataset.
- [Breaking] `ListFiles` in `IDataset` renamed to `ListFilesAsync`.
- [Breaking] `GetAssetDownloadUrlsAsync` of `IAsset` returns a mapping of file paths to Uris.
- [Breaking] Renamed `LinkedDatasetIds` in `IFile` to `LinkedDatasets` and enumerable type changed to `DatasetDescriptor`.

### Removed
- [Breaking] Removed `AssetServiceConfiguration`.
- [Breaking] `AssetTaxonomy`, `AssetAuthor`, `AssetLocation` removed.
- [Breaking] `Metadata` property of `IProject` removed.
- [Breaking] `CatalogId` and `Metadata` properties removed from `IAssetCollection`
- [Breaking] `VersionName`, `Origin`, `ShortId`, `Categories`, `StatusDetails` properties removed from `IAsset`
- Removed mocking code from `AssetDataSource`.
- [Breaking] Removed `IOrganizationProvider` and `IOrganization`. Use Identity's `IOrganizationRepository` and `IOrganization` instead.
- [Breaking] Removed `Id` and `Version` properties from `IAsset`; use `Descriptor.AssetId` and `Descriptor.Version` instead.
- [Breaking] Removed `Id` property from `IDataset`; use `Descriptor.DatasetId`.
- [Breaking] Removed `Path` property from `IFile`; use `Descriptor.FilePath`.
- [Breaking] Remove `UserCriteria` from `AssetSearchFilter`. For custom fields, extend `AssetSearchFilter` or implement `IAssetSearchFilter`.

### Fixed
- Fixed CryptographicUnexpectedOperationException during Md5 checksum calculation.
- Fixed `AssetSearchFilter.Type` criteria in search and aggregate requests.
- Fixed Shared Samples Search bar to allow search by type.
- Fixed Get collections json parsing.
- Fixed Pagination in assets search
- Fixed GetFileUrl in `Dataset` to start with ServiceUrl.
- Fixed Pagination in project list
- Cross project search with included collections.
- Aggregation search with included collections.

## [0.6.0] - 2023-09-15

### Added
- Added single entry point for API calls: `IAssetRepository`.

### Changed
- Turned AssetManager sample visible
- [Breaking] Changed how `AssetSearchFilter` searches collections. Instead of a `SearchCriteria`, populate the `Collections` list with the collection paths to search.

### Removed
- [Breaking] Removed all manager scripts: `IAssetProvider`, `IAssetManager`, `IFileManager`, `ICollectionManager`.
  - All previous actions are now available in entities: `IProject`, `IAsset`, `IAssetFile`, `IAssetCollection`.

## [0.5.0] - 2023-08-31

### Added
- [Breaking] Added the `DownloadAssetFileAsync` to `IAssetFileManager`.
- Added UseCaseDownloadFileExample documentation page.
- [Breaking] Added two new methods SendAsync to `IAssetHttpClient` to provide ways to do requests passing HttpCompletionOption argument.
- Added `InvalidDownloadUrlException` to `AssetExceptions`.
- Added official support for the latest LTS Editor 2022.3 while maintaining support for 2021.3.

### Changed
- [Breaking] Changed the `UploadAssetFileAsync` from `IAssetFileManager` to add the progress tracking.
- Changed the DiscoverySample download action to use the new `DownloadAssetFileAsync` method.
- Put `InternalsVisibleTo` attributes under conditional compilation in `Core\AssemblyInfo.cs`

### Fixed
- Fixed issue where search wasn't returning all results.

## [0.4.0] - 2023-08-17

### Added
- Added `IOrganization` property to `IProject`
- Added `AssetServiceConfiguration` parameter to `CloudAssetProvider` and `CloudAssetManager` constructors
- Added `SearchAsync` of `IAssetProvider` allow search across projects
- Updated Asset Database Uploader to keep the extension of source file in the Asset file name.
- Added search across all projects to Discovery sample
- New documentation.

### Changed
- Updated the UI of the Collection Management sample
- Updated Assets Runtime sample to allow create, upload actions.

### Removed
- [Breaking] Removed `IOrganization` parameter from all API methods that also have an `IProject` parameter.

## [0.3.0] - 2023-08-03

### Added
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
