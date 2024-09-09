#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class AutoRebakingPostprocessor : IAssetPostprocessSystem
    {
        private readonly AuthoringBakingService bakingService;

        public AutoRebakingPostprocessor(AuthoringBakingService bakingService)
        {
            this.bakingService = bakingService;
            ObjectChangeEvents.changesPublished += ChangesPublished;
        }

        public void Execute(OnAssetPostprocessContext context)
        {
            //if (context.DidDomainReload)
            //{
            //    bakingService.ForceGlobalBake();
            //    return;
            //}

            foreach (var importedAuthoringData in context.ImportedAuthorings)
            {
                switch (importedAuthoringData.type)
                {
                    case AuthoringType.Prefab:
                        RebakePrefab(importedAuthoringData.GUID);
                        break;

                    case AuthoringType.Scene:
                        RebakeScene(importedAuthoringData.GUID);
                        break;
                }
            }
        }

        private void RebakePrefab(string prefabGUID)
        {
            bakingService.BakePrefab(prefabGUID);
        }

        private void RebakeScene(string sceneGUID)
        {
            bakingService.BakeScene(sceneGUID);
        }

        private void ChangesPublished(ref ObjectChangeEventStream stream)
        {
            for (int i = 0; i < stream.length; ++i)
            {
                var type = stream.GetEventType(i);
                switch (type)
                {
                    case ObjectChangeKind.ChangeScene:
                        stream.GetChangeSceneEvent(i, out var changeSceneEvent);
                        Debug.Log($"{type}: {changeSceneEvent.scene}");
                        break;

                    case ObjectChangeKind.CreateGameObjectHierarchy:
                        stream.GetCreateGameObjectHierarchyEvent(i, out var createGameObjectHierarchyEvent);
                        var newGameObject = EditorUtility.InstanceIDToObject(createGameObjectHierarchyEvent.instanceId) as GameObject;
                        Debug.Log($"{type}: {newGameObject} in scene {newGameObject.scene.path}.");
                        break;

                    case ObjectChangeKind.ChangeGameObjectStructureHierarchy:
                        stream.GetChangeGameObjectStructureHierarchyEvent(i, out var changeGameObjectStructureHierarchy);
                        var gameObject = EditorUtility.InstanceIDToObject(changeGameObjectStructureHierarchy.instanceId) as GameObject;
                        Debug.Log($"{type}: {gameObject} in scene asdasda {gameObject.scene.path}.");
                        break;

                    case ObjectChangeKind.ChangeGameObjectStructure:
                        stream.GetChangeGameObjectStructureEvent(i, out var changeGameObjectStructure);
                        var gameObjectStructure = EditorUtility.InstanceIDToObject(changeGameObjectStructure.instanceId) as GameObject;
                        Debug.Log($"{type}: {gameObjectStructure} in scene asdasd {gameObjectStructure.scene.path}.");
                        break;

                    case ObjectChangeKind.ChangeGameObjectParent:
                        stream.GetChangeGameObjectParentEvent(i, out var changeGameObjectParent);
                        var gameObjectChanged = EditorUtility.InstanceIDToObject(changeGameObjectParent.instanceId) as GameObject;
                        var newParentGo = EditorUtility.InstanceIDToObject(changeGameObjectParent.newParentInstanceId) as GameObject;
                        var previousParentGo = EditorUtility.InstanceIDToObject(changeGameObjectParent.previousParentInstanceId) as GameObject;
                        Debug.Log($"{type}: {gameObjectChanged} from {previousParentGo} to {newParentGo} from scene {changeGameObjectParent.previousScene} to scene {changeGameObjectParent.newScene}.");
                        break;

                    case ObjectChangeKind.ChangeGameObjectOrComponentProperties:
                        stream.GetChangeGameObjectOrComponentPropertiesEvent(i, out var changeGameObjectOrComponent);
                        var goOrComponent = EditorUtility.InstanceIDToObject(changeGameObjectOrComponent.instanceId);
                        if (goOrComponent is GameObject go)
                        {
                            Debug.Log($"{type}: GameObject {go} change properties in scene {changeGameObjectOrComponent.scene}.");
                        }
                        else if (goOrComponent is Component component)
                        {
                            Debug.Log($"{type}: Component {component} change properties in scene {changeGameObjectOrComponent.scene}.");
                        }
                        break;

                    case ObjectChangeKind.DestroyGameObjectHierarchy:
                        stream.GetDestroyGameObjectHierarchyEvent(i, out var destroyGameObjectHierarchyEvent);
                        // The destroyed GameObject can not be converted with EditorUtility.InstanceIDToObject as it has already been destroyed.
                        var destroyParentGo = EditorUtility.InstanceIDToObject(destroyGameObjectHierarchyEvent.parentInstanceId) as GameObject;
                        Debug.Log($"{type}: {destroyGameObjectHierarchyEvent.instanceId} with parent {destroyParentGo} in scene {destroyGameObjectHierarchyEvent.scene.path}.");
                        break;

                    case ObjectChangeKind.CreateAssetObject:
                        stream.GetCreateAssetObjectEvent(i, out var createAssetObjectEvent);
                        var createdAsset = EditorUtility.InstanceIDToObject(createAssetObjectEvent.instanceId);
                        var createdAssetPath = AssetDatabase.GUIDToAssetPath(createAssetObjectEvent.guid);
                        Debug.Log($"{type}: {createdAsset} at {createdAssetPath} in scene {createAssetObjectEvent.scene.path}.");
                        break;

                    case ObjectChangeKind.DestroyAssetObject:
                        stream.GetDestroyAssetObjectEvent(i, out var destroyAssetObjectEvent);
                        // The destroyed asset can not be converted with EditorUtility.InstanceIDToObject as it has already been destroyed.
                        Debug.Log($"{type}: Instance Id {destroyAssetObjectEvent.instanceId} with Guid {destroyAssetObjectEvent.guid} in scene {destroyAssetObjectEvent.scene.path}.");
                        break;

                    case ObjectChangeKind.ChangeAssetObjectProperties:
                        stream.GetChangeAssetObjectPropertiesEvent(i, out var changeAssetObjectPropertiesEvent);
                        var changeAsset = EditorUtility.InstanceIDToObject(changeAssetObjectPropertiesEvent.instanceId);
                        var changeAssetPath = AssetDatabase.GUIDToAssetPath(changeAssetObjectPropertiesEvent.guid);
                        Debug.Log($"{type}: {changeAsset} at {changeAssetPath} in scene {changeAssetObjectPropertiesEvent.scene}.");
                        break;

                    case ObjectChangeKind.UpdatePrefabInstances:
                        stream.GetUpdatePrefabInstancesEvent(i, out var updatePrefabInstancesEvent);
                        string s = "";
                        s += $"{type}: scene {updatePrefabInstancesEvent.scene.path}. Instances ({updatePrefabInstancesEvent.instanceIds.Length}):\n";
                        foreach (var prefabId in updatePrefabInstancesEvent.instanceIds)
                        {
                            s += EditorUtility.InstanceIDToObject(prefabId).ToString() + "\n";
                        }
                        Debug.Log(s);
                        break;
                }
            }
        }
    }
}
#endif
