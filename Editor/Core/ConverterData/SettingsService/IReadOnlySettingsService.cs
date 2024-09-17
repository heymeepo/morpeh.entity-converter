using Scellecs.Morpeh.EntityConverter.Editor.Baking;
using Scellecs.Morpeh.EntityConverter.Logs;

namespace Scellecs.Morpeh.EntityConverter.Editor.Settings
{
    internal interface IReadOnlySettingsService
    {
        public bool GetBakingFlagEnabled(BakingFlags flag);

        public BakingFlags GetBakingFlags();

        public bool GetLogFlagEnabled(LogFlags flag);

        public LogFlags GetLogFlags();
    }
}