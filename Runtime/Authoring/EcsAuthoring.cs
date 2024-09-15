#if UNITY_EDITOR
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter
{
    public abstract class EcsAuthoring : MonoBehaviour
    {
        public EntityLink GetEntityLink() => GetComponent<ConvertToEntity>().GetEntityLink();

        public bool IsValidForConversion() => GetComponent<ConvertToEntity>() != null;

        public virtual void OnBeforeBake(UserContext userContext) { }

        public abstract void OnBake(BakingContext bakingContext, UserContext userContext);

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