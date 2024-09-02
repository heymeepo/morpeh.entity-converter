#if UNITY_EDITOR
using System;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter
{
    /// <returns>
    /// Editor-only entity link, valid only within a scene or prefab hierarchy. Intended for pass entities to components during baking stage.
    /// The returned entity cannot be used to set components, as it is just a packed data for deserialization.
    /// </returns>
    [Serializable]
    public struct EntityLink
    {
        [SerializeField]
        internal ConvertToEntity convertToEntity;
    }
}
#endif
