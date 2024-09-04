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

        public void Initialize() => data = AssetDatabase.LoadAssetAtPath<EntityConverterDataAsset>(EntityConverterUtility.ASSET_PATH);

        public void Reload()
        {
            if (IsValid)
            {
                data.SceneGuids.Clear();
                data.SceneBakedDataAssets.Clear();

                foreach (var sceneGuid in SceneUtility.FindAllProjectSceneGuids())
                {
                    data.SceneGuids.Add(sceneGuid);
                }

                var sceneAssetsGuids = AssetDatabase.FindAssets("t:SceneBakedDataAsset");

                foreach (string guid in sceneAssetsGuids)
                {
                    var asset = AssetDatabaseUtility.LoadAssetFromGuid<SceneBakedDataAsset>(guid);

                    if (asset != null)
                    {
                        data.SceneBakedDataAssets.Add(asset.SceneGuid, asset);
                    }
                }

                data.AuthoringPrefabsGuids.Clear();
                var allPrefabs = AssetDatabase.FindAssets("t:Prefab");

                foreach (string prefabGuid in allPrefabs)
                {
                    var prefabPath = AssetDatabase.GUIDToAssetPath(prefabGuid);
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                    if (prefab != null && prefab.GetComponent<ConvertToEntity>() != null)
                    {
                        data.AuthoringPrefabsGuids.Add(prefabGuid);
                    }
                }

                SaveDataAndNotifyChanged();
            }
        }

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

            return data.SceneGuids.Contains(sceneGuid);
        }

        public bool IsPrefabGuidExists(string prefabGuid) 
        {
            if (IsValid == false)
            {
                //exception
            }

            return data.AuthoringPrefabsGuids.Contains(prefabGuid);
        }

        public IEnumerator<string> GetSceneGuids()
        {
            if (IsValid == false)
            {
                //exception
            }

            return data.SceneGuids.GetEnumerator();
        }

        public IEnumerator<string> GetPrefabGuids()
        {
            if (IsValid == false)
            {
                //exception
            }

            return data.AuthoringPrefabsGuids.GetEnumerator();
        }

        public void AddSceneBakedDataAsset(SceneBakedDataAsset asset, string assetGuid)
        {
            if (IsValid == false)
            {
                //exception
            }

            data.SceneBakedDataAssets.Add(asset.SceneGuid, asset);
        }

        public void AddSceneGuid(string sceneGuid)
        {
            if (IsValid == false)
            {
                //exception
            }

            data.SceneGuids.Add(sceneGuid);
        }

        public bool CollectUnreferenced()
        {
            if (IsValid == false)
            {
                //exception
            }

            var sceneBakedDataAssets = data.SceneBakedDataAssets;
            var assetsToRemoveKeys = new List<string>();
            var sceneGuidsToRemove = new List<string>();

            foreach (var guid in data.SceneGuids)
            {
                if (AssetDatabaseUtility.IsAssetExistsFromGuid<SceneAsset>(guid) == false)
                {
                    sceneGuidsToRemove.Add(guid);
                }
            }

            foreach (var guid in sceneGuidsToRemove)
            {
                data.SceneGuids.Remove(guid);

                if (TryGetSceneBakedDataAsset(guid, out var asset))
                {
                    var assetPath = AssetDatabase.GetAssetPath(asset);
                    data.SceneBakedDataAssets.Remove(guid);
                    AssetDatabase.DeleteAsset(assetPath);
                }
            }

            foreach (var kvp in sceneBakedDataAssets)
            {
                if (kvp.Value == null)
                {
                    assetsToRemoveKeys.Add(kvp.Key);
                }
            }

            foreach (var key in assetsToRemoveKeys)
            {
                sceneBakedDataAssets.Remove(key);
            }

            return (assetsToRemoveKeys.Count + sceneGuidsToRemove.Count) > 0;
        }

        public void SaveDataAndNotifyChanged()
        {
            if (IsValid == false)
            {
                //exception
            }

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            RepositoryDataChanged?.Invoke();
        }
    }
}
#endif
