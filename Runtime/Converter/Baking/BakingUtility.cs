#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scellecs.Morpeh.EntityConverter
{
    internal static class BakingUtility
    {
        public static IEnumerable<ConvertToEntity> GetAllSceneRoots(Scene scene)
        {
            return scene.GetRootGameObjects().Select(x => x.GetComponent<ConvertToEntity>()).Where(c => c != null && c.excludeFromScene == false);
        }

        public static void TraverseHierarchy(ConvertToEntity current, ref BakingLookup lookup, ref int currentIndex, int parentIndex)
        {
            var instanceId = current.gameObject.GetInstanceID();

            lookup.instanceIdToIndex[instanceId] = currentIndex;
            lookup.instanceIdToParentIndex[instanceId] = parentIndex;
            lookup.instances.Add(current);

            int currentInstanceIndex = currentIndex;
            currentIndex++;

            foreach (Transform child in current.transform)
            {
                var childComponent = child.GetComponent<ConvertToEntity>();

                if (childComponent != null && childComponent.excludeFromScene == false)
                {
                    TraverseHierarchy(childComponent, ref lookup, ref currentIndex, currentInstanceIndex);
                }
            }
        }
    }
}
#endif
