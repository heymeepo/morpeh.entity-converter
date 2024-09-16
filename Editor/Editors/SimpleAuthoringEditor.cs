using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using static Scellecs.Morpeh.EntityConverter.Editor.SimpleAuthoringEditorUtility;

namespace Scellecs.Morpeh.EntityConverter.Editor
{
    [CustomEditor(typeof(SimpleAuthoring<>), true)]
    internal sealed class SimpleAuthoringEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();
            var field = CreateCompoentField(serializedObject, "component");

            container.Add(field);

            return container;
        }
    }

    [CustomEditor(typeof(SimpleAuthoring<,>), true)]
    internal sealed class SimpleAuthoring2ArgsEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();
            var field1 = CreateCompoentField(serializedObject, "component0");
            var field2 = CreateCompoentField(serializedObject, "component1");
            
            container.Add(field1);
            container.Add(field2);

            return container;
        }
    }

    [CustomEditor(typeof(SimpleAuthoring<,,>), true)]
    internal sealed class SimpleAuthoring3ArgsEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();
            var field1 = CreateCompoentField(serializedObject, "component0");
            var field2 = CreateCompoentField(serializedObject, "component1");
            var field3 = CreateCompoentField(serializedObject, "component2");

            container.Add(field1);
            container.Add(field2);
            container.Add(field3);

            return container;
        }
    }

    internal static class SimpleAuthoringEditorUtility
    {
        public static PropertyField CreateCompoentField(SerializedObject serializedObject, string propertyName)
        {
            var property = serializedObject.FindProperty(propertyName);
            return new PropertyField(property, property.type);
        }
    }
}
