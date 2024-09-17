using System.Collections.Generic;

namespace Scellecs.Morpeh.EntityConverter.Editor
{
    internal sealed class OnAssetPostprocessContext
    {
        public IEnumerable<string> AllImportedAssetsPaths { get; private set; }
        public IEnumerable<string> AllDeletedAssetsPaths { get; private set; }
        public IEnumerable<ImportedAuthoringData> ImportedAuthorings { get; private set; }

        public bool DidDomainReload { get; private set; }

        public OnAssetPostprocessContext(
            IEnumerable<string> allImportedAssetsPaths,
            IEnumerable<string> allDeletedAssetsPaths, 
            IEnumerable<ImportedAuthoringData> importedAuthorings, 
            bool didDomainReload)
        {
            AllImportedAssetsPaths = allImportedAssetsPaths;
            AllDeletedAssetsPaths = allDeletedAssetsPaths;
            ImportedAuthorings = importedAuthorings;
            DidDomainReload = didDomainReload;
        }
    }
}
