#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class SceneDependencyTracker
    {
        private readonly EntityConverterRepository repository;
        private Dictionary<int, string> instanceIdToGUID;

        private UnityEngine.SceneManagement.Scene activeScene;
        private string activeSceneGUID;

        public SceneDependencyTracker(EntityConverterRepository repository)
        {
            this.repository = repository;
        }

        public void Initialize()
        {
            EditorSceneManager.activeSceneChangedInEditMode += (s1, s2) => ChangeActiveScene();
            ChangeActiveScene();
            ObjectChangeEvents.changesPublished += ChangesPublished;
        }

        private void ChangeActiveScene()
        {
            activeScene = EditorSceneManager.GetActiveScene();
            activeSceneGUID = Utilities.SceneUtility.GetSceneGUID(activeScene);
            MapScenePrefabInstanceIDsToGUID();
        }

        private void MapScenePrefabInstanceIDsToGUID()
        {
            instanceIdToGUID = new Dictionary<int, string>();
            var rootObjects = activeScene.GetRootGameObjects();

            foreach (var root in rootObjects)
            {
                var transforms = root.GetComponentsInChildren<Transform>(true);
                foreach (var transform in transforms)
                {
                    var gameObject = transform.gameObject;
                    if (PrefabUtility.IsPartOfPrefabInstance(gameObject))
                    {
                        var prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
                        if (prefabAsset != null)
                        {
                            var prefabPath = AssetDatabase.GetAssetPath(prefabAsset);
                            var guid = AssetDatabase.AssetPathToGUID(prefabPath);
                            var instanceId = gameObject.GetInstanceID();

                            if (instanceIdToGUID.ContainsKey(instanceId) == false)
                            {
                                instanceIdToGUID[instanceId] = guid;
                            }
                        }
                        else
                        {
                            var guid = SceneDependencyTrackerUtility.ExtractGuidFromMissingPrefabName(gameObject.name);
                            var instanceId = gameObject.GetInstanceID();

                            if (guid != null && instanceIdToGUID.ContainsKey(gameObject.GetInstanceID()) == false)
                            {
                                instanceIdToGUID[instanceId] = guid;
                            }
                        }
                    }
                }
            }
        }

        private void ChangesPublished(ref ObjectChangeEventStream stream)
        {
            if (repository.IsValid == false)
            {
                return;
            }

            for (int i = 0; i < stream.length; i++)
            {
                var type = stream.GetEventType(i);

                if (type == ObjectChangeKind.CreateGameObjectHierarchy)
                {
                    stream.GetCreateGameObjectHierarchyEvent(i, out var createGameObjectHierarchyEvent);
                    if (Utilities.SceneUtility.GetSceneGUID(createGameObjectHierarchyEvent.scene) != activeSceneGUID)
                    {
                        continue;
                    }

                    var newGameObject = EditorUtility.InstanceIDToObject(createGameObjectHierarchyEvent.instanceId) as GameObject;
                    if (PrefabUtility.IsPartOfPrefabInstance(newGameObject))
                    {
                        var prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(newGameObject);
                        if (prefabAsset != null)
                        {
                            var prefabPath = AssetDatabase.GetAssetPath(prefabAsset);
                            var prefabGUID = AssetDatabase.AssetPathToGUID(prefabPath);
                            instanceIdToGUID.Add(createGameObjectHierarchyEvent.instanceId, prefabGUID);
                            repository.AddPrefabToSceneDependency(prefabGUID, activeSceneGUID);
                        }
                    }
                }
                else if (type == ObjectChangeKind.DestroyGameObjectHierarchy)
                {
                    stream.GetDestroyGameObjectHierarchyEvent(i, out var destroyGameObjectHierarchyEvent);
                    if (Utilities.SceneUtility.GetSceneGUID(destroyGameObjectHierarchyEvent.scene) != activeSceneGUID)
                    {
                        continue;
                    }

                    if (instanceIdToGUID.TryGetValue(destroyGameObjectHierarchyEvent.instanceId, out var prefabGUID))
                    {
                        if (instanceIdToGUID.ContainsKey(destroyGameObjectHierarchyEvent.instanceId))
                        {
                            instanceIdToGUID.Remove(destroyGameObjectHierarchyEvent.instanceId);
                        }

                        repository.RemovePrefabToSceneDependency(prefabGUID, activeSceneGUID);
                    }

                }
            }

            repository.SaveDataAndNotifyChanged();
        }
    }
}
#endif
