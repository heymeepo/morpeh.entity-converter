#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using System;
using Scellecs.Morpeh.EntityConverter.Utilities;
using System.IO;

namespace Scellecs.Morpeh.EntityConverter
{
    public class EntityConverterDatabase : ScriptableObject
    {
        public const string ASSET_PATH = "Assets/Plugins/Scellecs/Morpeh Entity Converter/Assets/EntityConverterDatabaseAsset.asset";

        [SerializeField]
        internal List<SceneBakedDataAsset> sceneBakedDataAssets = new List<SceneBakedDataAsset>();

        [SerializeField]
        internal List<string> sceneGuids = new List<string>();

        public event Action DatabaseChanged;

        public static EntityConverterDatabase GetInstance() => AssetDatabase.LoadAssetAtPath<EntityConverterDatabase>(ASSET_PATH);

        public static EntityConverterDatabase CreateInstance()
        {
            var dataBase = GetInstance();

            if (dataBase == null)
            {
                string folderPath = "Assets/Plugins/Scellecs/Morpeh Entity Converter/Assets";
                string[] folders = folderPath.Split('/');

                string currentPath = "";

                foreach (string folder in folders)
                {
                    if (string.IsNullOrEmpty(currentPath) == false)
                    {
                        currentPath += "/";
                    }

                    currentPath += folder;

                    if (AssetDatabase.IsValidFolder(currentPath) == false)
                    {
                        string parentDirectory = Path.GetDirectoryName(currentPath);
                        string newFolderName = Path.GetFileName(currentPath);

                        AssetDatabase.CreateFolder(parentDirectory, newFolderName);
                    }
                }

                var database = ScriptableObject.CreateInstance<EntityConverterDatabase>();
                dataBase.RefreshDatabase();

                AssetDatabase.CreateAsset(database, EntityConverterDatabase.ASSET_PATH);
                AssetDatabase.SaveAssets();
            }

            return dataBase;
        }

        public bool TryGetSceneBakedDataForScene(string sceneGuid, out SceneBakedDataAsset asset)
        {
            asset = sceneBakedDataAssets.Where(s => s.sceneGuid == sceneGuid).FirstOrDefault();
            return asset != null;
        }

        public SceneBakedDataAsset CreateSceneBakedDataAsset(string scenePath)
        {
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);

            if (sceneAsset == null)
            {
                return null;
            }

            string directoryPath = Path.GetDirectoryName(scenePath);
            string[] folders = directoryPath.Split('/');
            string currentPath = "";

            foreach (string folder in folders)
            {
                if (!string.IsNullOrEmpty(currentPath))
                {
                    currentPath += "/";
                }

                currentPath += folder;

                if (!AssetDatabase.IsValidFolder(currentPath))
                {
                    string parentDirectory = Path.GetDirectoryName(currentPath);
                    string newFolderName = Path.GetFileName(currentPath);

                    AssetDatabase.CreateFolder(parentDirectory, newFolderName);
                }
            }

            string sceneFolderName = Path.GetFileNameWithoutExtension(scenePath);
            currentPath += "/" + sceneFolderName;

            if (!AssetDatabase.IsValidFolder(currentPath))
            {
                string parentDirectory = Path.GetDirectoryName(currentPath);
                AssetDatabase.CreateFolder(parentDirectory, sceneFolderName);
            }

            string assetName = sceneFolderName + "BakedData.asset";
            string assetPath = Path.Combine(currentPath, assetName).Replace("\\", "/");

            var sceneBakedData = ScriptableObject.CreateInstance<SceneBakedDataAsset>();
            sceneBakedData.sceneGuid = AssetDatabase.GUIDFromAssetPath(scenePath).ToString();

            AssetDatabase.CreateAsset(sceneBakedData, assetPath);
            AssetDatabase.SaveAssets();

            return sceneBakedData;
        }

        internal void AddSceneBakedDataAsset(SceneBakedDataAsset asset)
        {
            if (sceneBakedDataAssets.Contains(asset) == false)
            {
                sceneBakedDataAssets.Add(asset);
            }
        }

        internal void RemoveSceneBakedDataAssetForScene(string sceneGuid)
        {
            if (TryGetSceneBakedDataForScene(sceneGuid, out var asset))
            {
                var assetPath = AssetDatabase.GetAssetPath(asset);
                sceneBakedDataAssets.Remove(asset);
                AssetDatabase.DeleteAsset(assetPath);
            }
        }

        internal void AddSceneGuid(string sceneGuid)
        {
            if (sceneGuids.Contains(sceneGuid) == false)
            {
                sceneGuids.Add(sceneGuid);
            }
        }

        internal bool ClearUnreferenced()
        {
            var changed = false;

            for (int i = sceneBakedDataAssets.Count - 1; i >= 0; i--)
            {
                var asset = sceneBakedDataAssets[i];

                if (asset == null)
                {
                    sceneBakedDataAssets.RemoveAt(i);
                    changed = true;
                }
            }

            for (int i = sceneGuids.Count - 1; i >= 0; i--)
            {
                var sceneGuid = sceneGuids[i];
                var sceneAsset = AssetDatabaseUtils.LoadAssetFromGUID<SceneAsset>(sceneGuid);
                
                if (sceneAsset == null)
                {
                    sceneGuids.RemoveAt(i);
                    RemoveSceneBakedDataAssetForScene(sceneGuid);
                    changed = true;
                }
            }

            return changed;
        }

        internal void RefreshDatabase()
        {
            sceneGuids = new List<string>(SceneUtils.FindAllProjectSceneGuids());
            sceneBakedDataAssets.Clear();

            var guids = AssetDatabase.FindAssets("t:SceneBakedDataAsset");

            foreach (string guid in guids)
            {
                var asset = AssetDatabaseUtils.LoadAssetFromGUID<SceneBakedDataAsset>(guid);

                if (asset != null)
                {
                    sceneBakedDataAssets.Add(asset);
                }
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();

            RaiseDatabaseChanged();
        }

        internal void RaiseDatabaseChanged() => DatabaseChanged?.Invoke();
    }
}
#endif
