using Scellecs.Morpeh.EntityConverter.Editor.Baking;
using UnityEditor;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter.Editor
{
    using SettingsService = Scellecs.Morpeh.EntityConverter.Editor.Settings.SettingsService;

    internal sealed class EditorWindowFactory
    {
        private static EditorWindowFactory instance;

        private SettingsService settingsService;
        private IReadOnlyAuthoringDataService authoringDataService;
        private IAuthoringBakingService bakingService;

        private EditorWindowFactory() { }

        public static EditorWindowFactory CreateInstance(
            SettingsService settingsService,
            IReadOnlyAuthoringDataService authoringDataService,
            IAuthoringBakingService bakingService)
        {
            instance ??= new EditorWindowFactory()
            {
                settingsService = settingsService,
                authoringDataService = authoringDataService,
                bakingService = bakingService
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
                window.Initialize(instance.settingsService, instance.authoringDataService, instance.bakingService);
            }
        }
    }
}
