#if UNITY_EDITOR
using System;

namespace Scellecs.Morpeh.EntityConverter
{
    [Flags]
    internal enum BakingFlags : byte
    { 
        BakeOnDomainReload = 1 << 1,
        BakePrefabs = 1 << 2,
        BakeScenes = 1 << 3,
    }
}
#endif
