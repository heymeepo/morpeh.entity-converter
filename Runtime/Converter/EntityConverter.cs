#if UNITY_EDITOR
using System.Collections.Generic;

namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class EntityConverter
    {
        private EntityConverterAssetPostprocessor assetPostprocessor;
        private EntityConverterBuildPreprocessor buildPreprocessor;
        private EntityConverterServiceProvider serviceProvider;
        private EntityConverterRepository repository;
        private AuthoringBakingService bakingService;
        private BakingProcessor bakingProcessor;

        public void Initialize()
        {
            repository = new EntityConverterRepository();
            buildPreprocessor = new EntityConverterBuildPreprocessor(repository);
            bakingProcessor = new BakingProcessor();
            bakingService = new AuthoringBakingService(repository, bakingProcessor);
            serviceProvider = EntityConverterServiceProvider.CreateInstance(repository, bakingService);

            CreatePostprocessor();
        }

        private void CreatePostprocessor()
        {
            var postprocessors = new List<AssetPostprocessSystem>()
            {
                new ValidateRepositoryPostprocesor(repository),
                new AutoRebakingPostprocessor(bakingService)
            };

            assetPostprocessor = new EntityConverterAssetPostprocessor(postprocessors);
        }
    }
}
#endif
