using System;
using UnityEngine;

namespace Unity.Cloud.Assets.Samples
{
    public static class LoggingExtensions
    {
        public static void LogException(this OperationCanceledException e, string title = "")
        {
            if (string.IsNullOrEmpty(title))
            {
                Debug.Log(e.Message ?? e.ToString());
            }
            else
            {
                Debug.Log($"{title}: {e.Message ?? e.ToString()}");
            }
        }

        public static void LogException(this Exception e, string title = "")
        {
            while (e != null)
            {
                switch (e)
                {
                    case AggregateException {InnerException: not null} ae:
                        e = ae.InnerException;
                        continue;
                    case OperationCanceledException oce:
                        oce.LogException(title);
                        break;
                    default:
                        if (string.IsNullOrEmpty(title))
                        {
                            Debug.LogException(e);
                        }
                        else
                        {
                            Debug.LogError($"Exception raised by {title}\n" + e.Message);
                        }

                        break;
                }

                break;
            }
        }
    }
}
