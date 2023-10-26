using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets.Documentation.Scripting
{
#pragma warning disable S1144 // Remove unused private method
public class AssetFileManagerExample
{
    #region CreateAssetFile

    async Task CreateAssetFileAsync(IDataset dataset, IFileCreation fileCreation, Stream uploadStream, IProgress<HttpProgress> progress, CancellationToken token)
    {
        await dataset.UploadFileAsync(fileCreation, uploadStream, progress, token);
    }

    #endregion

    #region UpdateAssetFile

    async Task UpdateAssetFileAsync(IFile assetFile, IFileUpdate fileUpdate, CancellationToken token)
    {
        await assetFile.UpdateAsync(fileUpdate, token);
    }

    #endregion

    #region DownloadAssetFile

    async Task DownloadAssetFileAsync(IFile assetFile, Stream stream, IProgress<HttpProgress> progress, CancellationToken token)
    {
        await assetFile.DownloadAsync(stream, progress, token);
    }

    #endregion
}
#pragma warning restore S1144
}
