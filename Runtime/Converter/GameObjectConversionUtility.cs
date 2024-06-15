#if UNITY_EDITOR
using Scellecs.Morpeh.Transforms;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter
{
    public static class GameObjectConversionUtility
    {
        private static List<GameObject> buffer = new List<GameObject>();

        public static void ExportBakedData(GameObject gameObject, List<EntityBakedData> roots)
        {
            var topLevelRoots = FindTopLevelConverters(gameObject);

            foreach (var root in topLevelRoots)
            {
                ExportBakedData(root, roots, new EntityBakedData(), true, out _);
            }
        }

        private static void ExportBakedData(GameObject gameObject, List<EntityBakedData> roots, EntityBakedData bakedData, bool isRoot, out bool isChild)
        {
            var authorings = gameObject.GetComponents<EcsAuthoring>();
            bool shouldUnparent = false;

            foreach (var authoring in authorings)
            {
                var components = new List<SetComponentWrapper>();
                authoring.components = components;
                authoring.Bake();
                authoring.components = null;

                foreach (var component in components)
                {
                    bakedData.SetComponent(component);
                }

                shouldUnparent |= authoring.Unparent;
            }

            isRoot = isRoot || shouldUnparent;
            isChild = isRoot == false;

            var transformComponents = CreateTransformGroup(gameObject, isRoot);

            foreach (var component in transformComponents)
            {
                bakedData.SetComponent(component);
            }

            foreach (Transform child in gameObject.transform)
            {
                var childData = new EntityBakedData();
                ExportBakedData(child.gameObject, roots, childData, false, out bool isCurrentChild);

                if (isCurrentChild)
                {
                    bakedData.SetChild(childData);
                }
            }

            if (isRoot)
            {
                roots.Add(bakedData);
            }
        }

        private static List<GameObject> FindTopLevelConverters(GameObject startObject)
        {
            buffer.Clear();
            SearchInChildren(startObject, buffer);
            return buffer;
        }

        private static void SearchInChildren(GameObject current, List<GameObject> found)
        {
            if (current.GetComponent<EcsAuthoring>() != null)
            {
                found.Add(current);
                return;
            }

            foreach (Transform child in current.transform)
            {
                SearchInChildren(child.gameObject, found);
            }
        }

        private static List<SetComponentWrapper> CreateTransformGroup(GameObject source, bool isRoot)
        {
            var components = new List<SetComponentWrapper>()
            {
                new SetComponentWrapper<LocalTransform>()
                {
                    data = new LocalTransform()
                    {
                        position = isRoot ? source.transform.localToWorldMatrix.GetPosition() : source.transform.localPosition,
                        rotation = isRoot ? source.transform.localToWorldMatrix.rotation : source.transform.localRotation,
                        scale = 1f
                    },
                    typeId = ComponentsTypeCache.GetTypeId(typeof(LocalTransform)),
                    dstSize = UnsafeUtility.SizeOf<LocalTransform>()
                },

                new SetComponentWrapper<LocalToWorld>()
                {
                    data = new LocalToWorld()
                    {
                        value = source.transform.localToWorldMatrix
                    },
                    typeId = ComponentsTypeCache.GetTypeId(typeof(LocalToWorld)),
                    dstSize = UnsafeUtility.SizeOf<LocalToWorld>()
                }
            };

            var hasNonIdentityScale = isRoot ? HasNonIdentityScale(source.transform.lossyScale) : HasNonIdentityScale(source.transform.localScale);

            if (hasNonIdentityScale)
            {
                var composite = isRoot ? float4x4.Scale(source.transform.lossyScale) : float4x4.Scale(source.transform.localScale);

                components.Add(new SetComponentWrapper<PostTransformMatrix>()
                {
                    data = new PostTransformMatrix() { Value = composite },
                    typeId = ComponentsTypeCache.GetTypeId(typeof(PostTransformMatrix)),
                    dstSize = UnsafeUtility.SizeOf<PostTransformMatrix>()
                });
            }

            return components;
        }

        private static bool HasNonIdentityScale(float3 scale) => math.lengthsq(scale - new float3(1f)) > 0f;
    }
}
#endif
