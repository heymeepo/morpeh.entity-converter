#if UNITY_EDITOR
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
                    var assetInfo = new AssetGUIDInfo()
                    {
                        assetGUID = guid,
                        registrationGUID = guid,
                        type = AssetGUIDType.Scene
                    };

                    AddAsset(null, assetInfo);
                }

                foreach (string guid in sceneBakedDataAssetsGUIDs)
                {
                    var asset = AssetDatabaseUtility.LoadAssetFromGuid<SceneBakedDataAsset>(guid);

                    if (asset != null)
                    {
                        var assetInfo = new AssetGUIDInfo()
                        {
                            assetGUID = guid,
                            registrationGUID = asset.SceneGuid,
                            type = AssetGUIDType.SceneBakedData
                        };

                        AddAsset(asset, assetInfo);
                    }
                }

                foreach (string guid in prefabAssetsGUIDs)
                {
                    var prefabPath = AssetDatabase.GUIDToAssetPath(guid);
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                    if (prefab != null && prefab.GetComponent<ConvertToEntity>() != null)
                    {
                        var assetInfo = new AssetGUIDInfo()
                        {
                            assetGUID = guid,
                            registrationGUID = guid,
                            type = AssetGUIDType.AuthoringPrefab
                        };

                        AddAsset(null, assetInfo);
                    }
                }

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

        public IEnumerator<string> GetSceneGuids()
        {
            if (IsValid == false)
            {
                //exception
            }

            return data.SceneGUIDs.GetEnumerator();
        }

        public IEnumerator<string> GetPrefabGuids()
        {
            if (IsValid == false)
            {
                //exception
            }

            return data.AuthoringPrefabGUIDs.GetEnumerator();
        }

        public void AddAsset(UnityEngine.Object asset, AssetGUIDInfo assetInfo)
        {
            if (IsValid == false)
            {
                //exception
            }

            var type = assetInfo.type;

            if (data.AssetGUIDInfos.ContainsKey(assetInfo.assetGUID) == false)
            {
                if (type == AssetGUIDType.Scene)
                {
                    data.SceneGUIDs.Add(assetInfo.registrationGUID);
                    data.AssetGUIDInfos.Add(assetInfo.assetGUID, assetInfo);
                }
                else if (type == AssetGUIDType.AuthoringPrefab)
                {
                    data.AuthoringPrefabGUIDs.Add(assetInfo.registrationGUID);
                    data.AssetGUIDInfos.Add(assetInfo.assetGUID, assetInfo);
                }
                else if (type == AssetGUIDType.SceneBakedData)
                {
                    data.SceneBakedDataAssets.Add(assetInfo.registrationGUID, asset as SceneBakedDataAsset);
                    data.AssetGUIDInfos.Add(assetInfo.assetGUID, assetInfo);
                }
                else if (type == AssetGUIDType.PrefabBakedData)
                {

                }
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

                if (type == AssetGUIDType.Scene)
                {
                    data.SceneGUIDs.Remove(assetInfo.registrationGUID);
                    data.AssetGUIDInfos.Remove(assetInfo.assetGUID);

                    if (data.SceneBakedDataAssets.TryGetValue(assetInfo.assetGUID, out var sceneBakedData))
                    {
                        var assetPath = AssetDatabase.GetAssetPath(sceneBakedData);
                        data.SceneBakedDataAssets.Remove(assetInfo.assetGUID);
                        AssetDatabase.DeleteAsset(assetPath);
                    }
                }
                else if (type == AssetGUIDType.AuthoringPrefab)
                {
                    data.AuthoringPrefabGUIDs.Remove(assetInfo.registrationGUID);
                    data.AssetGUIDInfos.Remove(assetInfo.assetGUID);
                }
                else if (type == AssetGUIDType.SceneBakedData)
                {
                    data.SceneBakedDataAssets.Remove(assetInfo.registrationGUID);
                    data.AssetGUIDInfos.Remove(assetInfo.assetGUID);
                }
                else if (type == AssetGUIDType.PrefabBakedData)
                {

                }
            }
        }

        public void SaveDataAndNotifyChanged()
        {
            if (IsValid == false)
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
