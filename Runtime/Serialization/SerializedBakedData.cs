using System.Collections.Generic;
using System;

namespace Scellecs.Morpeh.EntityConverter.Serialization
{
    [Serializable]
    internal struct SerializedBakedData
    {
        public byte[] serializedData;
        public List<UnityEngine.Object> unityObjects;

        public bool IsValid() => serializedData != null && serializedData.Length > 0 && unityObjects != null;
    }
}
