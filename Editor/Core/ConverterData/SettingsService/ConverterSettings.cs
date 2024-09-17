using Scellecs.Morpeh.EntityConverter.Editor.Baking;
using Scellecs.Morpeh.EntityConverter.Logs;
using System;

namespace Scellecs.Morpeh.EntityConverter.Editor.Settings
{
    [Serializable]
    internal sealed class ConverterSettings
    { 
        public BakingFlags bakingFlags;
        public LogDepthFlags logDepthFlags;
    }
}
