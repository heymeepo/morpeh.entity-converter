using Scellecs.Morpeh.Workaround;
using System;
using Unity.Collections.LowLevel.Unsafe;

namespace Scellecs.Morpeh.EntityConverter
{
    [Serializable]
    internal abstract class SetComponentData
    {
        public abstract ComponentDataInfo GetInfo();

        public unsafe abstract void* GetDataAddress();
    }

    [Serializable]
    internal sealed class SetComponentData<T> : SetComponentData where T : struct, IComponent
    {
        public T data;

        public override ComponentDataInfo GetInfo()
        {
            return new ComponentDataInfo()
            {
                typeId = MorpehInternalTools.GetTypeId(typeof(T)),
                size = UnsafeUtility.SizeOf<T>(),
                entityMapInfo = MorpehInternalTools.GetEntityMapInfoForComponentType(typeof(T))
            };
        }

        public unsafe override void* GetDataAddress() => UnsafeUtility.AddressOf(ref data);
    }
}
