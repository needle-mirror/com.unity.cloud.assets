using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Assets.Samples
{
    static class UnityTask
    {
#if UNITY_WEBGL
        static readonly TimeSpan k_InfiniteDelay = TimeSpan.FromMilliseconds(-1);
        const string k_IntExceptionMessage = "The value needs to be either -1 (signifying an infinite timeout), 0 or a positive integer.";
#endif

        internal static Task Delay(int millisecondsDelay, CancellationToken cancellationToken = default)
        {
#if UNITY_WEBGL
            if (millisecondsDelay < -1)
            {
                throw new ArgumentOutOfRangeException(k_IntExceptionMessage, nameof(millisecondsDelay));
            }
            return SafeDelay(TimeSpan.FromMilliseconds(millisecondsDelay), cancellationToken);
#else
            return Task.Delay(millisecondsDelay, cancellationToken);
#endif
        }

#if UNITY_WEBGL
        static async Task SafeDelay(TimeSpan delay, CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();
            Func<bool> predicate = delay == k_InfiniteDelay ? () => true : () => stopwatch.Elapsed < delay;
            while (predicate())
            {
                ThrowIfCancelled(cancellationToken);
                await Task.Yield();
            }
        }

        static void ThrowIfCancelled(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                throw new TaskCanceledException();
        }
#endif
    }
}
