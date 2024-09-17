using UnityEditor;

namespace Scellecs.Morpeh.EntityConverter.Editor
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
