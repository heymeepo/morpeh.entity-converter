using Scellecs.Morpeh.Workaround;
using System;
using System.ComponentModel;
using Unity.Collections.LowLevel.Unsafe;

namespace Scellecs.Morpeh.EntityConverter
{
    [Serializable]
    internal abstract class SetComponentWrapper
    {
        public abstract void SetToEntity(Entity entity, World world);
    }

    [Serializable]
    internal unsafe sealed class SetComponentWrapper<T> : SetComponentWrapper where T : struct
    {
        public T data;
        public int typeId;
        public int dstSize;

        public override void SetToEntity(Entity entity, World world)
        {
            var srcPtr = UnsafeUtility.AddressOf(ref data);
            MorpehInternalTools.SetComponentUnsafe(entity, typeId, srcPtr, dstSize);
        }
    }
}
