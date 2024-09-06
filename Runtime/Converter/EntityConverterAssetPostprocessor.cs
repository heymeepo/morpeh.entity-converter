#if UNITY_EDITOR
using UnityEditor;

namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class EntityConverterAssetPostprocessor : AssetPostprocessor
    {
        private static EntityConverterRepository repository;

        public EntityConverterAssetPostprocessor(EntityConverterRepository repository) => EntityConverterAssetPostprocessor.repository = repository;

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            if (IsRepositoryIntializeFirstTimeSessionCalled() == false)
            {
                repository.Initialize();
                return;
            }

            if (didDomainReload)
            {
                repository.Reload();
            }

            if (repository.IsValid == false)
            {
                foreach (var assetPath in importedAssets)
                {
                    var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);

                    if (asset as EntityConverterDataAsset != null)
                    {
                        repository.Initialize();
                        break;
                    }
                }

                return;
            }

            foreach (var assetPath in importedAssets)
            {
                var a = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);

                if (a != null)
                {
                    if (a is SceneBakedDataAsset bakedData)
                    {
                        var assetInfo = CreateAssetGUIDInfo(assetPath, AssetGUIDType.SceneBakedData, bakedData.SceneGuid);
                        repository.AddAsset(a, assetInfo);
                    }
                    else if (a is SceneAsset)
                    {
                        var assetInfo = CreateAssetGUIDInfo(assetPath, AssetGUIDType.Scene, string.Empty);
                        repository.AddAsset(a, assetInfo);
                    }
                }
            }

            foreach (var assetPath in deletedAssets)
            {
                var guid = AssetDatabase.AssetPathToGUID(assetPath);
                repository.RemoveAsset(guid);
            }

            repository.SaveDataAndNotifyChanged();
        }

        public static bool IsRepositoryIntializeFirstTimeSessionCalled()
        {
            var result = SessionState.GetBool(EntityConverterUtility.REPOSITORY_INTIALIZE_FIRST_TIME_CALLED, false);

            if (result == false)
            {
                SessionState.SetBool(EntityConverterUtility.REPOSITORY_INTIALIZE_FIRST_TIME_CALLED, true);
            }

            return result;
        }

        private static AssetGUIDInfo CreateAssetGUIDInfo(string assetPath, AssetGUIDType type, string registrationGUID)
        {
            var assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
            var regGUID = registrationGUID == string.Empty ? assetGuid : registrationGUID;
            var assetInfo = new AssetGUIDInfo()
            {
                assetGUID = assetGuid,
                registrationGUID = regGUID,
                type = type
            };
            return assetInfo;
        }
    }
}
#endif
