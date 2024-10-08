﻿using Scellecs.Morpeh.EntityConverter.Editor.Baking;
using Scellecs.Morpeh.EntityConverter.Utilities;
using UnityEditor;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter.Editor
{
    internal static class EntityConverterUtility
    {
        public const string DATA_ASSET_PATH = "Assets/Plugins/Scellecs/Morpeh Entity Converter/Assets/EntityConverterDataAsset.asset";

        public static EntityConverterDataAsset CreateDataAssetInstance()
        {
            AssetDatabaseUtility.CreateDirectoryFoldersForPath(DATA_ASSET_PATH, false);
            var converterData = ScriptableObject.CreateInstance<EntityConverterDataAsset>();
            AssetDatabase.CreateAsset(converterData, DATA_ASSET_PATH);
            converterData.ConverterSettings.bakingFlags = (BakingFlags)0xFF;
            EditorUtility.SetDirty(converterData);
            AssetDatabase.SaveAssets();
            return converterData;
        }

        public static SceneBakedDataAsset CreateSceneBakedDataAsset(string scenePath)
        {
            return AssetDatabaseUtility.CreateAssetForScene<SceneBakedDataAsset>(scenePath, "BakedData");
        }
    }
}
