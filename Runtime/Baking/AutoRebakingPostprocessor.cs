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

        public List<string> sceneGUIDsBuffer;
        public List<string> prefabGUIDsBuffer;

        public AutoRebakingPostprocessor(AuthoringBakingService bakingService, IReadOnlyEntityConverterRepository repository)
        {
            this.bakingService = bakingService;
            this.repository = repository;
            sceneGUIDsBuffer = new List<string>();
            prefabGUIDsBuffer = new List<string>();
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

            sceneGUIDsBuffer.Clear();
            prefabGUIDsBuffer.Clear();

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
            prefabGUIDsBuffer.Add(prefabGUID);
            sceneGUIDsBuffer.AddRange(repository.GetSceneDependenciesForPrefab(prefabGUID));
        }

        private void AddRebakeScene(string sceneGUID)
        {
            sceneGUIDsBuffer.Add(sceneGUID);
        }

        private void ExecuteRebake()
        {
            try
            {
                bakingService.SaveDirtyBeforeBaking();
                var sceneGUIDs = sceneGUIDsBuffer.Distinct();

                foreach (var prefabGUID in prefabGUIDsBuffer)
                {
                    bakingService.BakePrefabInternal(prefabGUID);
                }

                foreach (var sceneGUID in sceneGUIDs)
                {
                    bakingService.BakeSceneInternal(sceneGUID);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            catch (System.Exception ex) 
            { 
                UnityEngine.Debug.Log(ex.Message);
            }
        }
    }
}
#endif
