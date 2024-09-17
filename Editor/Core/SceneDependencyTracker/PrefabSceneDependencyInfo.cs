using Scellecs.Morpeh.EntityConverter.Editor.Collections;
using System;

namespace Scellecs.Morpeh.EntityConverter.Editor
{
    [Serializable]
    internal class PrefabSceneDependencyInfo
    {
        public SerializableDictionary<string, int> refCountPerScene;
    }
}
