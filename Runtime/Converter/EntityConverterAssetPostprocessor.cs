﻿#if UNITY_EDITOR
using UnityEditor;

namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class EntityConverterAssetPostprocessor : AssetPostprocessor
    {
        private static EntityConverterRepository repository;

        public EntityConverterAssetPostprocessor(EntityConverterRepository repository) => EntityConverterAssetPostprocessor.repository = repository;

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            if (repository.IsValid == false)
            {
                foreach (var assetPath in importedAssets)
                {
                    var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);

                    if (asset as EntityConverterDataAsset != null)
                    {
                        repository.Reload();
                        return;
                    }
                }
            }

            bool isDirty = false;

            foreach (var assetPath in importedAssets)
            {
                var a = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);

                if (a != null)
                {
                    if (a is SceneBakedDataAsset bakedData)
                    {
                        if (repository.TryGetSceneBakedDataAsset(bakedData.SceneGuid, out _) == false)
                        {
                            var assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
                            repository.AddSceneBakedDataAsset(bakedData, assetGuid);
                            isDirty = true;
                        }
                    }
                    else if (a is SceneAsset)
                    {
                        var sceneGuid = AssetDatabase.GUIDFromAssetPath(assetPath).ToString();

                        if (repository.IsSceneGuidExists(sceneGuid) == false)
                        {
                            repository.AddSceneGuid(sceneGuid);
                            isDirty = true;
                        }
                    }
                }
            }

            isDirty |= repository.CollectUnreferenced();

            if (isDirty)
            {
                repository.SaveDataAndNotifyChanged();
            }
        }
    }
}
#endif
