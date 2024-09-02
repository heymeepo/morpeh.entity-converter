namespace Scellecs.Morpeh.EntityConverter
{
    internal unsafe struct SetComponentDescriptor
    {
        public void* srcPtr;
        public ulong handle;
        public int typeId;
        public int size;
        public int entityIndex;
    }
}
