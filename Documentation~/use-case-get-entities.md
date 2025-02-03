# Use case: Get projects, assets, datasets and files

## How do I...?

### Get a project

The entry point for the Unity Cloud Assets package is the `IAssetRepository` class.

You can get a project like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseGetEntityExample.cs#Example_GetProject)]

### Get an asset from a project

Once you have a handle on an `IAssetProject`, you can get an asset by its ID, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseGetEntityExample.cs#Example_GetAsset)]

This will search the versions of an asset and return the first result. The query can be modified to return the oldest or latest version of an asset.

If you want to get a specific version of an asset, you can do so by providing the version or label, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseGetEntityExample.cs#Example_GetAssetAlternate)]

### Get a dataset from an asset

Once you have a handle on an `IAsset`, you can get a dataset by its ID, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseGetEntityExample.cs#Example_GetDataset)]

### Get a file from a dataset

Once you have a handle on an `IDataset`, you can get a file by its path, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseGetEntityExample.cs#Example_GetFile)]

The above methods traverse the hierarchy of entities in the Unity Cloud Assets package, starting from a project and ending with a file. You can use these methods to get any entity in the hierarchy.

### Skip the hierarchy

If you know the IDs of an entity and want to get it directly, you can use methods of the `IAssetRepository` class, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseGetEntityExample.cs#Example_GetEntities)]

The above methods allow you to get any entity in the hierarchy directly, without traversing the hierarchy.
