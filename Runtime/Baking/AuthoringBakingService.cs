#if UNITY_EDITOR
using Scellecs.Morpeh.EntityConverter.Utilities;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class AuthoringBakingService : IAuthoringBakingService
    {
        private readonly IReadOnlyEntityConverterRepository repository;
        private readonly BakingProcessor bakingProcessor;
        private readonly RestorePreBakingEditorState restorePreBakingState;

        public AuthoringBakingService(IReadOnlyEntityConverterRepository repository, BakingProcessor bakingProcessor)
        {
            this.repository = repository;
            this.bakingProcessor = bakingProcessor;
            restorePreBakingState = new RestorePreBakingEditorState();
        }

        public void ForceGlobalBake()
        {
            SavePreBakingEditorState();
            SaveDirtyBeforeBaking();

            if (repository.IsValid)
            {
                var prefabGuids = repository.GetPrefabGuids();
                var sceneGuids = repository.GetSceneGuids();

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
        }

        public void BakePrefab(string prefabGUID)
        {
            SavePreBakingEditorState();
            SaveDirtyBeforeBaking();

            if (repository.IsValid)
            {
                BakePrefabInternal(prefabGUID);
                BakeDependentScenesForPrefab(prefabGUID);
                SaveAssets();
                RestorePreBakingEditorState();
            }
        }

        public void BakeScene(string sceneGUID)
        {
            SavePreBakingEditorState();
            SaveDirtyBeforeBaking();

            if (repository.IsValid)
            {
                BakeSceneInternal(sceneGUID);
                SaveAssets();
                RestorePreBakingEditorState();
            }
        }

        internal void BakePrefabInternal(string prefabGUID)
        {
            if (repository.IsPrefabGuidExists(prefabGUID))
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

                    UnityEngine.Debug.Log($"Prefab baked: {prefab.name}");

                    UnityEditor.PrefabUtility.UnloadPrefabContents(prefab);
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogError($"Unable to bake prefab: {e.Message}");
                }
            }
        }

        internal void BakeSceneInternal(string sceneGUID)
        {
            if (repository.TryGetSceneBakedDataAsset(sceneGUID, out var bakedDataAsset))
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

                    UnityEngine.Debug.Log($"Scene baked: {scene.name}");

                    if (openScene)
                    {
                        EditorSceneManager.CloseScene(scene, true);
                    }
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogError($"Unable to bake scene: {e.Message}");
                }
            }
        }

        internal void BakeDependentScenesForPrefab(string prefabGUID)
        {
            var dependentScenes = repository.GetSceneDependenciesForPrefab(prefabGUID);

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

        internal void SavePreBakingEditorState() => restorePreBakingState.SaveEditorState();

        internal void RestorePreBakingEditorState() => restorePreBakingState.RestoreEditorState();

        private void SaveAssets()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif
