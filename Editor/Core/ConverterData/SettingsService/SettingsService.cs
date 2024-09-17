using Scellecs.Morpeh.EntityConverter.Editor.Baking;
using Scellecs.Morpeh.EntityConverter.Logs;
using System;
using UnityEditor;

namespace Scellecs.Morpeh.EntityConverter.Editor.Settings
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

        public void Reload()
        {
            if (dataProvider.TryGetData(out var data))
            {
                var settings = data.ConverterSettings;
                logger.SetLogFlags(settings.logFlags);
            }
        }

        public LogFlags GetLogFlags()
        {
            if (dataProvider.TryGetData(out var data))
            {
                return data.ConverterSettings.logFlags | Logger.DEFAULT_LOG_FLAGS;
            }

            return Logger.DEFAULT_LOG_FLAGS;
        }

        public bool GetLogFlagEnabled(LogFlags flag)
        {
            if (dataProvider.TryGetData(out var data))
            {
                return ((data.ConverterSettings.logFlags | Logger.DEFAULT_LOG_FLAGS) & flag) != 0;
            }

            return (Logger.DEFAULT_LOG_FLAGS & flag) != 0;
        }

        public void SetLogFlagState(LogFlags flag, bool state)
        {
            if (dataProvider.TryGetData(out var data, true))
            {
                var flags = data.ConverterSettings.logFlags;
                data.ConverterSettings.logFlags = state ? flags | flag : flags & ~flag;
                logger.SetLogFlags(data.ConverterSettings.logFlags);
            }
        }

        public void SetLogFlags(LogFlags flags)
        {
            if (dataProvider.TryGetData(out var data, true))
            {
                data.ConverterSettings.logFlags = flags;
                logger.SetLogFlags(flags);
            }
        }

        public BakingFlags GetBakingFlags()
        {
            var temp = SessionState.GetInt(TEMP_BAKING_FLAGS_KEY, int.MaxValue);

            if(temp != int.MaxValue) 
            {
                return (BakingFlags)(byte)temp;
            }

            if (dataProvider.TryGetData(out var data))
            {
                return data.ConverterSettings.bakingFlags;
            }

            return 0;
        }

        public bool GetBakingFlagEnabled(BakingFlags flag)
        {
            if (dataProvider.TryGetData(out var data))
            {
                return (data.ConverterSettings.bakingFlags & flag) != 0;
            }

            return false;
        }

        public void SetBakingFlags(BakingFlags flags)
        {
            if (dataProvider.TryGetData(out var data, true))
            {
                data.ConverterSettings.bakingFlags = flags;
            }
        }

        public void SetBakingFlagState(BakingFlags flag, bool state)
        {
            if (dataProvider.TryGetData(out var data, true))
            {
                var flags = data.ConverterSettings.bakingFlags;
                data.ConverterSettings.bakingFlags = state ? flags | flag : flags & ~flag;
            }
        }

        public void SetTemporaryBakingFlags(BakingFlags flags)
        {
            SessionState.SetInt(TEMP_BAKING_FLAGS_KEY, (int)flags);
            logger.Log($"Temp BakingFlags were set. Value: {Convert.ToString((byte)flags, 2).PadLeft(8, '0')} ", LogFlags.InternalDebug);
        }

        public void ClearTemporaryBakingFlags()
        {
            SessionState.SetInt(TEMP_BAKING_FLAGS_KEY, int.MaxValue);
            logger.Log("Temp BakingFlags were cleared.", LogFlags.InternalDebug);
        }
    }
}
