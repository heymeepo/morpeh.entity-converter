#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter.Utilities
{
    public static class PrefabUtility
    {
        public static bool IsSceneObject(GameObject obj) => obj.scene.IsValid() && PrefabStageUtility.GetPrefabStage(obj) == null;

        public static bool IsInPrefabEditingMode(GameObject obj) => obj.scene.IsValid() && PrefabStageUtility.GetPrefabStage(obj) != null;
    }
}
#endif
