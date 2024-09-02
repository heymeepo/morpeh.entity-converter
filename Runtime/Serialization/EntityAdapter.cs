using Unity.Serialization.Binary;

namespace Scellecs.Morpeh.EntityConverter.Serialization
{
    internal unsafe sealed class EntityAdapter : IBinaryAdapter<Entity>
    {
        public Entity Deserialize(in BinaryDeserializationContext<Entity> context) => context.Reader->ReadNext<Entity>();

        public void Serialize(in BinarySerializationContext<Entity> context, Entity value) => context.Writer->Add(value);
    }
}
