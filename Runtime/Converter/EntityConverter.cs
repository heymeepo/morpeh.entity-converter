﻿#if UNITY_EDITOR
using Scellecs.Morpeh.EntityConverter.Logs;
using System.Collections.Generic;
using UnityEditor;

namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class EntityConverter
    {
        private EntityConverterAssetPostprocessor assetPostprocessor;
        private List<IAssetPostprocessSystem> postprocessors;

        private EntityConverterDataProvider dataProvider;
        private SettingsService settingsService;
        private AuthoringDataService authoringDataService;
        private SceneDependencyService sceneDependencyService;
        private SceneDependencyTracker sceneTracker;

        private BakingProcessor bakingProcessor;
        private AuthoringBakingService bakingService;

        private Logger logger;

        public void Initialize()
        {
            logger = new Logger();
            dataProvider = new EntityConverterDataProvider();
            settingsService = new SettingsService(dataProvider, logger);
            authoringDataService = new AuthoringDataService(dataProvider, logger);
            sceneDependencyService = new SceneDependencyService(dataProvider, authoringDataService, logger);
            sceneTracker = new SceneDependencyTracker(sceneDependencyService);

            bakingProcessor = new BakingProcessor(logger);
            bakingService = new AuthoringBakingService(authoringDataService, sceneDependencyService, bakingProcessor, logger);

            assetPostprocessor = EntityConverterAssetPostprocessor.CreateInstance();
            postprocessors = new List<IAssetPostprocessSystem>
            {
                new InitializationPostprocessor(dataProvider, settingsService, authoringDataService, sceneDependencyService, logger),
                new ValidateAuthoringDataPostprocessor(authoringDataService),
                new SceneDependencyTrackerPostprocessor(sceneTracker),
                new AutoRebakingPostprocessor(bakingService, settingsService, sceneDependencyService, logger),
                new PlaymodePostprocessor(authoringDataService, settingsService, logger)
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
