﻿using Scellecs.Morpeh.EntityConverter.Logs;
using Scellecs.Morpeh.EntityConverter.Utilities;
using System.Collections.Generic;
using System;
using UnityEditor;

namespace Scellecs.Morpeh.EntityConverter.Editor
{
    internal sealed class AuthoringDataService : IReadOnlyAuthoringDataService
    {
        private readonly IEntityConverterDataProvider dataProvider;
        private readonly ILogger logger;

        public AuthoringDataService(IEntityConverterDataProvider dataProvider, ILogger logger)
        {
            this.dataProvider = dataProvider;
            this.logger = logger;
        }

        public void Initialize()
        {
            if (dataProvider.TryGetData(out var data, true))
            {
                try
                {
                    data.SceneGUIDs.Clear();
                    data.SceneBakedDataAssets.Clear();
                    data.AuthoringPrefabGUIDs.Clear();
                    data.AssetGUIDInfos.Clear();

                    var sceneGUIDs = SceneUtility.FindAllProjectSceneGuids();
                    var prefabAssetsGUIDs = AssetDatabase.FindAssets("t:Prefab");
                    var sceneBakedDataAssetsGUIDs = AssetDatabase.FindAssets("t:SceneBakedDataAsset");

                    foreach (var guid in sceneGUIDs)
                    {
                        AddAuthoringScene(guid);
                    }

                    foreach (string guid in prefabAssetsGUIDs)
                    {
                        var prefabPath = AssetDatabase.GUIDToAssetPath(guid);
                        var prefab = AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(prefabPath);

                        if (prefab != null && prefab.GetComponent<ConvertToEntity>() != null)
                        {
                            AddAuthoringPrefab(guid);
                        }
                    }

                    foreach (string guid in sceneBakedDataAssetsGUIDs)
                    {
                        var asset = AssetDatabaseUtility.LoadAssetFromGuid<SceneBakedDataAsset>(guid);

                        if (asset != null)
                        {
                            AddSceneBakedData(guid, asset);
                        }
                    }

                    logger.LogInitializationSuccess<AuthoringDataService>();
                }
                catch (System.Exception e)
                {
                    logger.LogInitializationFailedWithException<AuthoringDataService>(e);
                }
            }
            else
            {
                logger.LogInitializationFailedDataAssetNotLoaded<AuthoringDataService>();
            }
        }

        public IEnumerable<string> GetSceneGuids()
        {
            if (dataProvider.TryGetData(out var data))
            {
                return data.SceneGUIDs;
            }

            return Array.Empty<string>();
        }

        public IEnumerable<string> GetPrefabGuids()
        {
            if (dataProvider.TryGetData(out var data))
            {
                return data.AuthoringPrefabGUIDs;
            }

            return Array.Empty<string>();
        }

        public bool TryGetSceneBakedDataAsset(string sceneGuid, out SceneBakedDataAsset sceneBakedData)
        {
            if (dataProvider.TryGetData(out var data))
            {
                return data.SceneBakedDataAssets.TryGetValue(sceneGuid, out sceneBakedData);
            }

            sceneBakedData = default;
            return false;
        }

        public bool IsPrefabGuidExists(string prefabGuid)
        {
            if (dataProvider.TryGetData(out var data))
            {
                return data.AuthoringPrefabGUIDs.Contains(prefabGuid);
            }

            return false;
        }

        public void AddAuthoringScene(string sceneGUID)
        {
            if (string.IsNullOrEmpty(sceneGUID))
            {
                logger.Log("Unable to add authoring scene because an invalid GUID was received.", LogFlags.InternalDebug);
                return;
            }

            if (dataProvider.TryGetData(out var data, true))
            {
                if (data.AssetGUIDInfos.ContainsKey(sceneGUID) == false)
                {
                    logger.Log($"Adding authoring asset. Type: {AuthoringType.Scene}. GUID: {sceneGUID}", LogFlags.InternalDebug);

                    data.SceneGUIDs.Add(sceneGUID);
                    data.AssetGUIDInfos.Add(sceneGUID, new AssetGUIDInfo()
                    {
                        type = AuthoringType.Scene,
                        assetGUID = sceneGUID,
                        registrationGUID = sceneGUID
                    });
                }
            }
        }

        public void AddAuthoringPrefab(string prefabGUID)
        {
            if (string.IsNullOrEmpty(prefabGUID))
            {
                logger.Log("Unable to add authoring preafab because an invalid GUID was received.", LogFlags.InternalDebug);
                return;
            }

            if (dataProvider.TryGetData(out var data, true))
            {
                if (data.AssetGUIDInfos.ContainsKey(prefabGUID) == false)
                {
                    logger.Log($"Adding authoring asset. Type: {AuthoringType.Prefab}. GUID: {prefabGUID}", LogFlags.InternalDebug);

                    data.AuthoringPrefabGUIDs.Add(prefabGUID);
                    data.AssetGUIDInfos.Add(prefabGUID, new AssetGUIDInfo()
                    {
                        type = AuthoringType.Prefab,
                        assetGUID = prefabGUID,
                        registrationGUID = prefabGUID
                    });
                }
            }
        }

        public void AddSceneBakedData(string assetGUID, SceneBakedDataAsset sceneAsset)
        {
            if (string.IsNullOrEmpty(assetGUID))
            {
                logger.Log("Unable to add scene baked data because an invalid GUID was received.", LogFlags.InternalDebug);
            }

            if (dataProvider.TryGetData(out var data, true))
            {
                if (data.AssetGUIDInfos.ContainsKey(assetGUID) == false)
                {
                    logger.Log($"Adding authoring asset. Type: {AuthoringType.SceneBakedData}. GUID: {assetGUID}", LogFlags.InternalDebug);

                    var sceneGUID = sceneAsset.SceneGuid;
                    data.SceneBakedDataAssets.Add(sceneGUID, sceneAsset);
                    data.AssetGUIDInfos.Add(assetGUID, new AssetGUIDInfo()
                    {
                        type = AuthoringType.SceneBakedData,
                        assetGUID = assetGUID,
                        registrationGUID = sceneGUID
                    });
                }
            }
        }

        public void RemoveAuthoringAsset(string GUID)
        {
            if (string.IsNullOrEmpty(GUID))
            {
                logger.Log("Unable to remove authoring asset because an invalid GUID was received.", LogFlags.InternalDebug);
                return;
            }

            if (dataProvider.TryGetData(out var data, true))
            {
                if (data.AssetGUIDInfos.TryGetValue(GUID, out var assetInfo))
                {
                    var type = assetInfo.type;

                    logger.Log($"Removing authoring asset. Type: {type}. GUID: {GUID}", LogFlags.InternalDebug);

                    if (type == AuthoringType.Scene)
                    {
                        data.SceneGUIDs.Remove(assetInfo.registrationGUID);
                        data.AssetGUIDInfos.Remove(assetInfo.assetGUID);

                        if (data.SceneBakedDataAssets.TryGetValue(assetInfo.assetGUID, out var sceneBakedData))
                        {
                            var assetPath = AssetDatabase.GetAssetPath(sceneBakedData);
                            var assetGUID = AssetDatabase.AssetPathToGUID(assetPath);
                            data.SceneBakedDataAssets.Remove(assetInfo.assetGUID);
                            data.AssetGUIDInfos.Remove(assetGUID);
                            AssetDatabase.DeleteAsset(assetPath);
                        }
                    }
                    else if (type == AuthoringType.Prefab)
                    {
                        data.AuthoringPrefabGUIDs.Remove(assetInfo.registrationGUID);
                        data.AssetGUIDInfos.Remove(assetInfo.assetGUID);
                    }
                    else if (type == AuthoringType.SceneBakedData)
                    {
                        data.SceneBakedDataAssets.Remove(assetInfo.registrationGUID);
                        data.AssetGUIDInfos.Remove(assetInfo.assetGUID);
                    }
                }
            }
        }
    }
}
