#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter.Utilities
{
    public static class AssetDatabaseUtility
    {
        public static bool HasChildAsset(Object parent, Object child)
        {
            var assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(parent));

            foreach (var a in assets)
            {
                if (a == child)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool TryAddChildAsset(Object parent, Object child)
        {
            if (!HasChildAsset(parent, child))
            {
                AssetDatabase.AddObjectToAsset(child, parent);
                return true;
            }

            return false;
        }

        public static void RemoveChildAssets(Object parent)
        {
            var assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(parent));

            foreach (var a in assets)
            {
                if (a != parent)
                {
                    AssetDatabase.RemoveObjectFromAsset(a);
                }
            }
        }

        public static List<Object> GetAllChildAssets(Object parent)
        {
            var assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(parent));
            var childAssets = new List<Object>();

            foreach (var a in assets)
            {
                if (a != parent)
                {
                    childAssets.Add(a);
                }
            }

            return childAssets;
        }

        public static bool IsAssetExists(string path, System.Type type) => AssetDatabase.LoadAssetAtPath(path, type) != null;

        public static bool IsAssetExists<T>(string path) where T : Object => AssetDatabase.LoadAssetAtPath<T>(path) != null;

        public static bool IsAssetExistsFromGuid<T>(string guid) where T : Object => LoadAssetFromGuid<T>(guid) != null;

        public static T LoadAssetFromGuid<T>(string guid) where T : Object
        { 
            var path = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        public static T CreateAssetForScene<T>(string scenePath, string assetName) where T : ScriptableObject, ISceneAsset
        {
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);

            if (sceneAsset == null)
            {
                return null;
            }

            var path = CreateDirectoryFoldersForPath(scenePath, true);
            path = CreateFilePathFromDirectoryPath(path, $"{assetName}.asset", true);

            var asset = ScriptableObject.CreateInstance<T>();
            asset.SceneGuid = AssetDatabase.AssetPathToGUID(scenePath);
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();

            return asset;
        }

        public static string CreateFilePathFromDirectoryPath(string directoryPath, string fileName, bool addFolderNameToFileName)
        {
            if (addFolderNameToFileName)
            {
                string folderName = Path.GetFileName(directoryPath);
                fileName = $"{folderName}_{fileName}";
            }

            string filePath = Path.Combine(directoryPath, fileName).Replace("\\", "/");
            return filePath;
        }

        public static string CreateDirectoryFoldersForPath(string path, bool addFileNameAsFolder)
        {
            string directoryPath = addFileNameAsFolder ? Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path)) : Path.GetDirectoryName(path);
            CreateFoldersIfNecessary(directoryPath);
            return directoryPath;
        }

        private static void CreateFoldersIfNecessary(string folderPath)
        {
            string[] folders = folderPath.Split('\\');
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
        }
    }
}
#endif
