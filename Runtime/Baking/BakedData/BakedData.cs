using System;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter
{
    [Serializable]
    public sealed class BakedData
    {
        [SerializeField]
        internal SetComponentData[] components;

        [SerializeField, HideInInspector]
        internal int parentIndex = -1;
    }
}
