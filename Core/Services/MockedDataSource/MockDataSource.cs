#if UC_MOCK_ASSETS
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    partial class MockDataSource : IAssetDataSource
    {
        public async Task DownloadContentAsync(Uri downloadUri, Stream destinationStream, IProgress<HttpProgress> progress, CancellationToken token)
        {
            await Task.CompletedTask;
            var bytes = new byte[k_SizeBytes];
            for (byte i = 0; i < bytes.Length; ++i)
                bytes[i] = i;

#if UNITY_WEBGL && !UNITY_EDITOR
            destinationStream.Write(bytes, 0, bytes.Length, token);
#else
            await destinationStream.WriteAsync(bytes, 0, bytes.Length, token);
#endif
        }

        public async Task UploadContentAsync(Uri uploadUri, Stream sourceStream, IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            const int k_bufferSize = 4096;
            progress?.Report(new HttpProgress(null, 0));

            byte[] buffer = new byte[k_bufferSize];
            int bytesRead;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();
#if UNITY_WEBGL && !UNITY_EDITOR
                bytesRead = await Task.FromResult(sourceStream.Read(buffer, 0, k_bufferSize));
#else
                bytesRead = await sourceStream.ReadAsync(buffer, 0, k_bufferSize, cancellationToken);

                if (sourceStream.Length > 0)
                {
                    float progressValue;
                    try
                    {

                        progressValue = (float)sourceStream.Position / (float)sourceStream.Length;
                    }
                    catch
                    {
                        progressValue = 0;
                    }
                    progress?.Report(new HttpProgress(null, Math.Clamp(progressValue, 0f, 1f)));
                }
#endif
            } while (bytesRead > 0);
            progress?.Report(new HttpProgress(null, 1));
        }

        public Uri GetServiceUrl()
        {
            return new Uri("");
        }

        T[] ListItems<T>(List<T> itemList, Pagination pagination)
        {
            T[] resultArray = null;
            if (!string.IsNullOrEmpty(pagination.SortingField))
            {
                Func<T, object> func = p =>
                {
                    var t = typeof(T);
                    var pr = t.GetProperty(pagination.SortingField);
                    var val = pr?.GetValue(p);
                    return val;
                };
                switch (pagination.SortingOrder)
                {
                    case Pagination.Order.Ascending:
                        resultArray = itemList.OrderBy(func).ToArray();
                        break;
                    case Pagination.Order.Descending:
                        resultArray = itemList.OrderByDescending(func).ToArray();
                        break;
                    default:
                        resultArray = itemList.ToArray();
                        break;
                }
            }
            else
            {
                resultArray = itemList.ToArray();
            }

            try
            {
                resultArray = resultArray[pagination.Range];
            }
            catch (ArgumentOutOfRangeException)
            {
                resultArray = itemList.ToArray();
            }

            return resultArray;
        }

    }
}
#endif
