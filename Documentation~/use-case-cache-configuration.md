# Use case: Selective caching of data for performance optimization

The Unity Cloud Assets package provides advanced controls for batching entity data when possible.

### Control entity caching

Each entity has a cache configuration. The root configuration is defined by `AssetRepositoryCacheConfiguration` and must be set on the creation of `IAssetRepository`.

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseCacheConfigurationExample.cs#Example_Behaviour_SetAssetRepositoryCacheConfiguration)]

A common parameter among cache configurations is `CacheProperties`.
This parameter controls whether an entity's properties are cached when the entity is requested. Specifically, it determines whether the `GetPropertiesAsync(CancellationToken)` method of an entity triggers an HTTP request or returns the cached data.


### Change an entity's configuration

Entities provide their current cache configuration through the `CacheConfiguration` property.

To define and assign a new configuration, use the `WithConfigurationAsync` method available in each entity.
This method returns a new entity with its data cached according to the provided configuration.

For example, you can configure an `IAsset` to cache the list of datasets, files, and file download URLs. This way subsequent calls to the entity won't require HTTP calls.

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseCacheConfigurationExample.cs#Example_Behaviour_WithCacheConfiguration)]


### Configure a query builder

The `AssetQueryBuilder` and `VersionQueryBuilder` expose a `WithCacheConfiguration(AssetCacheConfiguration)` method to define the cache configuration of each `IAsset` result.
Similarly, the `LabelQueryBuilder`, `FieldDefinitionQueryBuilder`, `AssetProjectQueryBuilder`, `CollectionQueryBuilder`, and `TransformationQueryBuilder` each expose a `WithCacheConfiguration` method to define cache configurations for their results.


## Compare caching strategies

You can set up cache configurations of entities in different ways to balance between request speed and number.
Requesting all data upfront reduces downstream calls, however, there will be an initial cost to requesting so much data.

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseCacheConfigurationExample.cs#Example_Behaviour_CachingStrategies)]


## Default SDK cache strategy

The default configuration for caching maintains the current flow of HTTP calls. By default, an entity will have its properties cached.


## Caching limitations

### No caching

If an entity's cache configuration is set to not cache any data, the requested entity is returned synchronously without without performing any HTTP calls.
It acts as an empty container from which further actions can be performed.

### Nested entities

You can request all files and datasets for an asset in a single HTTP call as files and datasets are nested within Assets.
However, since there are no limits to how many datasets an asset can contain and how many files each dataset can contain, retrieving all this data in a single call may not be practical.

Use caution when caching all asset data. If an asset’s dataset list is very large, configure the asset to not cache the dataset list, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseCacheConfigurationExample.cs#Example_Behaviour_LimitationsOption1)]


Similarly, if a dataset's file list is very large, configure the asset to not cache the file list, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseCacheConfigurationExample.cs#Example_Behaviour_LimitationsOption2)]
