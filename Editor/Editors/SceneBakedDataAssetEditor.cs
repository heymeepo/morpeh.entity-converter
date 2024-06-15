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
            CreateAutoRebakeToggle();

            return inspector;
        }

        private void CreateSceneProperty()
        {
            var pathProperty = serializedObject.FindProperty(nameof(SceneBakedDataAsset.sceneGuid));

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

        private void CreateAutoRebakeToggle()
        {
            var rebakeProperty = serializedObject.FindProperty(nameof(SceneBakedDataAsset.autoRebakeOnAssemblyReload));

            var toggle = new Toggle("Auto Rebake On Assembly Reload");
            toggle.BindProperty(rebakeProperty);
            inspector.Add(toggle);
        }
    }
}
