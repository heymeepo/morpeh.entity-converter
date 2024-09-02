#if UNITY_EDITOR
namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class EntityConverterServiceProvider
    {
        public static EntityConverterServiceProvider Instance { get; private set; }

        public IReadOnlyEntityConverterRepository Repository { get; private set; }
        public EntityBakingService EntityBakingService { get; private set; }

        private EntityConverterServiceProvider() { }

        public static EntityConverterServiceProvider CreateInstance(
            IReadOnlyEntityConverterRepository repository,
            EntityBakingService entityBaker)
        {
            Instance ??= new EntityConverterServiceProvider()
            { 
                Repository = repository,
                EntityBakingService = entityBaker
            };

            return Instance;
        }
    }
}
#endif
