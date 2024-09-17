using Scellecs.Morpeh.EntityConverter.Logs;
using Scellecs.Morpeh.EntityConverter.Utilities;
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

            var prefabGuids = authoringDataService.GetPrefabGuids();
            var sceneGuids = authoringDataService.GetSceneGuids();

            foreach (var guid in prefabGuids)
            {
                BakePrefabInternal(guid);
            }

            foreach (var guid in sceneGuids)
            {
                BakeSceneInternal(guid);
            }

            SaveAssets();
            RestorePreBakingEditorState();
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

        internal void BakePrefabInternal(string prefabGUID)
        {
            if (authoringDataService.IsPrefabGuidExists(prefabGUID))
            {
                try
                {
                    var path = AssetDatabase.GUIDToAssetPath(prefabGUID);
                    var prefab = UnityEditor.PrefabUtility.LoadPrefabContents(path);

                    if (prefab.TryGetComponent(out ConvertToEntity convertToEntity))
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
                        }
                    }

                    logger.Log($"{nameof(AuthoringBakingService)}: Prefab baked {prefab.name}.", LogDepthFlags.Info);
                    UnityEditor.PrefabUtility.UnloadPrefabContents(prefab);
                }
                catch (System.Exception e)
                {
                    logger.LogError($"{nameof(AuthoringBakingService)}: Unable to bake prefab {e.Message}.", LogDepthFlags.Regular);
                }
            }
        }

        internal void BakeSceneInternal(string sceneGUID)
        {
            if (authoringDataService.TryGetSceneBakedDataAsset(sceneGUID, out var bakedDataAsset))
            {
                try
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

                    logger.Log($"{nameof(AuthoringBakingService)}: Scene baked {scene.name}.", LogDepthFlags.Info);

                    if (openScene)
                    {
                        EditorSceneManager.CloseScene(scene, true);
                    }
                }
                catch (System.Exception e)
                {
                    logger.LogError($"{nameof(AuthoringBakingService)}: Unable to bake scene {e.Message}.", LogDepthFlags.Regular);
                }
            }
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
