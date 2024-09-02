#if UNITY_EDITOR
using System;

namespace Scellecs.Morpeh.EntityConverter
{
    [Flags]
    internal enum EntityConverterBakingFlags
    { 
        BakeOnDomainReload = 1 << 1,
        BakeOnBuild = 1 << 2,
        BakeOnEnterPlaymode = 1 << 3
    }
}
#endif
