#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

namespace Scellecs.Morpeh.EntityConverter
{
    public ref struct BakingContext
    {
        private List<SetComponentData> components;
        private BakingLookup lookup;

        internal BakingContext(List<SetComponentData> components, BakingLookup lookup)
        {
            this.components = components;
            this.lookup = lookup;
        }

        public Entity GetEntityFromLink(EntityLink link) => lookup.CreateEntityFromLink(link);

        public void SetComponent<T>(T component) where T : struct, IComponent => components.Add(new SetComponentData<T>() { data = component });

        public unsafe void SetComponentUnsafe<T>(Type componentType, T data) where T : struct
        {
            var type = typeof(SetComponentData<>).MakeGenericType(componentType);
            var instance = (SetComponentData)Activator.CreateInstance(type);
            var dataPtr = UnsafeUtility.AddressOf(ref data);
            UnsafeUtility.MemCpy(instance.GetDataAddress(), dataPtr, UnsafeUtility.SizeOf<T>());
        }
    }
}
#endif
