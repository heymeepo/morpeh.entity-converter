#if UNITY_EDITOR
using Scellecs.Morpeh.EntityConverter.Logs;

namespace Scellecs.Morpeh.EntityConverter
{
    internal interface IReadOnlySettingsService
    {
        public bool TryGetBakingFlags(out BakingFlags flags);

        public bool TryGetLogDepthFlags(out LogDepthFlags flags);
    }
}
#endif