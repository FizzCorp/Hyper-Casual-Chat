using System;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#else
using System.Diagnostics;
#endif

namespace Fizz.Common
{
    public enum FizzLogType
    {
        Debug   = 0,
        Warning = 1,
        Error   = 2
    }

    public class FizzLogger
    {
        private const string TAG = "Fizz: ";
        private static FizzLogType s_logLevel = FizzLogType.Debug;
        private static Action<FizzLogType, string> s_log;

        public static void SetLogLevel(FizzLogType logLevel)
        {
            s_logLevel = logLevel;
        }

        public static void SetLogger(Action<FizzLogType, string> log)
        {
            s_log = log;
        }

        public static void D(string msg)
        {
            if (s_logLevel <= FizzLogType.Debug)
            {
                if (s_log != null)
                {
                    s_log(FizzLogType.Debug, TAG + msg);
                }
                else
                {
#if UNITY_5_3_OR_NEWER
                    Debug.Log(TAG + msg);
#else
                    Debug.Write(TAG + "[DEBUG] " + msg);
#endif
                }
            }
        }

        public static void W(string msg)
        {
            if (s_logLevel <= FizzLogType.Warning)
            {
                if (s_log != null)
                {
                    s_log(FizzLogType.Warning, TAG + msg);
                }
                else
                {
#if UNITY_5_3_OR_NEWER
                    Debug.LogWarning(TAG + msg);
#else
                    Debug.Write(TAG + "[WARN] " + msg);
#endif
                }
            }
        }

        public static void E(string msg)
        {
            if (s_logLevel <= FizzLogType.Error)
            {
                if (s_log != null)
                {
                    s_log(FizzLogType.Error, TAG + msg);
                }
                else
                {
#if UNITY_5_3_OR_NEWER
                    Debug.LogError(TAG + msg);
#else
                    Debug.Write(TAG + "[ERROR] " + msg);
#endif
                }
            }
        }

        public static void E(Exception e)
        {
            if (s_logLevel <= FizzLogType.Error)
            {
                if (s_log != null)
                {
                    s_log(FizzLogType.Error, TAG + e.Message);
                }
                else
                {
#if UNITY_5_3_OR_NEWER
                    Debug.LogError(e);
#else
                    Debug.Write(TAG + "[ERROR] " + e.Message);
#endif
                }
            }
        }
    }
}
