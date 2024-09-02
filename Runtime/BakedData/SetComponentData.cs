using Scellecs.Morpeh.Workaround;
using System;
using Unity.Collections.LowLevel.Unsafe;

namespace Scellecs.Morpeh.EntityConverter
{
    [Serializable]
    internal abstract class SetComponentData
    {
        public abstract int GetTypeId();

        public abstract int GetSize();

        public unsafe abstract void* GetDataAddress();

        public abstract EntityMapInfo GetEntityMapInfo();
    }

    [Serializable]
    internal sealed class SetComponentData<T> : SetComponentData where T : struct, IComponent
    {
        public T data;

        public override int GetTypeId() => MorpehInternalTools.GetTypeId(typeof(T));

        public override int GetSize() => UnsafeUtility.SizeOf<T>();

        public unsafe override void* GetDataAddress() => UnsafeUtility.AddressOf(ref data);

        public override EntityMapInfo GetEntityMapInfo() => MorpehInternalTools.GetEntityMapInfoForComponentType(typeof(T));
    }
}
