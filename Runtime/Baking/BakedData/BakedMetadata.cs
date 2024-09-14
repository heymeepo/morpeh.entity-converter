using System;

namespace Scellecs.Morpeh.EntityConverter
{
    [Serializable]
    internal struct BakedMetadata
    {
        public int entitiesCount;
        public int componentsCount;
        public int parentChildPairsCount;
    }
}
