#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter.Utilities
{
    internal sealed class RestorePreBakingEditorStatePostprocessor : IAssetPostprocessSystem
    {
        private const string SELECTED_OBJECT_PATH_KEY = "LAST_SELECTED_OBJECT_PATH";

        private Object lastValidSelectionObject;
        private string lastValidSelectionObjectPath;

        public RestorePreBakingEditorStatePostprocessor()
        {
            Selection.selectionChanged += TrackSelection;
        }

        public void Execute(OnAssetPostprocessContext context)
        {
            if (context.DidDomainReload)
            {
                RestoreActiveSelectionOnDomainReload();
            }

            if (lastValidSelectionObject != null)
            {
                Selection.activeObject = lastValidSelectionObject;
            }
        }

        private void TrackSelection()
        {
            var active = Selection.activeObject;

            if (active != null)
            {
                lastValidSelectionObjectPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                EditorPrefs.SetString(SELECTED_OBJECT_PATH_KEY, lastValidSelectionObjectPath);
                lastValidSelectionObject = active;
            }
        }

        private void RestoreActiveSelectionOnDomainReload()
        {
            lastValidSelectionObjectPath = EditorPrefs.GetString(SELECTED_OBJECT_PATH_KEY, string.Empty);

            if (string.IsNullOrEmpty(lastValidSelectionObjectPath) == false)
            {
                lastValidSelectionObject = AssetDatabase.LoadAssetAtPath<Object>(lastValidSelectionObjectPath);
            }
        }
    }
}
#endif
