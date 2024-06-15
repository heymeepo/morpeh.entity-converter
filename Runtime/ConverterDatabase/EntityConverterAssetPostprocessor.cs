using UnityEditor;

namespace Scellecs.Morpeh.EntityConverter
{
    public class EntityConverterAssetPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            EntityConverterDatabase database = EntityConverterDatabase.GetInstance();

            if (database == null)
            {
                return;
            }

            bool shouldSave = false;

            foreach (string assetPath in importedAssets)
            {
                var a = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);

                if (a != null) 
                {
                    if (a is SceneBakedDataAsset bakedData)
                    {
                        database.AddSceneBakedDataAsset(bakedData);
                        shouldSave = true;
                    }
                    else if (a is SceneAsset)
                    {
                        database.AddSceneGuid(AssetDatabase.GUIDFromAssetPath(assetPath).ToString());
                        shouldSave = true;
                    }
                }
            }

            if (deletedAssets.Length > 0)
            {
                shouldSave |= database.ClearUnreferenced();
            }

            if (shouldSave)
            {
                EditorUtility.SetDirty(database);
                AssetDatabase.SaveAssets();
                database.RaiseDatabaseChanged();
            }
        }
    }
}
