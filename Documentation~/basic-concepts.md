# Basic concepts

This section describes concepts that the Unity Cloud Assets relies on or introduces.

## Dataset

A dataset is a set of files and metadata that are uploaded to an asset on the Asset Manager.

* By default, an asset contains two datasets: 
  * source dataset 
  * preview dataset
* An asset can have more than two datasets.
* A dataset is always linked to an asset.
* A dataset has two types of metadata:
  * primary type
  * system tag
* A dataset can reference files contained in other datasets of the same asset. This avoid the duplication of data.
* Files can be referenced in two different datasets of the same asset, to avoid duplication of data.

## Asset

An asset is a collection of one or several datasets and metadata uploaded to the Asset Manager.

* An asset is always part of at least one project. 
* An asset can be linked to multiple projects. They can optionally be part of a collection.

## Projects

Asset Projects are where Organizations can add assets, collections and users, as well as assign roles to users.

Asset Projects are Unity Cloud Projects with the Asset Manager enabled.

## Collections

Within a project, assets can be linked to one or more collections. For information, see [the use case on collection ](use-case-manage-collections.md).

## Additional resources
* For information about creating assets, see the [Use case create asset files](use-case-create-asset-files.md).
* For information about finding assets in your projects, see the [use case search assets](use-case-search-assets.md) and for searching asset across project see [search across projects](use-case-search-across-projects-assets.md).
- For information about finding and downloading published assets in your Projects, see the [Asset Discovery Sample](./asset-discovery-sample.md)
- For more information about creating, viewing, and editing assets, see the [Asset Management Sample](./asset-management-sample.md).
- Unity Cloud Assets samples rely on functionality in the Unity Identity and Access Management SDK. For more information, see [Identity package for user login](https://docs.unity3d.com/Packages/com.unity.cloud.identity@latest).
