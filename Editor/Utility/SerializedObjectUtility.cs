using UnityEditor;

namespace Scellecs.Morpeh.EntityConverter.Editor
{
    public static class SerializedObjectUtility
    {
        public static SerializedProperty FindPropertyByAutoPropertyName(this SerializedObject obj, string propName)
        {
            return obj.FindProperty(string.Format("<{0}>k__BackingField", propName));
        }
    }
}
