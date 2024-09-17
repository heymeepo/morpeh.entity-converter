#if UNITY_EDITOR
using System;

namespace Scellecs.Morpeh.EntityConverter.Logs
{
    [Flags]
    internal enum LogFlags
    { 
        InternalDebug = 1 << 1,
        Debug = 1 << 2,
        Info = 1 << 3,
        Regular = 1 << 4,
        Fatal = 1 << 5,
    }
}
#endif
