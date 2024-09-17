using Scellecs.Morpeh.EntityConverter.Editor.Baking;
using UnityEditor;

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
            EntityConverterWindow window = EditorWindow.GetWindow<EntityConverterWindow>();
        }
    }
}
