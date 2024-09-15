#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class AutoRebakingPostprocessor : IAssetPostprocessSystem
    {
        private readonly AuthoringBakingService bakingService;
        private readonly IReadOnlyEntityConverterRepository repository;

        public List<string> sceneGUIDs;
        public List<string> prefabGUIDs;

        public AutoRebakingPostprocessor(AuthoringBakingService bakingService, IReadOnlyEntityConverterRepository repository)
        {
            this.bakingService = bakingService;
            this.repository = repository;
            sceneGUIDs = new List<string>();
            prefabGUIDs = new List<string>();
        }

        public void Execute(OnAssetPostprocessContext context)
        {
            if (repository.IsValid == false)
            {
                return;
            }

            //if (context.DidDomainReload)
            //{
            //    bakingService.ForceGlobalBake();
            //    return;
            //}

            sceneGUIDs.Clear();
            prefabGUIDs.Clear();

            foreach (var importedAuthoringData in context.ImportedAuthorings)
            {
                switch (importedAuthoringData.type)
                {
                    case AuthoringType.Prefab:
                        AddRebakePrefab(importedAuthoringData.GUID);
                        break;

                    case AuthoringType.Scene:
                        AddRebakeScene(importedAuthoringData.GUID);
                        break;
                }
            }

            ExecuteRebake();
        }

        private void AddRebakePrefab(string prefabGUID)
        {
            prefabGUIDs.Add(prefabGUID);
            sceneGUIDs.AddRange(repository.GetSceneDependenciesForPrefab(prefabGUID));
        }

        private void AddRebakeScene(string sceneGUID)
        {
            sceneGUIDs.Add(sceneGUID);
        }

        private void ExecuteRebake()
        {
            if (sceneGUIDs.Any() || prefabGUIDs.Any())
            {
                var sceneGUIDs = this.sceneGUIDs.Distinct();

                bakingService.SavePreBakingEditorState();
                bakingService.SaveDirtyBeforeBaking();

                foreach (var prefabGUID in prefabGUIDs)
                {
                    bakingService.BakePrefabInternal(prefabGUID);
                }

                foreach (var sceneGUID in sceneGUIDs)
                {
                    bakingService.BakeSceneInternal(sceneGUID);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                bakingService.RestorePreBakingEditorState();
            }
        }
    }
}
#endif
