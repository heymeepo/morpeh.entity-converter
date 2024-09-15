#if UNITY_EDITOR
using Scellecs.Morpeh.EntityConverter.Collections;
using System;

namespace Scellecs.Morpeh.EntityConverter
{
    [Serializable]
    internal class PrefabSceneDependencyInfo
    {
        public SerializableDictionary<string, int> refCountPerScene;
    }
}
#endif
