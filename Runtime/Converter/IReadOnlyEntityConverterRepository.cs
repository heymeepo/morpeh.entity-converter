#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace Scellecs.Morpeh.EntityConverter
{
    internal interface IReadOnlyEntityConverterRepository
    {
        public bool IsValid { get; }

        public event Action RepositoryDataChanged;

        public bool TryGetSceneBakedDataAsset(string sceneGuid, out SceneBakedDataAsset sceneBakedData);

        public bool IsSceneGuidExists(string sceneGuid);

        public IEnumerator<string> GetSceneGuids();
    }
}
#endif