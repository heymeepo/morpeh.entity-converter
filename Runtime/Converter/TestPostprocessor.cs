#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class TestPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            //foreach (var asset in importedAssets)
            //{
            //    Debug.Log($"imported: {asset}");
            //}

            //foreach (var asset in deletedAssets)
            //{
            //    Debug.Log($"deleted: {asset}");
            //}

            //foreach (var asset in movedAssets)
            //{
            //    Debug.Log($"moved: {asset}");
            //}
        }
    }
}
#endif
