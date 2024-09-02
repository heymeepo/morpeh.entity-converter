using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter
{
    public abstract class BakedDataAsset : ScriptableObject
    {
        [SerializeField]
        internal SerializedBakedData serializedData;

        [SerializeField]
        internal BakedMetadata metadata;
    }
}
