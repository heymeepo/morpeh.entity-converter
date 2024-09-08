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
            if (repository.IsValid)
            {
                var prefabGuids = repository.GetPrefabGuids();
                var sceneGuids = repository.GetSceneGuids();

                foreach (var guid in prefabGuids)
                {
                    BakePrefab(guid);
                }

                foreach (var guid in sceneGuids)
                {
                    BakeScene(guid);
                }
            }
        }

        public void BakePrefab(string prefabGUID)
        {
            if (repository.IsValid)
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
                            AssetDatabase.SaveAssets();
                        }
                    }

                    PrefabUtility.UnloadPrefabContents(prefab);
                }
            }
        }

        public void BakeScene(string sceneGUID)
        {
            if (repository.IsValid)
            {
                if (repository.TryGetSceneBakedDataAsset(sceneGUID, out var bakedDataAsset))
                {
                    var prevScene = EditorSceneManager.GetActiveScene();
                    var prevScenePath = prevScene.path;
                    var scene = Utilities.SceneUtility.GetSceneFromGUID(sceneGUID);

                    if (scene.IsValid() == false)
                    {
                        if (prevScene.isDirty)
                        {
                            EditorSceneManager.SaveScene(prevScene);
                        }

                        var scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
                        EditorSceneManager.OpenScene(scenePath);
                        scene = Utilities.SceneUtility.GetSceneFromGUID(sceneGUID);
                    }

                    var info = new SceneBakingInfo()
                    {
                        scene = scene,
                        bakedData = bakedDataAsset
                    };

                    bakingProcessor.ExecuteSceneBake(info);

                    if (scene.isDirty)
                    {
                        EditorSceneManager.SaveScene(scene);
                    }

                    AssetDatabase.SaveAssets();

                    if (string.IsNullOrEmpty(prevScenePath) == false && prevScenePath != "Null")
                    {
                        EditorSceneManager.OpenScene(prevScenePath);
                    }
                }
            }
        }
    }
}
#endif
