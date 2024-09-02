using UnityEditor;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter.Editor
{
    [CustomPropertyDrawer(typeof(EntityLink))]
    public sealed class EntityLinkDrawer : PropertyDrawer
    {
        private bool isValid = true;
        private bool isInitialized = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var component = property.serializedObject.targetObject as Component;

            if (component != null)
            {
                var sourceConvertToEntity = component.gameObject.GetComponent<ConvertToEntity>();

                if (isInitialized == false)
                {
                    PerformValidation(property, sourceConvertToEntity);
                    isInitialized = true;
                }

                EditorGUI.BeginProperty(position, label, property);

                var convertToEntityProperty = property.FindPropertyRelative(nameof(EntityLink.convertToEntity));
                var fieldHeight = EditorGUIUtility.singleLineHeight;

                var originalPosition = position;
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent($"{property.displayName}"));

                EditorGUI.BeginChangeCheck();
                var newObject = EditorGUI.ObjectField(
                    new Rect(position.x, position.y, position.width, fieldHeight),
                    convertToEntityProperty.objectReferenceValue,
                    typeof(ConvertToEntity),
                    true);

                if (EditorGUI.EndChangeCheck())
                {
                    convertToEntityProperty.objectReferenceValue = newObject;
                    property.serializedObject.ApplyModifiedProperties();
                    PerformValidation(property, sourceConvertToEntity);
                }

                if (isValid == false && sourceConvertToEntity != null)
                {
                    var warningHeight = EditorGUIUtility.singleLineHeight * 2;
                    EditorGUI.HelpBox(
                        new Rect(originalPosition.x, originalPosition.y + fieldHeight + EditorGUIUtility.standardVerticalSpacing, originalPosition.width, warningHeight),
                        "The given authoring GameObject is incompatible with the target link. The EntityLink will not pass validation and will not be baked.",
                        MessageType.Warning
                    );
                    position.y += warningHeight + EditorGUIUtility.standardVerticalSpacing;
                }

                EditorGUI.EndProperty();
            }
        }

        private void PerformValidation(SerializedProperty property, ConvertToEntity sourceConvertToEntity)
        {
            var convertToEntityProperty = property.FindPropertyRelative(nameof(EntityLink.convertToEntity));
            var newObject = convertToEntityProperty.objectReferenceValue as ConvertToEntity;

            if (sourceConvertToEntity != null && newObject != null)
            {
                isValid = sourceConvertToEntity.ValidateEntityLinkCompability(newObject);
            }
            else
            {
                isValid = true;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var component = property.serializedObject.targetObject as Component;

            if (component != null)
            {
                var sourceConvertToEntity = component.gameObject.GetComponent<ConvertToEntity>();
                var convertToEntityProperty = property.FindPropertyRelative(nameof(EntityLink.convertToEntity));

                if (isValid == false && sourceConvertToEntity != null && convertToEntityProperty.objectReferenceValue != null)
                {
                    return EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing;
                }
            }

            return EditorGUIUtility.singleLineHeight;
        }
    }
}
