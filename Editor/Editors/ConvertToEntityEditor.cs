using Scellecs.Morpeh.EntityConverter.Utilities;
using System.ComponentModel;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
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

            if (targetGO.CompareTag("EditorOnly") == false)
            {
                targetGO.tag = "EditorOnly";
            }

            var isSceneMode = Utilities.PrefabUtility.IsSceneObject(targetGO);

            if (isSceneMode == false && HasRootConvertToEntity(targetGO) == false) //TODO: add uss
            {
                var warningContainer = new VisualElement()
                {
                    style =
                    {
                        flexDirection = FlexDirection.Column,
                        paddingLeft = 10,
                        paddingRight = 10,
                        paddingTop = 10,
                        paddingBottom = 10,
                    }
                };

                var warningLabel = new Label("Warning: The prefab does not have ConvertToEntity on the root object. This is required.")
                {
                    style =
                    {
                        color = Color.white,
                        whiteSpace = WhiteSpace.Normal,
                        overflow = Overflow.Hidden
                    }
                };

                warningContainer.Add(warningLabel);

                inspector.Add(warningContainer);
                return inspector;
            }

            var bakedDataAssetField = CreateBakedDataAssetField(targetGO);
            var excludeSceneCheckbox = CreateExcludeSceneCheckbox();

            if (isSceneMode)
            {
                bakedDataAssetField.SetEnabled(false);
            }

            inspector.Add(bakedDataAssetField);

            if (isSceneMode)
            {
                inspector.Add(excludeSceneCheckbox);
            }

            return inspector;
        }

        private bool HasRootConvertToEntity(GameObject targetGO) => targetGO.transform.root.GetComponent<ConvertToEntity>() != null;

        private VisualElement CreateBakedDataAssetField(GameObject target)
        {
            var bakedDataAssetField = new ObjectField("Baked Data") { objectType = typeof(PrefabBakedDataAsset) };
            var topmostConverterSerializedObject = GetTopmostConverterSerializedObject(target, bakedDataAssetField);

            var bakedDataProperty = topmostConverterSerializedObject.FindProperty(nameof(ConvertToEntity.bakedDataAsset));
            bakedDataAssetField.BindProperty(bakedDataProperty);

            return bakedDataAssetField;
        }

        private SerializedObject GetTopmostConverterSerializedObject(GameObject target, ObjectField bakedDataAssetField)
        {
            var topmostConverterSerializedObject = serializedObject;

            if (target.FindTopmostInHierarchy<ConvertToEntity>(out var topmostConverter) && topmostConverter.gameObject != target)
            {
                topmostConverterSerializedObject = new SerializedObject(topmostConverter);
                bakedDataAssetField.SetEnabled(false);
            }

            return topmostConverterSerializedObject;
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
