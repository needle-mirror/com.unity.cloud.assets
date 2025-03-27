# Use case: Selective caching of data for performance tweaking

The Unity Cloud Assets package exposes advanced controls for batching entity data when possible.

## How do I...?

### Control entity caching

Each entity exposes a cache configuration. The root configuration is defined by `AssetRepositoryCacheConfiguration` and must be set on creation of the `IAssetRepository`.

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseCacheConfigurationExample.cs#Example_Behaviour_SetAssetRepositoryCacheConfiguration)]

A common parameter among cache configurations is `CacheProperties`.
This parameter controls whether the properties of an entity will be cached when the entity is requested and more specifically whether the `GetPropertiesAsync(CancellationToken)` method of an entity will trigger an HTTP request or return the cached data.


### Change an entity's configuration

Entities expose their current cache configuration through the `CacheConfiguration` property.

A new configuration can be defined and assigned using the `WithConfigurationAsync` method available in each entity.
This method will return a new entity with its data cached as defined by the provided configuration.

For example, an `IAsset` could be configured to cache the list of datasets, files and file download urls so that subsequent calls to the entity do not require HTTP calls.

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseCacheConfigurationExample.cs#Example_Behaviour_WithCacheConfiguration)]


### Configure a query builder

The `AssetQueryBuilder` and `VersionQueryBuilder` each expose a `WithCacheConfiguration(AssetCacheConfiguration)` for defining the cache configuration of each `IAsset` result.
Similarly, the `LabelQueryBuilder`, `FieldDefinitionQueryBuilder`, `AssetProjectQueryBuilder`, `CollectionQueryBuilder`, and `TransformationQueryBuilder` each expose a `WithCacheConfiguration` method for defining the cache configuration of their results.


## Comparing caching strategies

There are many ways to set up the cache configurations of entities to manipulate the balance between request speed and number.
Requesting all data upfront will result in fewer downstream calls, however there will be an initial cost to requesting so much data.

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseCacheConfigurationExample.cs#Example_Behaviour_CachingStrategies)]


## Default SDK cache strategy

A default configuration for caching has been defined to maintain the current flow of HTTP calls. By default, an entity will have its properties cached.


## Caching limitations

### No caching

When a cache configuration is configured to not cache any data, the requested entity will be returned in a synchronous manner, without performing any HTTP calls.
It will act as an empty container from which further actions can be performed.

### Nested entities

Due to the nesting of files and datasets within Assets, it is possible to request all files and datasets for an asset in a single HTTP call.
However, because there are no limits to how many datasets an asset can contain and how many files each dataset can contain, one must consider the practical limitations of requesting all this information in a single call.

It is therefore advised to use caution when caching all asset data. When there is a risk of asset dataset lists being very large, the asset configuration can be configured to not cache the dataset list, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseCacheConfigurationExample.cs#Example_Behaviour_LimitationsOption1)]

Similarly, if there is a risk that dataset file lists are very large, the asset configuration can be configured to not cache the file list, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseCacheConfigurationExample.cs#Example_Behaviour_LimitationsOption2)]
