#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class BakingProcessor
    {
        private List<ScriptableObject> userContext;

        public void ExecutePrefabBake(PrefabBakingInfo prefabBakingInfo)
        {
            var lookup = new BakingLookup()
            {
                instanceIdToIndex = new Dictionary<int, int>(),
                instanceIdToParentIndex = new Dictionary<int, int>(),
                instances = new List<ConvertToEntity>()
            };

            int currentIndex = 0;
            TraverseHierarchy(prefabBakingInfo.root, ref lookup, ref currentIndex, -1);
            ProcessBake(lookup, prefabBakingInfo.bakedData);
        }

        public void ExecuteSceneBake(SceneBakingInfo sceneBakingInfo)
        {
            var lookup = new BakingLookup()
            {
                instanceIdToIndex = new Dictionary<int, int>(),
                instanceIdToParentIndex = new Dictionary<int, int>(),
                instances = new List<ConvertToEntity>()
            };

            var roots = GetAllSceneRoots(sceneBakingInfo.scene);
            int currentIndex = 0;

            foreach (var root in roots)
            {
                TraverseHierarchy(root, ref lookup, ref currentIndex, -1);
            }

            ProcessBake(lookup, sceneBakingInfo.bakedData);
        }

        private void ProcessBake(BakingLookup lookup, BakedDataAsset bakedDataAsset)
        {
            var bakedData = new List<BakedData>();
            var components = new List<SetComponentData>();
            var bakingContext = new BakingContext(components, lookup);
            var componentsCount = 0;
            var parentChildPairsCount = 0;
            var userContext = default(UserContext);  //UserContext.Create(this.userContext);

            for (int i = 0; i < lookup.instances.Count; i++)
            {
                var converter = lookup.instances[i];
                var authorings = converter.GetAuthorings();

                for (int j = 0; j < authorings.Length; j++)
                {
                    var authoring = authorings[j];
                    authoring.OnBeforeBake(userContext);
                }
            }

            for (int i = 0; i < lookup.instances.Count; i++)
            {
                var converter = lookup.instances[i];
                var instanceId = converter.gameObject.GetInstanceID();
                var authorings = converter.GetAuthorings();
                var shouldUnparent = false;

                for (int j = 0; j < authorings.Length; j++)
                {
                    var authoring = authorings[j];
                    authoring.OnBake(bakingContext, userContext);
                    shouldUnparent |= authoring.ShouldUnparent;
                }

                var parentIndex = shouldUnparent ? -1 : lookup.instanceIdToParentIndex[instanceId];
                TransformBaking.BakeTransformGroup(bakingContext, converter.gameObject, parentIndex >= 0);

                parentChildPairsCount = parentIndex >= 0 ? parentChildPairsCount + 1 : parentChildPairsCount;
                componentsCount += components.Count;

                var addedComponents = new SetComponentData[components.Count];
                components.CopyTo(addedComponents);
                components.Clear();

                bakedData.Add(new BakedData()
                {
                    components = addedComponents,
                    parentIndex = parentIndex
                });
            }

            bakedDataAsset.serializedData = Serialization.SerializationUtility.SerializeBakedData(bakedData);
            bakedDataAsset.metadata = new BakedMetadata()
            {
                componentsCount = componentsCount,
                parentChildPairsCount = parentChildPairsCount
            };

            EditorUtility.SetDirty(bakedDataAsset);
        }

        private static void TraverseHierarchy(ConvertToEntity current, ref BakingLookup lookup, ref int currentIndex, int parentIndex)
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

        private static IEnumerable<ConvertToEntity> GetAllSceneRoots(Scene scene)
        {
            return scene.GetRootGameObjects().Select(x => x.GetComponent<ConvertToEntity>()).Where(c => c != null && c.excludeFromScene == false);
        }
    }
}
#endif
