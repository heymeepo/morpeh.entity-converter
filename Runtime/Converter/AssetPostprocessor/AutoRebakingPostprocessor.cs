#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class AutoRebakingPostprocessor : AssetPostprocessSystem
    {
        private readonly AuthoringBakingService bakingService;

        public AutoRebakingPostprocessor(AuthoringBakingService bakingService)
        {
            this.bakingService = bakingService;
        }

        public override void Execute(OnAssetPostprocessContext context)
        {
            if (context.DidDomainReload)
            {
                bakingService.BakeGlobal();
                return;
            }

            foreach (var importedAuthoringData in context.ImportedAuthorings)
            {
                switch (importedAuthoringData.type)
                {
                    case AuthoringType.Prefab:
                        RebakePrefab(importedAuthoringData.GUID);
                        break;

                    case AuthoringType.Scene:
                        RebakeScene(importedAuthoringData.GUID);
                        break;
                }
            }
        }

        private void RebakePrefab(string prefabGUID)
        {
            bakingService.BakePrefab(prefabGUID);
        }

        private void RebakeScene(string sceneGUID) 
        { 
            bakingService.BakeScene(sceneGUID);
        }
    }
}
#endif
