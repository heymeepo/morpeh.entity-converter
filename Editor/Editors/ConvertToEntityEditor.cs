using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Scellecs.Morpeh.EntityConverter.Editor
{
    [CustomEditor(typeof(ConvertToEntity))]
    public sealed class ConvertToEntityEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var inspector = new VisualElement();
            var targetGO = (target as ConvertToEntity).gameObject;
            targetGO.tag = "EditorOnly";

            var isSceneMode = Utilities.PrefabUtility.IsSceneObject(targetGO);
            var bakedDataAssetField = CreateBakedDataAssetField();
            var excludeSceneCheckbox = CreateExcludeSceneCheckbox();
            bakedDataAssetField.SetEnabled(isSceneMode == false);

            inspector.Add(bakedDataAssetField);
            
            if (isSceneMode) 
            {
                inspector.Add(excludeSceneCheckbox);
            }

            return inspector;
        }

        private VisualElement CreateBakedDataAssetField()
        {
            var bakedDataProperty = serializedObject.FindProperty(nameof(ConvertToEntity.bakedDataAsset));
            var bakedDataAssetField = new ObjectField("Baked Data");
            bakedDataAssetField.objectType = typeof(PrefabBakedDataAsset);
            bakedDataAssetField.SetEnabled(true);
            bakedDataAssetField.BindProperty(bakedDataProperty);

            return bakedDataAssetField;
        }

        private VisualElement CreateExcludeSceneCheckbox()
        {
            var excludeSceneProperty = serializedObject.FindProperty(nameof(ConvertToEntity.excludeFromScene));
            var excludeSceneCheckbox = new Toggle("Exclude From Scene");
            excludeSceneCheckbox.BindProperty(excludeSceneProperty);

            return excludeSceneCheckbox;
        }
    }
}
