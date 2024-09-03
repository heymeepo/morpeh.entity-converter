#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class EntityBakingService
    {
        private readonly IReadOnlyEntityConverterRepository repository;
        private readonly BakingProcessor bakingProcessor;

        public EntityBakingService(IReadOnlyEntityConverterRepository repository, BakingProcessor bakingProcessor)
        {
            this.repository = repository;
            this.bakingProcessor = bakingProcessor;
        }

        public void BakeGlobal()
        {
            if (repository.IsValid)
            {
                var prefabGuids = repository.GetPrefabGuids();
                var sceneGuids = repository.GetSceneGuids();

                while (prefabGuids.MoveNext())
                {
                    var guid = prefabGuids.Current;
                    BakePrefab(guid);
                }

                while (sceneGuids.MoveNext()) 
                { 
                    var guid = sceneGuids.Current;
                    BakeScene(guid);
                }
            }
        }

        public void BakePrefab(string prefabGuid)
        {
            if (repository.IsValid)
            {
                if (repository.IsPrefabGuidExists(prefabGuid))
                {
                    var path = AssetDatabase.GUIDToAssetPath(prefabGuid);
                    var prefab = AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(path);
                    AssetDatabase.OpenAsset(prefab.GetInstanceID());
                    var convertToEntity = prefab.GetComponent<ConvertToEntity>();

                    if (convertToEntity != null) 
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
                            AssetDatabase.Refresh();
                            return;
                        }
                    }
                }
            }
        }

        public void BakeScene(string sceneGuid)
        {
            if (repository.IsValid)
            {
                if (repository.TryGetSceneBakedDataAsset(sceneGuid, out var bakedDataAsset))
                {
                    var prevScene = EditorSceneManager.GetActiveScene();
                    var prevScenePath = prevScene.path;
                    var scene = Utilities.SceneUtility.GetSceneFromGUID(sceneGuid);

                    if (scene.IsValid() == false)
                    {
                        if (prevScene.isDirty)
                        {
                            EditorSceneManager.SaveScene(prevScene);
                        }

                        var scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);
                        EditorSceneManager.OpenScene(scenePath);
                        scene = Utilities.SceneUtility.GetSceneFromGUID(sceneGuid);
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
                    AssetDatabase.Refresh();

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
