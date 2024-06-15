#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Scellecs.Morpeh.EntityConverter.Utilities
{
    public static class SceneUtils
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
    }
}
#endif
