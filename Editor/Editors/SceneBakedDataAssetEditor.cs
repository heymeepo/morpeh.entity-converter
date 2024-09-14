using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scellecs.Morpeh.EntityConverter.Editor
{
    [CustomEditor(typeof(SceneBakedDataAsset), false)]
    public sealed class SceneBakedDataAssetEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI() //TODO: add uss
        {
            var inspector = new VisualElement();
            var sceneField = CreateSceneProperty();
            var metadataField = CreateMetadataProperty();
            var unityObjectsField = CreateUnityObjectsList();

            inspector.Add(sceneField);
            inspector.Add(metadataField);
            inspector.Add(unityObjectsField);

            return inspector;
        }

        private VisualElement CreateUnityObjectsList()
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Column;
            container.style.paddingLeft = 5;
            container.style.paddingRight = 5;
            container.style.paddingTop = 5;
            container.style.paddingBottom = 5;

            var unityObjectsProperty = serializedObject.FindProperty(nameof(BakedDataAsset.serializedData)).FindPropertyRelative(nameof(SerializedBakedData.unityObjects));
            var unityObjectsField = new PropertyField(unityObjectsProperty);
            unityObjectsField.SetEnabled(false);

            container.Add(unityObjectsField);
            return container;
        }

        private VisualElement CreateMetadataProperty()
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Column;
            container.style.paddingLeft = 5;
            container.style.paddingRight = 5;
            container.style.paddingTop = 5;
            container.style.paddingBottom = 5;

            var entitiesCountProperty = serializedObject.FindProperty(nameof(BakedDataAsset.metadata)).FindPropertyRelative(nameof(BakedMetadata.entitiesCount));
            var componentsCountProperty = serializedObject.FindProperty(nameof(BakedDataAsset.metadata)).FindPropertyRelative(nameof(BakedMetadata.componentsCount));
            var parentChildPairsCountProperty = serializedObject.FindProperty(nameof(BakedDataAsset.metadata)).FindPropertyRelative(nameof(BakedMetadata.parentChildPairsCount));
            var serializedDataProperty = serializedObject.FindProperty(nameof(BakedDataAsset.serializedData)).FindPropertyRelative(nameof(SerializedBakedData.serializedData));

            var entitiesCountField = new Label($"Entities: {entitiesCountProperty.intValue}");
            var rootEntitiesCountField = new Label($"Root entities: {(entitiesCountProperty.intValue - parentChildPairsCountProperty.intValue)}");
            var componentsCountField = new Label($"Components: {componentsCountProperty.intValue}");
            var parentChildPairsCountField = new Label($"Parent child pairs: {parentChildPairsCountProperty.intValue}");
            var kbBakedField = new Label($"Total baked: {(serializedDataProperty.arraySize / 1024f):F2} KB");

            container.Add(entitiesCountField);
            container.Add(rootEntitiesCountField);
            container.Add(componentsCountField);
            container.Add(parentChildPairsCountField);
            container.Add(kbBakedField);

            return container;
        }

        private ObjectField CreateSceneProperty()
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

            return sceneField;
        }
    }
}
