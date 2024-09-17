using Scellecs.Morpeh.EntityConverter.Editor.Baking;
using Scellecs.Morpeh.EntityConverter.Logs;

namespace Scellecs.Morpeh.EntityConverter.Editor.Settings
{
    internal interface IReadOnlySettingsService
    {
        public bool TryGetBakingFlags(out BakingFlags flags);

        public bool TryGetLogDepthFlags(out LogDepthFlags flags);
    }
}