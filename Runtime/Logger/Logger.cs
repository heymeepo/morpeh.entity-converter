#if UNITY_EDITOR
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter.Logs
{
    internal sealed class Logger : ILogger
    {
        public const LogFlags DEFAULT_LOG_FLAGS = LogFlags.Fatal | LogFlags.Regular;

        private LogFlags flags;

        public Logger() => flags = DEFAULT_LOG_FLAGS;

        public void Log(string message, LogFlags depth)
        {
            if (CheckLogFlags(depth))
            {
                Debug.Log(message);
            }
        }

        public void LogWarning(string message, LogFlags depth)
        {
            if (CheckLogFlags(depth))
            {
                Debug.LogWarning(message);
            }
        }

        public void LogError(string message, LogFlags depth) 
        {
            if (CheckLogFlags(depth))
            {
                Debug.LogError(message);
            }
        }

        public void LogInitializationSuccess<T>()
        {
            Log($"{typeof(T).Name}: Successfully intialized.", LogFlags.InternalDebug);
        }

        public void LogInitializationFailedDataAssetNotLoaded<T>()
        {
            LogError($"{typeof(T).Name}: Initialization failed because the EntityConverterDataAsset was not loaded.", LogFlags.Fatal);
        }

        public void LogInitializationFailedWithException<T>(System.Exception exception)
        {
            LogError($"{typeof(T).Name}: Initialization failed with exception {exception.Message}.", LogFlags.Fatal);
        }

        public void SetLogFlags(LogFlags flags) => this.flags = DEFAULT_LOG_FLAGS | flags;

        private bool CheckLogFlags(LogFlags depth) => (flags & depth) != 0;
    }
}
#endif
