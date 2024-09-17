using System.Collections.Generic;

namespace Scellecs.Morpeh.EntityConverter.Editor
{
    internal interface IReadOnlyAuthoringDataService
    {
        public IEnumerable<string> GetSceneGuids();

        public IEnumerable<string> GetPrefabGuids();

        public bool IsPrefabGuidExists(string prefabGuid);

        public bool TryGetSceneBakedDataAsset(string sceneGuid, out SceneBakedDataAsset sceneBakedData);
    }
}