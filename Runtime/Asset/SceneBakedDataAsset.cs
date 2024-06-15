#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Linq;
using Scellecs.Morpeh.EntityConverter.Utilities;
using UnityEngine.SceneManagement;

namespace Scellecs.Morpeh.EntityConverter
{
    [CreateAssetMenu(fileName = "SceneBakedData", menuName = "ECS/Baker/SceneBakedDataAsset")]
    public sealed class SceneBakedDataAsset : ScriptableObject
    {
        [SerializeField]
        internal string sceneGuid;

        [SerializeField]
        internal bool autoRebakeOnAssemblyReload;

        [SerializeField]
        internal List<EntityBakedDataAsset> sceneAssets;

        [SerializeField]
        private List<ScriptableObject> sharedResources;

#if UNITY_EDITOR
        internal void BakeScene()
        {
            var prevScene = EditorSceneManager.GetActiveScene();
            var prevScenePath = prevScene.path;

            var scene = GetSceneFromGUID(sceneGuid);

            if (scene.IsValid() == false)
            {
                EditorSceneManager.SaveScene(prevScene);
                var scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);
                EditorSceneManager.OpenScene(scenePath);
            }

            ValidateScene();

            scene = GetSceneFromGUID(sceneGuid);
            var roots = GetAllSceneRoots(scene);

            foreach (var root in roots)
            {
                if (root.excludeFromScene == false)
                {
                    root.Bake();
                    EditorUtility.SetDirty(root.bakedDataSceneAsset);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorSceneManager.SaveScene(scene);
            EditorSceneManager.OpenScene(prevScenePath);
        }

        internal void ValidateScene()
        {
            var scene = GetSceneFromGUID(sceneGuid);

            var bakedDatas = new Dictionary<int, int>();
            var childAssets = AssetDatabaseUtils.GetAllChildAssets(this);

            for (int i = 0; i < childAssets.Count; i++)
            {
                var sceneAsset = childAssets[i];
                bakedDatas.Add(sceneAsset.GetInstanceID(), i);
            }

            var roots = GetAllSceneRoots(scene);
            var newRoots = new List<ConvertToEntity>();
            var isDirty = false;

            sceneAssets.Clear();

            foreach (var root in roots)
            {
                if (root.excludeFromScene == false)
                {
                    if (root.bakedDataSceneAsset != null)
                    {
                        var key = root.bakedDataSceneAsset.GetInstanceID();

                        if (bakedDatas.TryGetValue(key, out var idx))
                        {
                            bakedDatas.Remove(key);
                            sceneAssets.Add(childAssets[idx] as EntityBakedDataAsset);
                        }
                        else
                        {
                            newRoots.Add(root);
                        }
                    }
                    else
                    {
                        newRoots.Add(root);
                    }
                }
            }

            var indicesToRemove = bakedDatas.Values.OrderByDescending(index => index);

            foreach (var idx in indicesToRemove)
            {
                AssetDatabase.RemoveObjectFromAsset(childAssets[idx]);
                isDirty = true;
            }

            foreach (var root in newRoots)
            {
                CreateNewBakedData(root);
            }

            if (isDirty)
            {
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorSceneManager.SaveScene(scene);
            }

            void CreateNewBakedData(ConvertToEntity root)
            {
                var bakedDataAsset = CreateInstance<EntityBakedDataAsset>();
                bakedDataAsset.name = $"{root.name}_EntityBakedData";
                root.bakedDataSceneAsset = bakedDataAsset;
                sceneAssets.Add(bakedDataAsset);
                root.Bake();
                EditorUtility.SetDirty(bakedDataAsset);
                AssetDatabase.AddObjectToAsset(bakedDataAsset, this);
                isDirty = true;
            }
        }

        private static IEnumerable<ConvertToEntity> GetAllSceneRoots(Scene scene) => scene.GetRootGameObjects().Select(x => x.GetComponent<ConvertToEntity>()).Where(c => c != null);

        private static Scene GetSceneFromGUID(string sceneGuid)
        {
            var scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);
            return EditorSceneManager.GetSceneByPath(scenePath);
        }
#endif
    }
}
