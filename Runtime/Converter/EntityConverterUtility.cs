﻿#if UNITY_EDITOR
using Scellecs.Morpeh.EntityConverter.Utilities;
using UnityEditor;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter
{
    internal static class EntityConverterUtility
    {
        public const string DATA_ASSET_PATH = "Assets/Plugins/Scellecs/Morpeh Entity Converter/Assets/EntityConverterDataAsset.asset";
        public const string ON_POSTPROCESS_ALL_ASSETS_CALLED_FIRST_TIME_KEYWORD = "_ON_POSTPROCESS_ALL_ASSETS_CALLED";

        public static EntityConverterDataAsset CreateDataAssetInstance()
        {
            AssetDatabaseUtility.CreateDirectoryFoldersForPath(DATA_ASSET_PATH, false);
            var converterData = ScriptableObject.CreateInstance<EntityConverterDataAsset>();
            AssetDatabase.CreateAsset(converterData, DATA_ASSET_PATH);
            AssetDatabase.SaveAssets();

            return converterData;
        }

        public static SceneBakedDataAsset CreateSceneBakedDataAsset(string scenePath)
        {
            return AssetDatabaseUtility.CreateAssetForScene<SceneBakedDataAsset>(scenePath, "BakedData");
        }
    }
}
#endif
