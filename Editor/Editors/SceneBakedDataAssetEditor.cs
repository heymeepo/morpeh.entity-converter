using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Scellecs.Morpeh.EntityConverter.Editor
{
    [CustomEditor(typeof(SceneBakedDataAsset), false)]
    public sealed class SceneBakedDataAssetEditor : UnityEditor.Editor
    {
        private VisualElement inspector;

        public override VisualElement CreateInspectorGUI()
        {
            inspector = new VisualElement();
            CreateSceneProperty();
            return inspector;
        }

        private void CreateSceneProperty()
        {
            var pathProperty = serializedObject.FindPropertyByAutoPropertyName(nameof(SceneBakedDataAsset.SceneGuid));

            var sceneField = new ObjectField("Scene");
            sceneField.objectType = typeof(SceneAsset);
            sceneField.SetEnabled(false);

            if (string.IsNullOrEmpty(pathProperty.stringValue) == false)
            {
                var path = AssetDatabase.GUIDToAssetPath(pathProperty.stringValue);
                sceneField.value = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            }

            inspector.Add(sceneField);
        }
    }
}
