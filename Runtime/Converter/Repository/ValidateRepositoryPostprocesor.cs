#if UNITY_EDITOR
using UnityEditor;

namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class ValidateRepositoryPostprocesor : IAssetPostprocessSystem
    {
        private readonly EntityConverterRepository repository;

        public ValidateRepositoryPostprocesor(EntityConverterRepository repository) => this.repository = repository;

        public void Execute(OnAssetPostprocessContext context)
        {
            if (IsRepositoryIntializeCalledFirstTimeThisSession() == false)
            {
                repository.Initialize();
                return;
            }

            if (context.DidDomainReload)
            {
                repository.Reload();
            }

            if (repository.IsValid == false)
            {
                foreach (var assetPath in context.AllImportedAssetsPaths)
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

            foreach (var data in context.ImportedAuthorings)
            {
                switch (data.type)
                {
                    case AuthoringType.Scene:
                        repository.AddScene(data.GUID);
                        break;

                    case AuthoringType.Prefab:
                        repository.AddPrefab(data.GUID);
                        break;

                    case AuthoringType.SceneBakedData:
                        repository.AddSceneBakedData(data.GUID, data.asset as SceneBakedDataAsset);
                        break;
                }
            }

            foreach (var assetPath in context.AllDeletedAssetsPaths)
            {
                var guid = AssetDatabase.AssetPathToGUID(assetPath);
                repository.RemoveAsset(guid);
            }

            repository.SaveDataAndNotifyChanged();
        }

        private static bool IsRepositoryIntializeCalledFirstTimeThisSession()
        {
            var result = SessionState.GetBool(EntityConverterUtility.REPOSITORY_INITIALIZE_FIRST_TIME_CALLED, false);

            if (result == false)
            {
                SessionState.SetBool(EntityConverterUtility.REPOSITORY_INITIALIZE_FIRST_TIME_CALLED, true);
            }

            return result;
        }
    }
}
#endif
