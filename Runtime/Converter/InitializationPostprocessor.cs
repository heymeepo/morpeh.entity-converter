﻿#if UNITY_EDITOR
using Scellecs.Morpeh.EntityConverter.Logger;
using UnityEditor;

namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class InitializationPostprocessor : IAssetPostprocessSystem
    {
        private const string INITIALIZE_FIRST_TIME_CALLED = "__EC_INITIALIZE_FIRST_TIME_CALLED";

        private readonly EntityConverterDataProvider dataProvider;
        private readonly SettingsService settingsService;
        private readonly AuthoringDataService authoringDataService;
        private readonly SceneDependencyService sceneDependencyService;
        private readonly ILogger logger;

        public InitializationPostprocessor(
            EntityConverterDataProvider dataProvider,
            SettingsService settingsService,
            AuthoringDataService authoringDataService,
            SceneDependencyService sceneDependencyService,
            ILogger logger)
        {
            this.dataProvider = dataProvider;
            this.settingsService = settingsService;
            this.authoringDataService = authoringDataService;
            this.sceneDependencyService = sceneDependencyService;
            this.logger = logger;
        }

        public void Execute(OnAssetPostprocessContext context)
        {
            if (IsInitializeCalledFirstTimeThisSession() == false)
            {
                Initialize();
            }

            if (context.DidDomainReload)
            {
                dataProvider.Reload();
            }

            if (dataProvider.IsValid() == false)
            {
                foreach (var assetPath in context.AllImportedAssetsPaths)
                {
                    var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);

                    if (asset as EntityConverterDataAsset != null)
                    {
                        Initialize();
                        break;
                    }
                }
            }
        }

        private void Initialize()
        {
            dataProvider.Reload();

            if (dataProvider.IsValid())
            {
                settingsService.Initialize();
                authoringDataService.Initialize();
                sceneDependencyService.Intialize();

                logger.LogInitializationSuccess<InitializationPostprocessor>();
            }
            else
            {
                logger.LogInitializationFailedDataAssetNotLoaded<InitializationPostprocessor>();
            }
        }

        private bool IsInitializeCalledFirstTimeThisSession()
        {
            var result = SessionState.GetBool(INITIALIZE_FIRST_TIME_CALLED, false);

            if (result == false)
            {
                SessionState.SetBool(INITIALIZE_FIRST_TIME_CALLED, true);
            }

            return result;
        }
    }
}
#endif
