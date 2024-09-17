#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
using Scellecs.Morpeh.EntityConverter.Logs;
using Scellecs.Morpeh.EntityConverter.Collections;
using System.Collections.Generic;
using System;

namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class SceneDependencyService : IReadOnlySceneDependencyService
    {
        private readonly IEntityConverterDataProvider dataProvider;
        private readonly IReadOnlyAuthoringDataService authoringDataService;
        private readonly ILogger logger;

        public SceneDependencyService(IEntityConverterDataProvider dataProvider, IReadOnlyAuthoringDataService authoringDataService, ILogger logger)
        {
            this.dataProvider = dataProvider;
            this.authoringDataService = authoringDataService;
            this.logger = logger;
        }

        public void Intialize()
        {
            if (dataProvider.IsValid())
            {
                try
                {
                    var sceneGUIDs = authoringDataService.GetSceneGuids();

                    foreach (var guid in sceneGUIDs)
                    {
                        var scene = Utilities.SceneUtility.GetSceneFromGUID(guid);
                        var openScene = scene.IsValid() == false;

                        if (openScene)
                        {
                            var scenePath = AssetDatabase.GUIDToAssetPath(guid);
                            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                            scene = Utilities.SceneUtility.GetSceneFromGUID(guid);
                        }

                        var rootObjects = scene.GetRootGameObjects();

                        foreach (var root in rootObjects)
                        {
                            var transforms = root.GetComponentsInChildren<UnityEngine.Transform>(true);
                            foreach (var transform in transforms)
                            {
                                var gameObject = transform.gameObject;
                                if (UnityEditor.PrefabUtility.IsPartOfPrefabInstance(gameObject))
                                {
                                    var prefabAsset = UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
                                    if (prefabAsset != null)
                                    {
                                        var prefabPath = AssetDatabase.GetAssetPath(prefabAsset);
                                        var prefabGuid = AssetDatabase.AssetPathToGUID(prefabPath);
                                        AddPrefabToSceneDependency(prefabGuid, guid);
                                    }
                                    else
                                    {
                                        var prefabGuid = SceneDependencyTrackerUtility.ExtractGuidFromMissingPrefabName(gameObject.name);

                                        if (guid != null)
                                        {
                                            AddPrefabToSceneDependency(prefabGuid, guid);
                                        }
                                    }
                                }
                            }
                        }

                        if (openScene)
                        {
                            EditorSceneManager.CloseScene(scene, true);
                        }

                        logger.LogInitializationSuccess<SceneDependencyService>();
                    }
                }
                catch (System.Exception e)
                {
                    logger.LogInitializationFailedWithException<SceneDependencyService>(e);
                }
            }
            else
            {
                logger.LogInitializationFailedDataAssetNotLoaded<SceneDependencyService>();
            }
        }

        public IEnumerable<string> GetSceneDependenciesForPrefab(string prefabGUID)
        {
            if (dataProvider.TryGetData(out var data))
            {
                if (data.PrefabToSceneDependencies.TryGetValue(prefabGUID, out var dependecyInfo))
                {
                    var sceneGUIDs = new List<string>();

                    foreach (var sceneGUID in dependecyInfo.refCountPerScene.Keys)
                    {
                        sceneGUIDs.Add(sceneGUID);
                    }

                    return sceneGUIDs;
                }
            }

            return Array.Empty<string>();
        }

        public void AddPrefabToSceneDependency(string prefabGUID, string sceneGUID)
        {
            if (string.IsNullOrEmpty(prefabGUID) || string.IsNullOrEmpty(sceneGUID))
            {
                logger.LogWarning($"Unable to ADD the prefab as a scene dependency because an invalid GUID was received. Scene GUID: {sceneGUID}. Prefab GUID: {prefabGUID}.", LogDepthFlags.InternalDebug);
                return;
            }

            if (dataProvider.TryGetData(out var data))
            {
                if (data.PrefabToSceneDependencies.TryGetValue(prefabGUID, out var dependencyInfo))
                {
                    if (dependencyInfo.refCountPerScene.TryGetValue(sceneGUID, out int refCount))
                    {
                        dependencyInfo.refCountPerScene[sceneGUID] = refCount + 1;
                    }
                    else
                    {
                        dependencyInfo.refCountPerScene.Add(sceneGUID, 1);
                    }
                }
                else
                {
                    data.PrefabToSceneDependencies.Add(prefabGUID, new PrefabSceneDependencyInfo()
                    {
                        refCountPerScene = new SerializableDictionary<string, int>()
                        {
                            [sceneGUID] = 1
                        }
                    });
                }
            }
        }

        public void RemovePrefabToSceneDependency(string prefabGUID, string sceneGUID)
        {
            if (string.IsNullOrEmpty(prefabGUID) || string.IsNullOrEmpty(sceneGUID))
            {
                logger.LogWarning($"Unable to REMOVE the prefab as a scene dependency because an invalid GUID was received. Scene GUID: {sceneGUID}. Prefab GUID: {prefabGUID}.", LogDepthFlags.InternalDebug);
                return;
            }

            if (dataProvider.TryGetData(out var data))
            {
                if (data.PrefabToSceneDependencies.TryGetValue(prefabGUID, out var dependencyInfo))
                {
                    if (dependencyInfo.refCountPerScene.TryGetValue(sceneGUID, out int refCount))
                    {
                        refCount--;

                        if (refCount > 0)
                        {
                            dependencyInfo.refCountPerScene[sceneGUID] = refCount;
                        }
                        else
                        {
                            dependencyInfo.refCountPerScene.Remove(sceneGUID);

                            if (dependencyInfo.refCountPerScene.Count == 0)
                            {
                                data.PrefabToSceneDependencies.Remove(prefabGUID);
                            }
                        }
                    }
                }
            }
        }
    }
}
#endif
