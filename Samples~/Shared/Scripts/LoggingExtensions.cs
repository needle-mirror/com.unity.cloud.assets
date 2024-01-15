using System;
using UnityEngine;

namespace Unity.Cloud.Assets.Samples
{
    public static class LoggingExtensions
    {
        public static void LogException(this OperationCanceledException e)
        {
            Debug.Log(e.Message ?? e.ToString());
        }

        public static void LogException(this Exception e)
        {
            while (true)
            {
                switch (e)
                {
                    case AggregateException {InnerException: not null} ae:
                        e = ae.InnerException;
                        continue;
                    case OperationCanceledException oce:
                        oce.LogException();
                        break;
                    default:
                        Debug.LogException(e);
                        break;
                }

                break;
            }
        }
    }
}
