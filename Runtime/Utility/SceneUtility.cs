#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scellecs.Morpeh.EntityConverter.Utilities
{
    public static class SceneUtility
    {
        public static string[] FindAllProjectSceneGuids()
        {
            var sceneGuids = AssetDatabase.FindAssets("t:Scene");
            var scenePaths = new List<string>();

            foreach (string guid in sceneGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);

                if (path.StartsWith("Assets/"))
                {
                    scenePaths.Add(guid);
                }
            }

            return scenePaths.ToArray();
        }

        public static Scene GetSceneFromGUID(string sceneGuid)
        {
            var scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);
            return EditorSceneManager.GetSceneByPath(scenePath);
        }

        public static string GetActiveSceneGUID()
        {
            var scene = EditorSceneManager.GetActiveScene();
            return GetSceneGUID(scene);
        }

        public static string GetSceneGUID(Scene scene) => AssetDatabase.GUIDFromAssetPath(scene.path).ToString();

        public static IEnumerable<ConvertToEntity> GetAllTopmostConvertersInScene(Scene scene)
        {
            var rootObjects = scene.GetRootGameObjects();

            List<ConvertToEntity> topMostEntities = new List<ConvertToEntity>();

            foreach (var root in rootObjects)
            {
                var topEntitiesInHierarchy = GetTopmostConvertersInHierarchy(root);
                topMostEntities.AddRange(topEntitiesInHierarchy);
            }

            return topMostEntities;
        }

        private static IEnumerable<ConvertToEntity> GetTopmostConvertersInHierarchy(GameObject root)
        {
            var result = new List<ConvertToEntity>();
            TraverseHierarchy(root);

            void TraverseHierarchy(GameObject currentObject)
            {
                var conveter = currentObject.GetComponent<ConvertToEntity>();

                if (conveter != null)
                {
                    if (conveter.excludeFromScene == false)
                    {
                        result.Add(conveter);
                    }
                }
                else
                {
                    foreach (Transform child in currentObject.transform)
                    {
                        TraverseHierarchy(child.gameObject);
                    }
                }
            }

            return result;
        }
    }
}
#endif
