#if UNITY_EDITOR
using Scellecs.Morpeh.EntityConverter.Utilities;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.VersionControl;
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
            Reload();

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

        public void Reload() => data = AssetDatabase.LoadAssetAtPath<EntityConverterDataAsset>(EntityConverterUtility.DATA_ASSET_PATH);

        public bool TryGetSceneBakedDataAsset(string sceneGuid, out SceneBakedDataAsset sceneBakedData)
        {
            if (IsValid == false)
            {
                //exception
            }

            return data.SceneBakedDataAssets.TryGetValue(sceneGuid, out sceneBakedData);
        }

        public bool IsSceneGuidExists(string sceneGuid)
        {
            if (IsValid == false)
            {
                //exception
            }

            return data.SceneGUIDs.Contains(sceneGuid);
        }

        public bool IsPrefabGuidExists(string prefabGuid)
        {
            if (IsValid == false)
            {
                //exception
            }

            return data.AuthoringPrefabGUIDs.Contains(prefabGuid);
        }

        public IEnumerable<string> GetSceneGuids()
        {
            if (IsValid == false)
            {
                //exception
            }

            return data.SceneGUIDs;
        }

        public IEnumerable<string> GetPrefabGuids()
        {
            if (IsValid == false)
            {
                //exception
            }

            return data.AuthoringPrefabGUIDs;
        }

        public void AddScene(string sceneGUID)
        {
            if (IsValid == false)
            {
                //exception
            }

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

        public void AddPrefab(string prefabGUID)
        {
            if (IsValid == false)
            {
                //exception
            }

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

        public void AddSceneBakedData(string assetGUID, SceneBakedDataAsset sceneAsset)
        {
            if (IsValid == false)
            {
                //exception
            }

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

        public void RemoveAsset(string GUID)
        {
            if (IsValid == false)
            {
                //exception
            }

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
        }
    }
}
#endif
