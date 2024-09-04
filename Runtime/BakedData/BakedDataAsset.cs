using System;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter
{
    public abstract class BakedDataAsset : ScriptableObject, IDisposable
    {
        [SerializeField]
        internal SerializedBakedData serializedData;

        [SerializeField]
        internal BakedMetadata metadata;

        [NonSerialized]
        internal EntityFactory factory;

        public EntityFactory GetFactory()
        {
            if (factory == null || factory.IsDisposed)
            {
                factory = new EntityFactory(this);
            }

            return factory;
        }

        public void Dispose()
        {
            if (factory == null || factory.IsDisposed)
            {
                factory.Dispose();
            }
        }
    }
}
