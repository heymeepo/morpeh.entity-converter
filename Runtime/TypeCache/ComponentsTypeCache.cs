using Scellecs.Morpeh.Workaround.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Scellecs.Morpeh.EntityConverter
{
    internal static class ComponentsTypeCache
    {
#if UNITY_EDITOR
        private static Dictionary<Type, int> allTypes;
#endif
        private static Action<World> WarmupTypes;

        static ComponentsTypeCache()
        {
            var typeCacheType = ReflectionHelpers.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.FullName == "MainAssembly_TypeRegistry.morpeh__ComponentsTypeCache")
                .FirstOrDefault();
#if UNITY_EDITOR
            typeCacheType.GetMethod("LoadCache", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
            allTypes = (Dictionary<Type, int>)typeCacheType.GetField("AllComponentTypes", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
#endif
            WarmupTypes = (Action<World>)Delegate.CreateDelegate(typeof(Action<World>), typeCacheType.GetMethod("WarmupAllTypes", BindingFlags.Static | BindingFlags.NonPublic));
        }
#if UNITY_EDITOR
        public static int GetTypeId(Type componentType) => allTypes[componentType] + 1;
#endif
        public static void WarmupComponentsTypes(World world) => WarmupTypes(world);
    }
}
