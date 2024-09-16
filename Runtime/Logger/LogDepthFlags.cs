﻿#if UNITY_EDITOR
using System;

namespace Scellecs.Morpeh.EntityConverter.Logger
{
    [Flags]
    internal enum LogDepthFlags
    { 
        InternalDebug = 1 << 1,
        Debug = 1 << 2,
        Info = 1 << 3,
        Regular = 1 << 4,
        Fatal = 1 << 5,
    }
}
#endif
