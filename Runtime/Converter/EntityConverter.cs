#if UNITY_EDITOR
namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class EntityConverter
    {
        private EntityConverterAssetPostprocessor assetPostProcessor;
        private EntityConverterBuildPreprocessor buildPreprocessor;
        private EntityConverterServiceProvider serviceProvider;
        private EntityConverterRepository repository;
        private BakingProcessor bakingProcessor;
        private EntityBakingService bakingService;

        public void Initialize()
        {
            repository = new EntityConverterRepository();
            assetPostProcessor = new EntityConverterAssetPostprocessor(repository);
            buildPreprocessor = new EntityConverterBuildPreprocessor(repository);
            bakingProcessor = new BakingProcessor();
            bakingService = new EntityBakingService(repository, bakingProcessor);
            serviceProvider = EntityConverterServiceProvider.CreateInstance(repository, bakingService);

            repository.Reload();
        }
    }
}
#endif
