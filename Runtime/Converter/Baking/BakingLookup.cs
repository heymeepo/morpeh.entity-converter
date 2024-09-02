#if UNITY_EDITOR
using System.Collections.Generic;

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
                //warning
                return default;
            }

            var instanceId = link.convertToEntity.gameObject.GetInstanceID();
            var index = instanceIdToIndex[instanceId];

            var value = ((long)(index & 0xFFFFFFFF) << 32) | ((long)(1 & 0xFFFF) << 16);
            var entity = new Entity();
            var entPtr = (long*)&entity;
            *entPtr = value;
            return entity;
        }
    }
}
#endif
