#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter
{
    [InitializeOnLoad]
    internal static class EntityConverterInitializer
    {
        private static EntityConverter converter;

        static EntityConverterInitializer()
        {
            converter = new EntityConverter();
            converter.Initialize();
        }
    }
}
#endif
