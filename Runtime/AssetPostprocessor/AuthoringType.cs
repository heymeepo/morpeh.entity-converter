﻿#if UNITY_EDITOR
namespace Scellecs.Morpeh.EntityConverter
{
    internal enum AuthoringType
    { 
        None = 0,
        Scene = 1,
        Prefab = 2,
        SceneBakedData = 3,
        PrefabBakedData = 4,
    }
}
#endif