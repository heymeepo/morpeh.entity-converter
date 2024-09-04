using Scellecs.Morpeh.Workaround;
using System.Collections.Generic;
using System;
using Unity.Collections.LowLevel.Unsafe;
using Scellecs.Morpeh.Transforms;
using Unity.Mathematics;

namespace Scellecs.Morpeh.EntityConverter
{
    public unsafe class EntityFactory : IDisposable
    {
        public bool IsDisposed { get; private set; }

        private int entitiesCount;
        private int[] rootIndices;
        private EntityParentingInfo[] parentingInfo;
        private SetComponentDescriptor[] componentsDesc;
        private List<ResolveEntityDescriptor> entityResolveDesc;

        public EntityFactory(BakedDataAsset bakedData) => DeserializeAndExpand(bakedData);

        /// <summary>
        /// Creates a hierarchy of entities based on the baked data
        /// </summary>
        /// <returns>
        /// A span of entities representing the hierarchy roots, but not the full hierarchy. If no roots are present, an empty span is returned.
        /// </returns>
        public Span<Entity> Create(World world)
        {
            Span<Entity> entities = stackalloc Entity[entitiesCount];

            for (int i = 0; i < entities.Length; i++)
            {
                entities[i] = world.CreateEntity();
            }

            for (int i = 0; i < parentingInfo.Length; i++)
            {
                var info = parentingInfo[i];
                var child = entities[info.childIndex];
                var parent = entities[info.parentIndex];
                child.SetParent(parent);
            }

            for (int i = 0; i < entityResolveDesc.Count; i++)
            {
                var desciptor = entityResolveDesc[i];
                var entity = entities[desciptor.entityIndex];
                UnsafeUtility.MemCpy(desciptor.dstPtr, &entity, UnsafeUtility.SizeOf<Entity>());
            }

            for (int i = 0; i < componentsDesc.Length; i++)
            {
                ref var descriptor = ref componentsDesc[i];
                var entity = entities[descriptor.entityIndex];
                MorpehInternalTools.SetComponentUnsafe(entity, descriptor.typeId, descriptor.srcPtr, descriptor.size);
            }

            Span<Entity> roots = stackalloc Entity[rootIndices.Length];

            for (int i = 0; i < rootIndices.Length; i++)
            {
                int index = rootIndices[i];
                roots[i] = entities[index];
            }
#pragma warning disable 9080
            return roots;
#pragma warning restore 9080
        }

        public Span<Entity> CreateAt(World world, float3 position, quaternion rotation)
        {
            var roots = Create(world);

            for (int i = 0; i < roots.Length; i++)
            {
                var ent = roots[i];
#pragma warning disable 0618
                ref var transform = ref ent.GetComponent<LocalTransform>();
#pragma warning restore 0618
                transform.position += position;
                transform.rotation = math.mul(transform.rotation, rotation);
            }

            return roots;
        }

        public void Dispose()
        {
            if (IsDisposed == false)
            {
                for (int i = 0; i < componentsDesc.Length; i++)
                {
                    ref var decriptor = ref componentsDesc[i];
                    decriptor.srcPtr = (void*)IntPtr.Zero;
                    UnsafeUtility.ReleaseGCObject(decriptor.handle);
                }

                IsDisposed = true;
            }
        }

        private void DeserializeAndExpand(BakedDataAsset bakedDataAsset)
        {
            var bakedDataList = Scellecs.Morpeh.EntityConverter.Serialization.SerializationUtility.DeserializeBakedData(bakedDataAsset.serializedData);

            int componentsCount = bakedDataAsset.metadata.componentsCount;
            int parentChildPairsCount = bakedDataAsset.metadata.parentChildPairsCount;

            entitiesCount = bakedDataList.Count;
            parentingInfo = new EntityParentingInfo[parentChildPairsCount];
            componentsDesc = new SetComponentDescriptor[componentsCount];
            entityResolveDesc = new List<ResolveEntityDescriptor>();

            int componentsCounter = 0;
            int parentingCounter = 0;
            int rootsCounter = 0;

            for (int i = 0; i < entitiesCount; i++)
            {
                var bakedData = bakedDataList[i];

                for (int j = 0; j < bakedData.components.Length; j++)
                {
                    var componentData = bakedData.components[j];
                    UnsafeUtility.PinGCObjectAndGetAddress(componentData, out var handle);
                    var ptr = componentData.GetDataAddress();
                    var info = componentData.GetInfo();
                    var entityMapInfo = info.entityMapInfo;

                    componentsDesc[componentsCounter++] = new SetComponentDescriptor()
                    {
                        srcPtr = ptr,
                        handle = handle,
                        typeId = info.typeId,
                        size = info.size,
                        entityIndex = i
                    };

                    if (info.entityMapInfo.IsValid)
                    {
                        for (int k = 0; k < entityMapInfo.Count; k++)
                        {
                            var localOffset = entityMapInfo[k];
                            var dst = (byte*)ptr + localOffset;
                            var packedData = *(Entity*)dst;

                            //The baker assigns the local index of the entity within the configuration asset to its Id.
                            //Also sets the Generation to 1 if any entity was assigned from the editor.
                            //Otherwise, the default value is stored.
                            if (packedData.Generation == 1)
                            {
                                var entityIndex = packedData.Id;
                                entityResolveDesc.Add(new ResolveEntityDescriptor()
                                {
                                    dstPtr = dst,
                                    entityIndex = entityIndex
                                });
                            }
                        }
                    }
                }

                if (bakedData.parentIndex >= 0)
                {
                    parentingInfo[parentingCounter++] = new EntityParentingInfo()
                    {
                        parentIndex = bakedData.parentIndex,
                        childIndex = i
                    };
                }
                else
                {
                    rootIndices[rootsCounter++] = i;
                }
            }
        }
    }
}
