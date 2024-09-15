#if UNITY_EDITOR
using Scellecs.Morpeh.EntityConverter.Collections;
using Scellecs.Morpeh.EntityConverter.Utilities;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class EntityConverterRepository : IReadOnlyEntityConverterRepository
    {
        public bool IsValid => data != null;
        public bool IsDirty { get; private set; } = false;
        public event Action RepositoryDataChanged;

        private EntityConverterDataAsset data;

        public void Initialize()
        {
            data = AssetDatabase.LoadAssetAtPath<EntityConverterDataAsset>(EntityConverterUtility.DATA_ASSET_PATH);

            if (IsValid)
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
                    AddScene(guid);
                }

                foreach (string guid in prefabAssetsGUIDs)
                {
                    var prefabPath = AssetDatabase.GUIDToAssetPath(guid);
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                    if (prefab != null && prefab.GetComponent<ConvertToEntity>() != null)
                    {
                        AddPrefab(guid);
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

                IsDirty = true;
                SaveDataAndNotifyChanged();
            }
        }

        public void Reload()
        {
            data = AssetDatabase.LoadAssetAtPath<EntityConverterDataAsset>(EntityConverterUtility.DATA_ASSET_PATH);

            if (IsValid)
            {
                RepositoryDataChanged?.Invoke();
            }
        }

        public bool TryGetSceneBakedDataAsset(string sceneGuid, out SceneBakedDataAsset sceneBakedData)
        {
            if (ValidCheck())
            {
                return data.SceneBakedDataAssets.TryGetValue(sceneGuid, out sceneBakedData);
            }

            sceneBakedData = default;
            return false;
        }

        public bool IsSceneGuidExists(string sceneGuid)
        {
            if (ValidCheck())
            {
                return data.SceneGUIDs.Contains(sceneGuid);
            }

            return false;
        }

        public bool IsPrefabGuidExists(string prefabGuid)
        {
            if (ValidCheck())
            {
                return data.AuthoringPrefabGUIDs.Contains(prefabGuid);
            }

            return false;
        }

        public IEnumerable<string> GetSceneGuids()
        {
            if (ValidCheck())
            {
                return data.SceneGUIDs;
            }

            return Array.Empty<string>();
        }

        public IEnumerable<string> GetPrefabGuids()
        {
            if (ValidCheck())
            {
                return data.AuthoringPrefabGUIDs;
            }

            return Array.Empty<string>();
        }

        public IEnumerable<string> GetSceneDependenciesForPrefab(string prefabGUID)
        {
            if (ValidCheck())
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

        public void AddScene(string sceneGUID)
        {
            if (ValidCheck())
            {
                if (data.AssetGUIDInfos.ContainsKey(sceneGUID) == false)
                {
                    data.SceneGUIDs.Add(sceneGUID);
                    data.AssetGUIDInfos.Add(sceneGUID, new AssetGUIDInfo()
                    {
                        type = AuthoringType.Scene,
                        assetGUID = sceneGUID,
                        registrationGUID = sceneGUID
                    });

                    IsDirty = true;
                }
            }
        }

        public void AddPrefab(string prefabGUID)
        {
            if (ValidCheck())
            {
                if (data.AssetGUIDInfos.ContainsKey(prefabGUID) == false)
                {
                    data.AuthoringPrefabGUIDs.Add(prefabGUID);
                    data.AssetGUIDInfos.Add(prefabGUID, new AssetGUIDInfo()
                    {
                        type = AuthoringType.Prefab,
                        assetGUID = prefabGUID,
                        registrationGUID = prefabGUID
                    });

                    IsDirty = true;
                }
            }
        }

        public void AddSceneBakedData(string assetGUID, SceneBakedDataAsset sceneAsset)
        {
            if (ValidCheck())
            {
                if (data.AssetGUIDInfos.ContainsKey(assetGUID) == false)
                {
                    var sceneGUID = sceneAsset.SceneGuid;
                    data.SceneBakedDataAssets.Add(sceneGUID, sceneAsset);
                    data.AssetGUIDInfos.Add(assetGUID, new AssetGUIDInfo()
                    {
                        type = AuthoringType.SceneBakedData,
                        assetGUID = assetGUID,
                        registrationGUID = sceneGUID
                    });

                    IsDirty = true;
                }
            }
        }

        public void RemoveAsset(string GUID)
        {
            if (ValidCheck())
            {
                if (data.AssetGUIDInfos.TryGetValue(GUID, out var assetInfo))
                {
                    var type = assetInfo.type;

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
                    else if (type == AuthoringType.PrefabBakedData)
                    {

                    }

                    IsDirty = true;
                }
            }
        }

        public void AddPrefabToSceneDependency(string prefabGUID, string sceneGUID)
        {
            if (ValidCheck())
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

                IsDirty = true;
            }
        }

        public void RemovePrefabToSceneDependency(string prefabGUID, string sceneGUID)
        {
            if (ValidCheck())
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
                        }

                        IsDirty = true;
                    }
                }
            }
        }

        public void RemovePrefabToAllScenesDependencies(string prefabGUID)
        {
            if (ValidCheck())
            {
                if (data.PrefabToSceneDependencies.ContainsKey(prefabGUID))
                {
                    data.PrefabToSceneDependencies.Remove(prefabGUID);
                    IsDirty = true;
                }
            }
        }

        public void SaveDataAndNotifyChanged()
        {
            if (IsValid == false || IsDirty == false)
            {
                return;
            }

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            RepositoryDataChanged?.Invoke();
            IsDirty = false;
        }

        private bool ValidCheck()
        {
            if (IsValid == false)
            {
                //warning
            }

            return IsValid;
        }
    }
}
#endif
