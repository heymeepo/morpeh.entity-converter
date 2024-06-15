using Scellecs.Morpeh.EntityConverter.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Scellecs.Morpeh.EntityConverter.Editor
{
    [CustomEditor(typeof(ConvertToEntity))]
    public sealed class ConvertToEntityEditor : UnityEditor.Editor
    {
        private ObjectField bakedDataAssetField;
        private Toggle excludeSceneCheckbox;

        public override VisualElement CreateInspectorGUI()
        {
            var inspector = new VisualElement();
            var targetGO = (target as ConvertToEntity).gameObject;
            targetGO.tag = "EditorOnly";

            var sceneMode = PrefabUtils.IsSceneObject(targetGO);

            if (sceneMode)
            {
                CreateSceneBakedDataAssetField();
                CreateExcludeSceneCheckbox();
            }
            else
            {
                CreateBakedDataAssetField();
            }

            inspector.Add(bakedDataAssetField);

            if (sceneMode)
            {
                inspector.Add(excludeSceneCheckbox);
            }

            return inspector;
        }

        private void CreateBakedDataAssetField()
        {
            var bakedDataProperty = serializedObject.FindProperty(nameof(ConvertToEntity.bakedDataAsset));

            bakedDataAssetField = new ObjectField("Baked Data");
            bakedDataAssetField.objectType = typeof(EntityBakedDataAsset);
            bakedDataAssetField.SetEnabled(true);
            bakedDataAssetField.BindProperty(bakedDataProperty);
        }

        private void CreateSceneBakedDataAssetField()
        {
            var bakedDataProperty = serializedObject.FindProperty(nameof(ConvertToEntity.bakedDataSceneAsset));

            bakedDataAssetField = new ObjectField("Baked Data");
            bakedDataAssetField.objectType = typeof(EntityBakedDataAsset);
            bakedDataAssetField.SetEnabled(false);
            bakedDataAssetField.BindProperty(bakedDataProperty);
        }

        private void CreateExcludeSceneCheckbox()
        {
            var excludeSceneProperty = serializedObject.FindProperty(nameof(ConvertToEntity.excludeFromScene));

            excludeSceneCheckbox = new Toggle("Exclude From Scene");
            excludeSceneCheckbox.BindProperty(excludeSceneProperty);
        }
    }
}
