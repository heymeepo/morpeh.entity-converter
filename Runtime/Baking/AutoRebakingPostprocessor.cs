#if UNITY_EDITOR
using Scellecs.Morpeh.EntityConverter.Logs;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class AutoRebakingPostprocessor : IAssetPostprocessSystem
    {
        private readonly AuthoringBakingService bakingService;
        private readonly IReadOnlySettingsService settingsService;
        private readonly IReadOnlySceneDependencyService sceneDependencyService;
        private readonly ILogger logger;

        public List<string> sceneGUIDs;
        public List<string> prefabGUIDs;

        public AutoRebakingPostprocessor(
            AuthoringBakingService bakingService, 
            IReadOnlySettingsService settingsService,
            IReadOnlySceneDependencyService sceneDependencyService, 
            ILogger logger)
        {
            this.bakingService = bakingService;
            this.settingsService = settingsService;
            this.sceneDependencyService = sceneDependencyService;
            this.logger = logger;

            sceneGUIDs = new List<string>();
            prefabGUIDs = new List<string>();
        }

        public void Execute(OnAssetPostprocessContext context)
        {
            if (context.DidDomainReload)
            {
                if (settingsService.TryGetBakingFlags(out var flags) && (flags & BakingFlags.BakeOnDomainReload) != 0)
                {
                    bakingService.ForceGlobalBake();
                    return;
                }
            }

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
            if (settingsService.TryGetBakingFlags(out var flags) && (flags & BakingFlags.BakePrefabs) != 0)
            {
                prefabGUIDs.Add(prefabGUID);
                sceneGUIDs.AddRange(sceneDependencyService.GetSceneDependenciesForPrefab(prefabGUID));
            }
        }

        private void AddRebakeScene(string sceneGUID)
        {
            if (settingsService.TryGetBakingFlags(out var flags) && (flags & BakingFlags.BakeScenes) != 0)
            {
                sceneGUIDs.Add(sceneGUID);
            }
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

                logger.Log($"Total auto rebaked: {prefabGUIDs.Count} prefabs, {sceneGUIDs.Count()} scenes", LogDepthFlags.Info);
            }
        }
    }
}
#endif
