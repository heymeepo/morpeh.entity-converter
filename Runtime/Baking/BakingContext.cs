#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter
{
    public ref struct BakingContext
    {
        private List<SetComponentData> components;
        private BakingLookup lookup;

        internal bool unparent;

        internal BakingContext(List<SetComponentData> components, BakingLookup lookup)
        {
            this.components = components;
            this.lookup = lookup;
            unparent = false;
        }

        public Entity GetEntityFromLink(EntityLink link)
        {
            var ent = lookup.CreateEntityFromLink(link);

            if (ent == default)
            { 
                //warning
            }

            return ent;
        }

        public void UnparentThis() => unparent = true;

        public void SetComponent<T>(T component) where T : struct, IComponent => components.Add(new SetComponentData<T>() { data = component });

        public unsafe void SetComponentReinterpret<T>(Type componentType, T data) where T : struct
        {
            if (typeof(IComponent).IsAssignableFrom(componentType) && componentType.IsValueType)
            {
                var type = typeof(SetComponentData<>).MakeGenericType(componentType);
                var instance = (SetComponentData)Activator.CreateInstance(type);
                var dataPtr = UnsafeUtility.AddressOf(ref data);
                UnsafeUtility.MemCpy(instance.GetDataAddress(), dataPtr, UnsafeUtility.SizeOf<T>());
                components.Add(instance);
            }
            else
            {
                Debug.LogError($"The specified type {componentType} is not valid. Please ensure that the type you are trying to assign is a value type and implements IComponent interface.");
            }
        }
    }
}
#endif
