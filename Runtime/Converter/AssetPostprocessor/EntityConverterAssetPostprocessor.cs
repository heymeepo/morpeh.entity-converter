#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class EntityConverterAssetPostprocessor : AssetPostprocessor
    {
        private static List<AssetPostprocessSystem> postprocessors = new List<AssetPostprocessSystem>();

        public EntityConverterAssetPostprocessor(List<AssetPostprocessSystem> postprocessors) => EntityConverterAssetPostprocessor.postprocessors = postprocessors;

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            if (postprocessors == null || postprocessors.Count == 0)
            {
                return;
            }

            var importedAuthorings = new List<ImportedAuthoringData>();

            foreach (var assetPath in importedAssets)
            {
                var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                var assetGUID = AssetDatabase.AssetPathToGUID(assetPath);

                if (asset != null)
                {
                    var authoringType = AuthoringType.None;

                    if (asset is SceneBakedDataAsset)
                    {
                        authoringType = AuthoringType.SceneBakedData;
                    }
                    else if (asset is SceneAsset)
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

            var context = new OnAssetPostprocessContext(
                importedAssets,
                deletedAssets,
                importedAuthorings,
                didDomainReload);

            foreach (var postrpocessor in postprocessors) 
            {
                postrpocessor.Execute(context);
            }
        }
    }
}
#endif
