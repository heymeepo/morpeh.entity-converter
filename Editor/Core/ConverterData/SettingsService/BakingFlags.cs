using System;

namespace Scellecs.Morpeh.EntityConverter.Editor.Baking
{
    [Flags]
    internal enum BakingFlags : byte
    { 
        BakeOnDomainReload = 1 << 1,
        BakePrefabs = 1 << 2,
        BakeScenes = 1 << 3,
    }
}
