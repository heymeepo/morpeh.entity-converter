#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
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
            return AssetDatabase.GUIDFromAssetPath(scene.path).ToString();
        }
    }
}
#endif
