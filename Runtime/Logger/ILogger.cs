#if UNITY_EDITOR
namespace Scellecs.Morpeh.EntityConverter.Logs
{
    internal interface ILogger
    {
        public void Log(string message, LogFlags depth);

        public void LogWarning(string message, LogFlags depth);

        public void LogError(string message, LogFlags depth);

        public void LogInitializationSuccess<T>();

        public void LogInitializationFailedDataAssetNotLoaded<T>();

        public void LogInitializationFailedWithException<T>(System.Exception exception);
    }
}
#endif
