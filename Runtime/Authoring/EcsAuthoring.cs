#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter
{
    public abstract class EcsAuthoring : MonoBehaviour
    {
        [field: SerializeField, HideInInspector] 
        public bool Unparent { get; protected set; } = false;

        [NonSerialized]
        internal List<SetComponentWrapper> components;

        public abstract void Bake();

        protected void SetComponent<T>() where T : struct, IComponent
        {
            if (components == null)
            {
                return;
            }

            components.Add(new SetComponentWrapper<T>()
            {
                data = default,
                typeId = GetComponentTypeId<T>(),
                dstSize = UnsafeUtility.SizeOf<T>()
            });
        }

        protected void SetComponent<T>(T data) where T : struct, IComponent
        {
            if (components == null)
            {
                return;
            }

            components.Add(new SetComponentWrapper<T>()
            { 
                data = data,
                typeId = GetComponentTypeId<T>(),
                dstSize = UnsafeUtility.SizeOf<T>()
            });
        }

        protected void SetComoponentDataUnsafe<T>(T data, int typeId) where T : struct
        {
            if (components == null)
            {
                return;
            }

            components.Add(new SetComponentWrapper<T>()
            {
                data = data,
                typeId = typeId,
                dstSize = UnsafeUtility.SizeOf<T>()
            });
        }

        protected int GetComponentTypeId<T>() where T : struct, IComponent
        {
            return ComponentsTypeCache.GetTypeId(typeof(T));
        }

        protected int GetComponentTypeId(Type componentType)
        {
            return ComponentsTypeCache.GetTypeId(componentType);
        }

        protected internal bool IsPrimaryRoot() => transform.parent == null || transform.parent.GetComponentsInParent<EcsAuthoring>().Length == 0;

        protected internal bool FindClosestInHierarchy<T>(out T component, bool includeThis = true) where T : EcsAuthoring
        {
            var current = includeThis ? transform : transform.parent;

            while (current != null)
            {
                component = current.GetComponent<T>();
                if (component != null)
                {
                    return true;
                }
                current = current.parent;
            }

            component = default;
            return false;
        }

        protected internal bool FindTopmostInHierarchy<T>(out T component) where T : EcsAuthoring
        {
            component = null;
            var current = transform;

            while (current != null)
            {
                var currentComponent = current.GetComponent<T>();
                if (currentComponent != null)
                {
                    component = currentComponent;
                }
                current = current.parent;
            }

            return component != null;
        }
    }
}
#endif
