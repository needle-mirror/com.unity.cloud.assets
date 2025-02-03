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
* A dataset can reference files contained in other datasets of the same asset. This avoids data duplication.
* Files can be referenced in two different datasets of the same asset. This avoids data duplication.

## Asset

An asset is a collection of one or several datasets and metadata uploaded to Asset Manager.

* An asset is always part of at least one project.
* You can link an asset to multiple projects. Optionally, you can add this asset to those projects' collections.

## Projects

Asset projects are Unity Cloud projects with Asset Manager enabled. In Asset Manager projects, members of an organization can add assets, collections, and users. They can also assign roles to users.  

## Collections

Within a project, you can link assets to one or more collections. Read more about [collections](use-case-manage-collections.md).

## Metadata

Metadata exposes additional information about an asset, dataset, or file.
Metadata keys are defined in an organization's library. Read more about [managing metadata keys](use-case-manage-fields.md).
When you have created keys within an organization, use them to populate the metadata of assets, datasets, and files. Read more about [adding or updating an asset's metadata entries](use-case-manage-asset-metadata.md).

## Additional resources

* Read more about:
  * [Creating assets](use-case-create-files.md).
  * [Finding assets in your projects](use-case-search-assets.md).
  * [Searching across projects](use-case-search-across-projects-assets.md).
  * [Finding and downloading published assets in your projects](./asset-discovery-sample.md)
  * [Creating, viewing, and editing assets](./asset-management-sample.md).

Unity Cloud Assets samples rely on functionality in the Unity Identity and Access Management SDK. Read more about the [Identity package](https://docs.unity3d.com/Packages/com.unity.cloud.identity@latest).
