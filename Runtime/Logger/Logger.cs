#if UNITY_EDITOR
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter.Logs
{
    internal sealed class Logger : ILogger
    {
        private const LogDepthFlags DEFAULT_LOG_DEPTH_FLAGS = LogDepthFlags.Fatal | LogDepthFlags.Regular;

        private LogDepthFlags depthFlags;

        public Logger() => depthFlags = DEFAULT_LOG_DEPTH_FLAGS | LogDepthFlags.InternalDebug | LogDepthFlags.Debug | LogDepthFlags.Info;

        public void Log(string message, LogDepthFlags depth)
        {
            if (CheckLogDepthFlags(depth))
            {
                Debug.Log(message);
            }
        }

        public void LogWarning(string message, LogDepthFlags depth)
        {
            if (CheckLogDepthFlags(depth))
            {
                Debug.LogWarning(message);
            }
        }

        public void LogError(string message, LogDepthFlags depth) 
        {
            if (CheckLogDepthFlags(depth))
            {
                Debug.LogError(message);
            }
        }

        public void LogInitializationSuccess<T>()
        {
            Log($"{typeof(T).Name}: Successfully intialized.", LogDepthFlags.InternalDebug);
        }

        public void LogInitializationFailedDataAssetNotLoaded<T>()
        {
            LogError($"{typeof(T).Name}: Initialization failed because the EntityConverterDataAsset was not loaded.", LogDepthFlags.Fatal);
        }

        public void LogInitializationFailedWithException<T>(System.Exception exception)
        {
            LogError($"{typeof(T).Name}: Initialization failed with exception {exception.Message}.", LogDepthFlags.Fatal);
        }

        public void SetLogFlags(LogDepthFlags depthFlags) => this.depthFlags = DEFAULT_LOG_DEPTH_FLAGS | depthFlags;

        private bool CheckLogDepthFlags(LogDepthFlags depth) => (depthFlags & depth) != 0;
    }
}
#endif
