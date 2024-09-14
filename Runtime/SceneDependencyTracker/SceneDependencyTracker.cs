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
                    }
                }
            }
        }

        private void ChangesPublished(ref ObjectChangeEventStream stream)
        {
            for (int i = 0; i < stream.length; ++i)
            {
                var type = stream.GetEventType(i);
                switch (type)
                {
                    case ObjectChangeKind.CreateGameObjectHierarchy:
                        stream.GetCreateGameObjectHierarchyEvent(i, out var createGameObjectHierarchyEvent);
                        var newGameObject = EditorUtility.InstanceIDToObject(createGameObjectHierarchyEvent.instanceId) as GameObject;
                        Debug.Log($"{type}: {newGameObject} in scene {createGameObjectHierarchyEvent.scene}.");
                        break;

                    case ObjectChangeKind.DestroyGameObjectHierarchy:
                        stream.GetDestroyGameObjectHierarchyEvent(i, out var destroyGameObjectHierarchyEvent);
                        // The destroyed GameObject can not be converted with EditorUtility.InstanceIDToObject as it has already been destroyed.
                        var destroyParentGo = EditorUtility.InstanceIDToObject(destroyGameObjectHierarchyEvent.parentInstanceId) as GameObject;
                        Debug.Log($"{type}: {destroyGameObjectHierarchyEvent.instanceId} with parent {destroyParentGo} in scene {destroyGameObjectHierarchyEvent.scene}.");
                        break;

                    case ObjectChangeKind.CreateAssetObject:
                        stream.GetCreateAssetObjectEvent(i, out var createAssetObjectEvent);
                        var createdAsset = EditorUtility.InstanceIDToObject(createAssetObjectEvent.instanceId);
                        var createdAssetPath = AssetDatabase.GUIDToAssetPath(createAssetObjectEvent.guid);
                        Debug.Log($"{type}: {createdAsset} at {createdAssetPath} in scene {createAssetObjectEvent.scene}.");
                        break;

                    case ObjectChangeKind.DestroyAssetObject:
                        stream.GetDestroyAssetObjectEvent(i, out var destroyAssetObjectEvent);
                        // The destroyed asset can not be converted with EditorUtility.InstanceIDToObject as it has already been destroyed.
                        Debug.Log($"{type}: Instance Id {destroyAssetObjectEvent.instanceId} with Guid {destroyAssetObjectEvent.guid} in scene {destroyAssetObjectEvent.scene}.");
                        break;
                }
            }
        }
    }
}
#endif
