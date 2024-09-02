#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Scellecs.Morpeh.EntityConverter.BakingUtility;

namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class BakingProcessor
    {
        private List<ScriptableObject> userContext;

        public void ExecuteFullBake()
        {

        }

        public void ExecutePrefabBake()
        {

        }

        public void ExecuteSceneBake(SceneBakingInfo sceneBakingInfo)
        {
            var roots = GetAllSceneRoots(sceneBakingInfo.scene);
            var lookup = new BakingLookup()
            {
                instanceIdToIndex = new Dictionary<int, int>(),
                instanceIdToParentIndex = new Dictionary<int, int>(),
                instances = new List<ConvertToEntity>()
            };

            int currentIndex = 0;

            foreach (var root in roots)
            {
                TraverseHierarchy(root, ref lookup, ref currentIndex, -1);
            }

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

            sceneBakingInfo.sceneBakedData.serializedData = Serialization.SerializationUtility.SerializeBakedData(bakedData);
            sceneBakingInfo.sceneBakedData.metadata = new BakedMetadata()
            {
                componentsCount = componentsCount,
                parentChildPairsCount = parentChildPairsCount
            };

            EditorUtility.SetDirty(sceneBakingInfo.sceneBakedData);
        }
    }
}
#endif
