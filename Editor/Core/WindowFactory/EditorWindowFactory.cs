using Scellecs.Morpeh.EntityConverter.Editor.Baking;
using UnityEditor;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter.Editor
{
    using SettingsService = Scellecs.Morpeh.EntityConverter.Editor.Settings.SettingsService;

    internal sealed class EditorWindowFactory
    {
        private static EditorWindowFactory instance;

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
            instance ??= new EditorWindowFactory()
            {
                dataNotifier = dataNotifier,
                authoringDataService = authoringDataService,
                bakingService = bakingService,
                settingsService = settingsService
            };
            return instance;
        }

        [MenuItem("Tools/Morpeh/Entity Converter")]
        private static void CreateEntityConverterWindow()
        {
            if (instance != null)
            {
                EntityConverterWindow window = EditorWindow.GetWindow<EntityConverterWindow>();
                window.titleContent = new GUIContent("Entity Converter");
                window.Initialize(instance.dataNotifier, instance.authoringDataService, instance.bakingService, instance.settingsService);
            }
        }
    }
}
