#if UNITY_EDITOR
using UnityEditor;

namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class ValidateAuthoringDataPostprocessor : IAssetPostprocessSystem
    {
        private readonly AuthoringDataService authoringDataService;

        public ValidateAuthoringDataPostprocessor(AuthoringDataService authoringDataService)
        {
            this.authoringDataService = authoringDataService;
        }

        public void Execute(OnAssetPostprocessContext context)
        {
            foreach (var data in context.ImportedAuthorings)
            {
                switch (data.type)
                {
                    case AuthoringType.Scene:
                        authoringDataService.AddAuthoringScene(data.GUID);
                        break;

                    case AuthoringType.Prefab:
                        authoringDataService.AddAuthoringPrefab(data.GUID);
                        break;

                    case AuthoringType.SceneBakedData:
                        authoringDataService.AddSceneBakedData(data.GUID, data.asset as SceneBakedDataAsset);
                        break;
                }
            }

            foreach (var assetPath in context.AllDeletedAssetsPaths)
            {
                var guid = AssetDatabase.AssetPathToGUID(assetPath);
                authoringDataService.RemoveAuthoringAsset(guid);
            }
        }
    }
}
#endif
