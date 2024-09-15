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
        public int RootEntitiesCount => rootIndices.Length;

        private int[] rootIndices;
        private Entity[] entities;
        private EntityParentingInfo[] parentingInfo;
        private SetComponentDescriptor[] componentsDesc;
        private List<ResolveEntityDescriptor> entityResolveDesc;

        internal EntityFactory(BakedDataAsset bakedData) => DeserializeAndExpand(bakedData);

        public void Create(World world)
        {
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
        }

        public void Create(World world, Span<Entity> roots)
        {
            if (roots.Length < RootEntitiesCount)
            { 
                //exception
            }

            Create(world);

            for (int i = 0; i < RootEntitiesCount; i++)
            {
                int index = rootIndices[i];
                roots[i] = entities[index];
            }
        }

        public void CreateAt(World world, float3 position, quaternion rotation)
        {
            Span<Entity> roots = stackalloc Entity[RootEntitiesCount];
            CreateAt(world, position, rotation, world.GetStash<LocalTransform>(), roots);
        }

        public void CreateAt(World world, float3 position, quaternion rotation, Stash<LocalTransform> transformStash)
        {
            Span<Entity> roots = stackalloc Entity[RootEntitiesCount];
            CreateAt(world, position, rotation, transformStash, roots);
        }

        public void CreateAt(World world, float3 position, quaternion rotation, Stash<LocalTransform> transformStash, Span<Entity> roots)
        {
            Create(world, roots);

            for (int i = 0; i < roots.Length; i++)
            {
                var ent = roots[i];
                ref var transform = ref transformStash.Get(ent);
                transform.position += position;
                transform.rotation = math.mul(transform.rotation, rotation);
            }
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

            entities = new Entity[bakedDataList.Count];
            rootIndices = new int[entities.Length - parentChildPairsCount];
            parentingInfo = new EntityParentingInfo[parentChildPairsCount];
            componentsDesc = new SetComponentDescriptor[componentsCount];
            entityResolveDesc = new List<ResolveEntityDescriptor>();

            int componentsCounter = 0;
            int parentingCounter = 0;
            int rootsCounter = 0;

            for (int i = 0; i < entities.Length; i++)
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
