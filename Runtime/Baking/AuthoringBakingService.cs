#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class AuthoringBakingService : IAuthoringBakingService
    {
        private readonly IReadOnlyEntityConverterRepository repository;
        private readonly BakingProcessor bakingProcessor;

        public AuthoringBakingService(IReadOnlyEntityConverterRepository repository, BakingProcessor bakingProcessor)
        {
            this.repository = repository;
            this.bakingProcessor = bakingProcessor;
        }

        public void ForceGlobalBake()
        {
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
            }
        }

        public void BakePrefab(string prefabGUID)
        {
            SaveDirtyBeforeBaking();

            if (repository.IsValid)
            {
                BakePrefabInternal(prefabGUID);
                SaveAssets();
            }
        }

        public void BakeScene(string sceneGUID)
        {
            SaveDirtyBeforeBaking();

            if (repository.IsValid)
            {
                BakeSceneInternal(sceneGUID);
                SaveAssets();
            }
        }

        internal void BakePrefabInternal(string prefabGUID)
        {
            if (repository.IsPrefabGuidExists(prefabGUID))
            {
                var path = AssetDatabase.GUIDToAssetPath(prefabGUID);
                var prefab = PrefabUtility.LoadPrefabContents(path);

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

                PrefabUtility.UnloadPrefabContents(prefab);
            }
        }

        internal void BakeSceneInternal(string sceneGUID)
        {
            if (repository.TryGetSceneBakedDataAsset(sceneGUID, out var bakedDataAsset))
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

                if (openScene)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }
            }
        }

        private void SaveDirtyBeforeBaking()
        {
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

            if (prefabStage != null)
            {
                PrefabUtility.SaveAsPrefabAsset(prefabStage.prefabContentsRoot, prefabStage.assetPath);
                prefabStage.ClearDirtiness();
            }

            EditorSceneManager.MarkAllScenesDirty();
            EditorSceneManager.SaveOpenScenes();

            SaveAssets();
        }

        private void SaveAssets()
        { 
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif
