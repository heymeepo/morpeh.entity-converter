using System.Collections.Generic;

namespace Scellecs.Morpeh.EntityConverter
{
    internal interface IReadOnlySceneDependencyService
    {
        public IEnumerable<string> GetSceneDependenciesForPrefab(string prefabGUID);
    }
}