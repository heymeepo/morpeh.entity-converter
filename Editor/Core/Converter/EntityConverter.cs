using Scellecs.Morpeh.EntityConverter.Editor.Baking;
using Scellecs.Morpeh.EntityConverter.Logs;
using System.Collections.Generic;
using System.Text;
using UnityEditor;

namespace Scellecs.Morpeh.EntityConverter.Editor
{
    using SettingsService = Scellecs.Morpeh.EntityConverter.Editor.Settings.SettingsService;

    internal sealed class EntityConverter
    {
        private Logger logger;

        private EntityConverterDataProvider dataProvider;
        private SettingsService settingsService;
        private AuthoringDataService authoringDataService;
        private SceneDependencyService sceneDependencyService;
        private SceneDependencyTracker sceneTracker;

        private BakingProcessor bakingProcessor;
        private AuthoringBakingService bakingService;

        private EntityConverterAssetPostprocessor assetPostprocessor;
        private List<IAssetPostprocessSystem> postprocessors;

        private EditorWindowFactory editorWindowFactory;

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

            editorWindowFactory = EditorWindowFactory.CreateInstance(dataProvider, authoringDataService, bakingService, settingsService);
            EditorApplication.update += Update;
        }

        //This method necessary because direct calls from OnPostprocessAllAssets cause unpredictable bugs.
        private void Update()
        {
            if (assetPostprocessor.TryGetContext(out var context))
            {
                LogContext(context);

                foreach (var postrpocessor in postprocessors)
                {
                    postrpocessor.Execute(context);
                }
            }

            if (dataProvider.IsValid())
            {
                dataProvider.SaveAndNotifyChangedIfDirty();
            }
        }

        private void LogContext(OnAssetPostprocessContext context)
        {
            var sb = new StringBuilder();
            sb.AppendLine("OnAssetPostprocessContext:");
            sb.AppendLine("\nImportedAuthorings:");

            foreach (var importedAuthoring in context.ImportedAuthorings)
            {
                sb.AppendLine($"{importedAuthoring.type}, {importedAuthoring.GUID}, {importedAuthoring.path}");
            }

            sb.AppendLine("\nDeleted assets paths:");

            foreach (var deleted in context.AllDeletedAssetsPaths)
            {
                sb.AppendLine($"{deleted}");
            }

            logger.Log(sb.ToString(), LogFlags.InternalDebug);
        }
    }
}
