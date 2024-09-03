﻿#if UNITY_EDITOR
using UnityEditor;

namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class EntityConverter
    {
        private EntityConverterAssetPostprocessor assetPostProcessor;
        private EntityConverterBuildPreprocessor buildPreprocessor;
        private EntityConverterServiceProvider serviceProvider;
        private EntityConverterRepository repository;
        private EntityBakingService bakingService;
        private BakingProcessor bakingProcessor;

        public void Initialize()
        {
            repository = new EntityConverterRepository();
            assetPostProcessor = new EntityConverterAssetPostprocessor(repository);
            buildPreprocessor = new EntityConverterBuildPreprocessor(repository);
            bakingProcessor = new BakingProcessor();
            bakingService = new EntityBakingService(repository, bakingProcessor);
            serviceProvider = EntityConverterServiceProvider.CreateInstance(repository, bakingService);

            repository.Initialize();

            if (CheckEditorSession())
            {
                repository.Reload();
            }
        }

        private bool CheckEditorSession()
        {
            var result = SessionState.GetBool("EditorSessionStarted", false);

            if (result == false)
            {
                SessionState.SetBool("EditorSessionStarted", true);
            }

            return result;
        }
    }
}
#endif
