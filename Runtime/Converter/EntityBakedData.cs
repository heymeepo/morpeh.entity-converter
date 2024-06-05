using UnityEngine;
using System;
using System.Collections.Generic;

namespace Scellecs.Morpeh.EntityConverter
{
    [Serializable]
    public sealed class EntityBakedData
    {
        public int ChildsCount => childs?.Count ?? 0;

        [SerializeField]
        private List<SetComponentWrapper> components;

        [SerializeField]
        private List<EntityBakedData> childs;

        public void SetToEntity(Entity entity, World world)
        {
            foreach (var componentData in components)
            {
                componentData.SetToEntity(entity, world);
            }
        }

        public EntityBakedData GetChild(int index) => childs[index];

        internal void SetComponent(SetComponentWrapper component) 
        {
            components ??= new List<SetComponentWrapper>();
            components.Add(component);
        }

        internal void SetChild(EntityBakedData child)
        { 
            childs ??= new List<EntityBakedData>();
            childs.Add(child);
        }
    }
}
