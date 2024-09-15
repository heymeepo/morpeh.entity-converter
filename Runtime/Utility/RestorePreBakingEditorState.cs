#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter.Utilities
{
    internal sealed class RestorePreBakingEditorState
    {
        private string prefabPath;
        private string selectionPath;

        private Object lastValidSelectionObject;

        public void SaveEditorState()
        {
            prefabPath = string.Empty;
            selectionPath = string.Empty;

            var active = Selection.activeObject;
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

            if (prefabStage != null)
            {
                prefabPath = prefabStage.assetPath;
                if (active is GameObject go)
                {
                    if (prefabStage.IsPartOfPrefabContents(go))
                    {
                        selectionPath = GetRelativePath(prefabStage.prefabContentsRoot, go);
                    }
                }
            }
        }

        public void RestoreEditorState()
        {
            if (string.IsNullOrEmpty(prefabPath) == false)
            {
                RestoreOpenedPrefabSelection();
            }

            if (lastValidSelectionObject != null)
            {
                Selection.activeObject = lastValidSelectionObject;
            }
        }

        private void RestoreOpenedPrefabSelection()
        {
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

            if (prefabStage == null)
            {
                prefabStage = PrefabStageUtility.OpenPrefab(prefabPath);
            }

            var root = prefabStage.prefabContentsRoot;

            if (string.IsNullOrEmpty(selectionPath) == false)
            {
                var selected = root.transform.Find(selectionPath);

                if (selected != null)
                {
                    lastValidSelectionObject = selected;
                }
            }
        }

        private string GetRelativePath(GameObject root, GameObject obj)
        {
            string path = obj.name;
            Transform current = obj.transform;
            while (current.parent != null && current.parent != root.transform)
            {
                path = current.parent.name + "/" + path;
                current = current.parent;
            }
            return path;
        }
    }
}
#endif
