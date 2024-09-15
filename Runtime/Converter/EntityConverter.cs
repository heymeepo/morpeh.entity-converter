#if UNITY_EDITOR
using Scellecs.Morpeh.EntityConverter.Utilities;
using System.Collections.Generic;
using UnityEditor;

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
        private SceneDependencyTracker sceneTracker;
        private List<IAssetPostprocessSystem> postprocessors;

        public void Initialize()
        {
            repository = new EntityConverterRepository();
            buildPreprocessor = new EntityConverterBuildPreprocessor(repository);
            bakingProcessor = new BakingProcessor();
            bakingService = new AuthoringBakingService(repository, bakingProcessor);
            sceneTracker = new SceneDependencyTracker(repository);
            serviceProvider = EntityConverterServiceProvider.CreateInstance(repository, bakingService);
            assetPostprocessor = EntityConverterAssetPostprocessor.CreateInstance();
            postprocessors = new List<IAssetPostprocessSystem>
            {
                new ValidateRepositoryPostprocesor(repository),
                new SceneDependencyTrackerPostprocessor(sceneTracker),
                new AutoRebakingPostprocessor(bakingService, repository),
                new RestorePreBakingEditorStatePostprocessor()
            };

            EditorApplication.update += Update;
        }

        //This method is called after OnPostprocessAllAssets.
        //It's necessary because direct calls from OnPostprocessAllAssets cause unpredictable bugs.
        private void Update()
        {
            if (assetPostprocessor.TryGetContext(out var context))
            {
                foreach (var postrpocessor in postprocessors)
                {
                    postrpocessor.Execute(context);
                }
            }
        }
    }
}
#endif
