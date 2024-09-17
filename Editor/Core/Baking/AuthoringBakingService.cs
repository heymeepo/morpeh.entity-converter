using Scellecs.Morpeh.EntityConverter.Logs;
using Scellecs.Morpeh.EntityConverter.Utilities;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Scellecs.Morpeh.EntityConverter.Editor.Baking
{
    internal sealed class AuthoringBakingService : IAuthoringBakingService
    {
        private readonly IReadOnlyAuthoringDataService authoringDataService;
        private readonly IReadOnlySceneDependencyService sceneDependencyService;
        private readonly BakingProcessor bakingProcessor;
        private readonly RestorePreBakingEditorState restorePreBakingEditorState;
        private readonly ILogger logger;

        public AuthoringBakingService(
            IReadOnlyAuthoringDataService authoringDataService,
            IReadOnlySceneDependencyService sceneDependencyService,
            BakingProcessor bakingProcessor,
            ILogger logger)
        {
            this.authoringDataService = authoringDataService;
            this.sceneDependencyService = sceneDependencyService;
            this.bakingProcessor = bakingProcessor;
            this.logger = logger;
            this.restorePreBakingEditorState = new RestorePreBakingEditorState();
        }

        public void ForceGlobalBake()
        {
            SavePreBakingEditorState();
            SaveDirtyBeforeBaking();

            var prefabGUIDs = authoringDataService.GetPrefabGuids();
            var sceneGUIDs = authoringDataService.GetSceneGuids();

            int prefabsBaked = 0;
            int scenesBaked = 0;

            foreach (var guid in prefabGUIDs)
            {
                if (BakePrefabInternal(guid))
                {
                    prefabsBaked++;
                }
            }

            foreach (var guid in sceneGUIDs)
            {
                if (BakeSceneInternal(guid))
                {
                    scenesBaked++;
                }
            }

            SaveAssets();
            RestorePreBakingEditorState();

            logger.Log($"Global rebake forced: total rebaked: {prefabsBaked} prefabs, {scenesBaked} scenes", LogFlags.Info);
        }

        public void BakePrefab(string prefabGUID)
        {
            SavePreBakingEditorState();
            SaveDirtyBeforeBaking();

            BakePrefabInternal(prefabGUID);
            BakeDependentScenesForPrefab(prefabGUID);
            SaveAssets();
            RestorePreBakingEditorState();
        }

        public void BakeScene(string sceneGUID)
        {
            SavePreBakingEditorState();
            SaveDirtyBeforeBaking();

            BakeSceneInternal(sceneGUID);
            SaveAssets();
            RestorePreBakingEditorState();
        }

        internal bool BakePrefabInternal(string prefabGUID)
        {
            try
            {
                if (authoringDataService.IsPrefabGuidExists(prefabGUID))
                {
                    var path = AssetDatabase.GUIDToAssetPath(prefabGUID);
                    var prefab = UnityEditor.PrefabUtility.LoadPrefabContents(path);
                    var exists = prefab.TryGetComponent(out ConvertToEntity convertToEntity);

                    if (exists)
                    {
                        var bakedData = convertToEntity.bakedDataAsset;

                        if (bakedData != null)
                        {
                            var info = new PrefabBakingInfo()
                            {
                                root = convertToEntity,
                                bakedData = bakedData
                            };

                            bakingProcessor.ExecutePrefabBake(info);
                            logger.Log($"{nameof(AuthoringBakingService)}: Prefab baked {prefab.name}.", LogFlags.Info);
                        }
                    }

                    UnityEditor.PrefabUtility.UnloadPrefabContents(prefab);
                    return exists;
                }
            }
            catch (System.Exception e)
            {
                logger.LogError($"{nameof(AuthoringBakingService)}: Unable to bake prefab {e.Message}.", LogFlags.Regular);
            }

            return false;
        }

        internal bool BakeSceneInternal(string sceneGUID)
        {
            try
            {
                if (authoringDataService.TryGetSceneBakedDataAsset(sceneGUID, out var bakedDataAsset))
                {
                    var scene = Utilities.SceneUtility.GetSceneFromGUID(sceneGUID);
                    var openScene = scene.IsValid() == false;

                    if (openScene)
                    {
                        var scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
                        EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                        scene = Utilities.SceneUtility.GetSceneFromGUID(sceneGUID);
                    }

                    var info = new SceneBakingInfo()
                    {
                        scene = scene,
                        bakedData = bakedDataAsset
                    };

                    bakingProcessor.ExecuteSceneBake(info);

                    logger.Log($"{nameof(AuthoringBakingService)}: Scene baked {scene.name}.", LogFlags.Info);

                    if (openScene)
                    {
                        EditorSceneManager.CloseScene(scene, true);
                    }

                    return true;
                }
            }
            catch (System.Exception e)
            {
                logger.LogError($"{nameof(AuthoringBakingService)}: Unable to bake scene {e.Message}.", LogFlags.Regular);
            }

            return false;
        }

        internal void BakeDependentScenesForPrefab(string prefabGUID)
        {
            var dependentScenes = sceneDependencyService.GetSceneDependenciesForPrefab(prefabGUID);

            foreach (var sceneGUID in dependentScenes)
            {
                BakeSceneInternal(sceneGUID);
            }
        }

        internal void SaveDirtyBeforeBaking()
        {
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

            if (prefabStage != null)
            {
                UnityEditor.PrefabUtility.SaveAsPrefabAsset(prefabStage.prefabContentsRoot, prefabStage.assetPath);
                prefabStage.ClearDirtiness();
            }

            EditorSceneManager.SaveOpenScenes();
            SaveAssets();
        }

        internal void SavePreBakingEditorState() => restorePreBakingEditorState.SaveEditorState();

        internal void RestorePreBakingEditorState() => restorePreBakingEditorState.RestoreEditorState();

        private void SaveAssets()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
