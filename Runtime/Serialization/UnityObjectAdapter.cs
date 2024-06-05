using System.Collections.Generic;
using Unity.Serialization.Binary;

namespace Scellecs.Morpeh.EntityConverter.Serialization
{
    internal unsafe sealed class UnityObjectAdapter : IContravariantBinaryAdapter<UnityEngine.Object>
    {
        private Dictionary<int, int> instanceIdMap;
        private List<UnityEngine.Object> unityObjects;

        public UnityObjectAdapter(List<UnityEngine.Object> unityObjects, bool readOnly = false)
        {
            this.unityObjects = unityObjects;

            if (readOnly == false)
            {
                instanceIdMap = new Dictionary<int, int>();
            }
        }

        public void Serialize(IBinarySerializationContext context, UnityEngine.Object value)
        {
            var index = -1;

            if (value != null)
            {
                var instanceId = value.GetInstanceID();

                if (instanceId != 0)
                {
                    if (instanceIdMap.TryGetValue(instanceId, out index) == false)
                    {
                        index = unityObjects.Count;
                        instanceIdMap.Add(instanceId, index);
                        unityObjects.Add(value);
                    }
                }
            }

            context.Writer->Add(index);
        }

        public object Deserialize(IBinaryDeserializationContext context)
        {
            var index = context.Reader->ReadNext<int>();

            if (index == -1)
            {
                return null;
            }

            return unityObjects[index];
        }
    }
}
