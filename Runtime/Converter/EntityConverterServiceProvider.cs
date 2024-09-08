#if UNITY_EDITOR
namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class EntityConverterServiceProvider
    {
        public static EntityConverterServiceProvider Instance { get; private set; }

        public IReadOnlyEntityConverterRepository Repository { get; private set; }
        public AuthoringBakingService EntityBakingService { get; private set; }

        private EntityConverterServiceProvider() { }

        public static EntityConverterServiceProvider CreateInstance(IReadOnlyEntityConverterRepository repository, AuthoringBakingService entityBaker)
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
