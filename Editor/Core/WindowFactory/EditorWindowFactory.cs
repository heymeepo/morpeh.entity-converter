using Scellecs.Morpeh.EntityConverter.Editor.Baking;
using UnityEditor;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter.Editor
{
    using SettingsService = Scellecs.Morpeh.EntityConverter.Editor.Settings.SettingsService;

    internal sealed class EditorWindowFactory
    {
        public static EditorWindowFactory Instance { get; private set; }

        private IEntityConverterDataNotifier dataNotifier;
        private IReadOnlyAuthoringDataService authoringDataService;
        private IAuthoringBakingService bakingService;
        private SettingsService settingsService;

        private EditorWindowFactory() { }

        public static EditorWindowFactory CreateInstance(
            IEntityConverterDataNotifier dataNotifier,
            IReadOnlyAuthoringDataService authoringDataService,
            IAuthoringBakingService bakingService,
            SettingsService settingsService)
        {
            Instance ??= new EditorWindowFactory()
            {
                dataNotifier = dataNotifier,
                authoringDataService = authoringDataService,
                bakingService = bakingService,
                settingsService = settingsService
            };

            return Instance;
        }

        public static void InitializeEntityConverterWindow(EntityConverterWindow window)
        {
            if (Instance != null)
            {
                window.Initialize(Instance.dataNotifier, Instance.authoringDataService, Instance.bakingService, Instance.settingsService);
            }
        }
    }
}
