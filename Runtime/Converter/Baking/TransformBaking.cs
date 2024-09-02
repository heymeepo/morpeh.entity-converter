#if UNITY_EDITOR
using Scellecs.Morpeh.Transforms;
using Unity.Mathematics;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter
{
    internal static class TransformBaking
    {
        public static void BakeTransformGroup(BakingContext context, GameObject source, bool isRoot)
        {
            context.SetComponent(new LocalTransform()
            {
                position = isRoot ? source.transform.localToWorldMatrix.GetPosition() : source.transform.localPosition,
                rotation = isRoot ? source.transform.localToWorldMatrix.rotation : source.transform.localRotation,
                scale = 1f
            });

            context.SetComponent(new LocalToWorld()
            {
                value = source.transform.localToWorldMatrix
            });

            var hasNonIdentityScale = isRoot ? HasNonIdentityScale(source.transform.lossyScale) : HasNonIdentityScale(source.transform.localScale);

            if (hasNonIdentityScale)
            {
                var compositeMatrix = isRoot ? float4x4.Scale(source.transform.lossyScale) : float4x4.Scale(source.transform.localScale);
                context.SetComponent(new PostTransformMatrix()
                {
                    Value = compositeMatrix
                });
            }
        }

        private static bool HasNonIdentityScale(float3 scale) => math.lengthsq(scale - new float3(1f)) > 0f;
    }
}
#endif
