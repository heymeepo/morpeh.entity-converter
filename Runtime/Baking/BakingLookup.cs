#if UNITY_EDITOR
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

namespace Scellecs.Morpeh.EntityConverter
{
    internal struct BakingLookup
    {
        public Dictionary<int, int> instanceIdToIndex;
        public Dictionary<int, int> instanceIdToParentIndex;
        public List<ConvertToEntity> instances;

        public unsafe Entity CreateEntityFromLink(EntityLink link)
        {
            if (link.convertToEntity == null)
            {
                return default;
            }

            var instanceId = link.convertToEntity.gameObject.GetInstanceID();
            var index = instanceIdToIndex[instanceId];
            var value = ((long)(index & 0xFFFFFFFF) << 32) | ((long)(1 & 0xFFFF) << 16);
            var ent = default(Entity);
            var ptr = (long*)&ent;
            UnsafeUtility.MemSet(ptr, 0, sizeof(long));
            *ptr = value;

            return ent;
        }
    }
}
#endif
