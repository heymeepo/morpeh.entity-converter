#if UNITY_EDITOR
using Scellecs.Morpeh.EntityConverter.Utilities;
using Scellecs.Morpeh.Workaround.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

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

            var postprocessors = new List<IAssetPostprocessSystem>
            {
                new ValidateRepositoryPostprocesor(repository),
                new AutoRebakingPostprocessor(bakingService),
                new RestoreActiveSelectionPostprocessor()
            };

            assetPostprocessor = EntityConverterAssetPostprocessor.CreateInstance(postprocessors);
        }
    }
}
#endif
