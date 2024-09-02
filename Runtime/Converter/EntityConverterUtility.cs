#if UNITY_EDITOR
using Scellecs.Morpeh.EntityConverter.Utilities;
using UnityEditor;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter
{
    internal static class EntityConverterUtility
    {
        public const string ASSET_PATH = "Assets/Plugins/Scellecs/Morpeh Entity Converter/Assets/EntityConverterDataAsset.asset";

        public static EntityConverterDataAsset CreateDataAssetInstance()
        {
            AssetDatabaseUtility.CreateDirectoryFoldersForPath(ASSET_PATH, false);
            var converterData = ScriptableObject.CreateInstance<EntityConverterDataAsset>();
            AssetDatabase.CreateAsset(converterData, ASSET_PATH);
            AssetDatabase.SaveAssets();

            return converterData;
        }

        internal static SceneBakedDataAsset CreateSceneBakedDataAsset(string scenePath)
        {
            return AssetDatabaseUtility.CreateAssetForScene<SceneBakedDataAsset>(scenePath, "BakedData");
        }
    }
}
#endif
