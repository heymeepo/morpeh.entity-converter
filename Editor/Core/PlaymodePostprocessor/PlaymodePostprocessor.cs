using Scellecs.Morpeh.EntityConverter.Logs;
using Scellecs.Morpeh.EntityConverter.Utilities;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Scellecs.Morpeh.EntityConverter.Editor
{
    using SettingsService = Scellecs.Morpeh.EntityConverter.Editor.Settings.SettingsService;

    internal sealed class PlaymodePostprocessor : IAssetPostprocessSystem
    {
        private readonly IReadOnlyAuthoringDataService authoringDataService;
        private readonly SettingsService settingsService;
        private readonly ILogger logger;

        private bool playmodeExitPerformed;

        public PlaymodePostprocessor(IReadOnlyAuthoringDataService authoringDataService, SettingsService settingsService, ILogger logger)
        {
            EditorApplication.playModeStateChanged += HanldePlaymodeState;
            this.authoringDataService = authoringDataService;
            this.settingsService = settingsService;
            this.logger = logger;
        }

        public void Execute(OnAssetPostprocessContext context)
        {
            if (playmodeExitPerformed)
            {
                settingsService.ClearTemporaryBakingFlags();
                playmodeExitPerformed = false;
            }
        }

        private void HanldePlaymodeState(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.ExitingEditMode)
            {
                DisableSceneAuthoringsOnEnterPlaymode();
            }
            else if(change == PlayModeStateChange.EnteredEditMode)
            {
                RestoreDisabledAuthoringsOnExitPlaymode();
            }
        }

        private void DisableSceneAuthoringsOnEnterPlaymode()
        {
            settingsService.SetTemporaryBakingFlags(0);
            var sceneGUIDs = authoringDataService.GetSceneGuids();

            foreach (var sceneGUID in sceneGUIDs)
            {
                if (authoringDataService.TryGetSceneBakedDataAsset(sceneGUID, out _))
                {
                    SetEnabledSceneAuthorings(sceneGUID, false);
                }
            }            
        }

        private void RestoreDisabledAuthoringsOnExitPlaymode()
        {
            settingsService.SetTemporaryBakingFlags(0);
            var sceneGUIDs = authoringDataService.GetSceneGuids();

            foreach (var sceneGUID in sceneGUIDs)
            {
                if (authoringDataService.TryGetSceneBakedDataAsset(sceneGUID, out _))
                {
                    SetEnabledSceneAuthorings(sceneGUID, true);
                }
            }

            playmodeExitPerformed = true;
        }

        private void SetEnabledSceneAuthorings(string sceneGUID, bool state)
        {
            var scene = Utilities.SceneUtility.GetSceneFromGUID(sceneGUID);
            var openScene = scene.IsValid() == false;

            if (openScene)
            {
                var scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                scene = Utilities.SceneUtility.GetSceneFromGUID(sceneGUID);
            }

            var roots = SceneUtility.GetAllTopmostConvertersInScene(scene);

            foreach (var root in roots) 
            { 
                root.gameObject.SetActive(state);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            logger.Log($"{nameof(PlaymodePostprocessor)}: Authorings set enabled {state}. At scene {scene.name}.", LogFlags.InternalDebug);

            if (openScene)
            {
                EditorSceneManager.CloseScene(scene, true);
            }

        }
    }
}
