#if UNITY_EDITOR
using Scellecs.Morpeh.EntityConverter.Logger;
using System;

namespace Scellecs.Morpeh.EntityConverter
{
    [Serializable]
    internal sealed class ConverterSettings
    { 
        public BakingFlags bakingFlags;
        public LogDepthFlags logDepthFlags;
    }
}

#endif
