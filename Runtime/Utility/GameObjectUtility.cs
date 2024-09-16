#if UNITY_EDITOR
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter.Utilities
{
    public static class GameObjectUtility
    {
        public static bool FindClosestInHierarchy<T>(this GameObject gameObject, out T component, bool includeThis = true) where T : Component
        {
            var current = includeThis ? gameObject.transform : gameObject.transform.parent;

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

        public static bool FindTopmostInHierarchy<T>(this GameObject gameObject, out T component) where T : Component
        {
            component = null;
            var current = gameObject.transform;

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
