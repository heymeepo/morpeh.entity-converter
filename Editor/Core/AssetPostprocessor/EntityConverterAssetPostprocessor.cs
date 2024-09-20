using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter.Editor
{
    internal sealed class EntityConverterAssetPostprocessor : AssetPostprocessor
    {
        private static EntityConverterAssetPostprocessor instance;
        private OnAssetPostprocessContext postProcessContext;

        private EntityConverterAssetPostprocessor() { }

        public static EntityConverterAssetPostprocessor CreateInstance()
        {
            instance ??= new EntityConverterAssetPostprocessor();
            return instance;
        }

        public bool TryGetContext(out OnAssetPostprocessContext context)
        {
            if (postProcessContext != null)
            {
                context = postProcessContext;
                postProcessContext = null;
                return true;
            }

            context = null;
            return false;
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            var importedAuthorings = new List<ImportedAuthoringData>();

            foreach (var assetPath in importedAssets)
            {
                var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                var assetGUID = AssetDatabase.AssetPathToGUID(assetPath);

                if (asset != null)
                {
                    var authoringType = AuthoringType.None;

                    if (asset is SceneAsset)
                    {
                        authoringType = AuthoringType.Scene;
                    }
                    else if (asset is SceneBakedDataAsset)
                    {
                        authoringType = AuthoringType.SceneBakedData;
                    }
                    else if (asset is GameObject go)
                    {
                        if (go.GetComponent<ConvertToEntity>() != null)
                        {
                            authoringType = AuthoringType.Prefab;
                        }
                    }

                    if (authoringType != AuthoringType.None)
                    {
                        importedAuthorings.Add(new ImportedAuthoringData()
                        {
                            GUID = assetGUID,
                            path = assetPath,
                            type = authoringType,
                            asset = asset
                        });
                    }
                }
            }

            if (instance.postProcessContext != null)
            {
                var importedAssetsCombined = new List<string>(instance.postProcessContext.AllImportedAssetsPaths);
                importedAssetsCombined.AddRange(importedAssets);

                var deletedAssetsCombined = new List<string>(instance.postProcessContext.AllDeletedAssetsPaths);
                deletedAssetsCombined.AddRange(deletedAssets);
                
                var importedAuthoringsCombined = new List<ImportedAuthoringData>(instance.postProcessContext.ImportedAuthorings);
                importedAuthoringsCombined.AddRange(importedAuthorings);

                instance.postProcessContext = new OnAssetPostprocessContext(
                    importedAssetsCombined.Distinct(),
                    deletedAssetsCombined.Distinct(),
                    importedAuthoringsCombined.Distinct(ImportedAuthoringDataEqualityComparer.Default),
                    _ = instance.postProcessContext.DidDomainReload);
            }
            else
            {
                instance.postProcessContext = new OnAssetPostprocessContext(
                    importedAssets,
                    deletedAssets,
                    importedAuthorings,
                    didDomainReload);
            }
        }
    }
}
