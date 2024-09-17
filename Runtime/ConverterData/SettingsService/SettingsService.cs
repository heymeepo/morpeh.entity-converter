#if UNITY_EDITOR
using Scellecs.Morpeh.EntityConverter.Logs;
using System;
using UnityEditor;

namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class SettingsService : IReadOnlySettingsService
    {
        private const string TEMP_BAKING_FLAGS_KEY = "__EC_TEMP_BAKING_FLAGS";

        private readonly IEntityConverterDataProvider dataProvider;
        private readonly Logger logger;

        public SettingsService(IEntityConverterDataProvider dataProvider, Logger logger)
        {
            this.dataProvider = dataProvider;
            this.logger = logger;
        }

        public void Initialize()
        {
            if (dataProvider.TryGetData(out var data))
            {
                var settings = data.ConverterSettings;
                //logger.SetLogFlags(settings.logDepthFlags);
            }
            else
            {
                logger.LogInitializationFailedDataAssetNotLoaded<SettingsService>();
            }
        }

        public bool TryGetLogDepthFlags(out LogDepthFlags flags)
        {
            if (dataProvider.TryGetData(out var data))
            {
                flags = data.ConverterSettings.logDepthFlags;
                return true;
            }

            flags = default;
            return false;
        }

        public bool TryGetBakingFlags(out BakingFlags flags)
        {
            var temp = SessionState.GetInt(TEMP_BAKING_FLAGS_KEY, int.MaxValue);

            if(temp != int.MaxValue) 
            {
                flags = (BakingFlags)(byte)temp;
                return true;
            }

            if (dataProvider.TryGetData(out var data))
            {
                flags = data.ConverterSettings.bakingFlags;
                return true;
            }

            flags = default;
            return false;
        }

        public void SetLogDepthFlags(LogDepthFlags flags)
        {
            if (dataProvider.TryGetData(out var data))
            {
                data.ConverterSettings.logDepthFlags = flags;
                logger.SetLogFlags(flags);
            }
        }

        public void SetBakingFlags(BakingFlags flags)
        {
            if (dataProvider.TryGetData(out var data))
            {
                data.ConverterSettings.bakingFlags = flags;
            }
        }

        public void SetTemporaryBakingFlags(BakingFlags flags)
        {
            SessionState.SetInt(TEMP_BAKING_FLAGS_KEY, (int)flags);
            logger.Log($"Temp BakingFlags were set. Value: {Convert.ToString((byte)flags, 2).PadLeft(8, '0')} ", LogDepthFlags.InternalDebug);
        }

        public void ClearTemporaryBakingFlags()
        {
            SessionState.SetInt(TEMP_BAKING_FLAGS_KEY, int.MaxValue);
            logger.Log("Temp BakingFlags were cleared.", LogDepthFlags.InternalDebug);
        }
    }
}
#endif
