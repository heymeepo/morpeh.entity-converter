using System.Collections.Generic;

namespace Scellecs.Morpeh.EntityConverter.Editor
{
    internal interface IReadOnlySceneDependencyService
    {
        public IEnumerable<string> GetSceneDependenciesForPrefab(string prefabGUID);
    }
}