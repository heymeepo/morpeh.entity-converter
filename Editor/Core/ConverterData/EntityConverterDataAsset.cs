﻿using Scellecs.Morpeh.EntityConverter.Editor.Collections;
using Scellecs.Morpeh.EntityConverter.Editor.Settings;
using System.Collections.Generic;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter.Editor
{
    internal sealed class EntityConverterDataAsset : ScriptableObject
    {
        [field: SerializeField]
        public SerializableHashSet<string> AuthoringPrefabGUIDs { get; private set; } = new SerializableHashSet<string>();

        [field: SerializeField]
        public SerializableHashSet<string> SceneGUIDs { get; private set; } = new SerializableHashSet<string>();

        [field: SerializeField]
        public SerializableDictionary<string, SceneBakedDataAsset> SceneBakedDataAssets { get; private set; } = new SerializableDictionary<string, SceneBakedDataAsset>();

        [field: SerializeField]
        public SerializableDictionary<string, PrefabSceneDependencyInfo> PrefabToSceneDependencies { get; private set; } = new SerializableDictionary<string, PrefabSceneDependencyInfo>();

        [field: SerializeField]
        public SerializableDictionary<string, AssetGUIDInfo> AssetGUIDInfos { get; private set; } = new SerializableDictionary<string, AssetGUIDInfo>();

        [field: SerializeField]
        public List<ScriptableObject> UserContext { get; private set; }

        [field: SerializeField]
        public ConverterSettings ConverterSettings { get; private set; }
    }
}