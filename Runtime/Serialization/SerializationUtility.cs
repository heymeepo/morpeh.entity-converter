using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections;
using Unity.Serialization.Binary;

namespace Scellecs.Morpeh.EntityConverter.Serialization
{
    internal static unsafe class SerializationUtility
    {
        public static SerializedBakedData SerializeBakedData(List<EntityBakedData> bakedData)
        {
            if (bakedData == null) 
            {
                return default;
            }

            using var stream = new UnsafeAppendBuffer(16, 8, Allocator.Temp);

            var unityObjectsList = new List<UnityEngine.Object>();
            var parameters = new BinarySerializationParameters
            {
                UserDefinedAdapters = new List<IBinaryAdapter> { new UnityObjectAdapter(unityObjectsList) }
            };

            BinarySerialization.ToBinary(&stream, bakedData, parameters);

            var serializedData = new byte[stream.Length];

            fixed (byte* buffer = &serializedData[0])
            {
                UnsafeUtility.MemCpy(buffer, stream.Ptr, stream.Length);
            }

            return new SerializedBakedData()
            {
                serializedData = serializedData,
                unityObjects = unityObjectsList
            };
        }

        public static List<EntityBakedData> DeserializeBakedData(SerializedBakedData serializedData) 
        {
            if (serializedData.IsValid() == false)
            {
                return null;
            }

            var parameters = new BinarySerializationParameters
            {
                UserDefinedAdapters = new List<IBinaryAdapter> { new UnityObjectAdapter(serializedData.unityObjects, true) }
            };

            fixed (byte* buffer = &serializedData.serializedData[0])
            {
                var streamReader = new UnsafeAppendBuffer.Reader(buffer, serializedData.serializedData.Length);
                return BinarySerialization.FromBinary<List<EntityBakedData>>(&streamReader, parameters);
            }
        }
    }
}
