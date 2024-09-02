#if UNITY_EDITOR
using Scellecs.Morpeh.EntityConverter.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class EntityConverterDataAsset : ScriptableObject
    {
        [field: SerializeField]
        public SerializableHashSet<string> AuthoringPrefabsGuids { get; private set; } = new SerializableHashSet<string>();

        [field: SerializeField]
        public SerializableDictionary<string, SceneBakedDataAsset> SceneBakedDataAssets { get; private set; } = new SerializableDictionary<string, SceneBakedDataAsset>();

        [field: SerializeField]
        public SerializableHashSet<string> SceneGuids { get; private set; } = new SerializableHashSet<string>();

        [field: SerializeField]
        public List<ScriptableObject> UserContext { get; private set; } = new List<ScriptableObject>();
    }
}
#endif
