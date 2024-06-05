using UnityEngine;
using System;
using System.Collections.Generic;
using Scellecs.Morpeh.EntityConverter.Serialization;

namespace Scellecs.Morpeh.EntityConverter
{
    [PreferBinarySerialization]
    [CreateAssetMenu(fileName = "EntityBakedData", menuName = "ECS/Baker/EntityBakedDataAsset")]
    public sealed class EntityBakedDataAsset : ScriptableObject, ISerializationCallbackReceiver
    {
        public int RootsCount => bakedRoots?.Count ?? 0;

        [NonSerialized]
        private List<EntityBakedData> bakedRoots;

        [SerializeField]
        private SerializedBakedData serializedData;

        public void OnAfterDeserialize() => bakedRoots = SerializationUtility.DeserializeBakedData(serializedData);

        public void OnBeforeSerialize() { }

        public EntityBakedData GetRoot(int index) => bakedRoots?[index] ?? default;

        internal void SetSerializedData(SerializedBakedData serializedData) => this.serializedData = serializedData;
    }
}
