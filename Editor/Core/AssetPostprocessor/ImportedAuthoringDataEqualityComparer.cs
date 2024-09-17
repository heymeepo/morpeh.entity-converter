using System.Collections.Generic;

namespace Scellecs.Morpeh.EntityConverter.Editor
{
    internal sealed class ImportedAuthoringDataEqualityComparer : IEqualityComparer<ImportedAuthoringData>
    {
        public static ImportedAuthoringDataEqualityComparer Default = new ImportedAuthoringDataEqualityComparer();

        public bool Equals(ImportedAuthoringData x, ImportedAuthoringData y) => x.GUID == y.GUID;

        public int GetHashCode(ImportedAuthoringData obj) => obj.GUID.GetHashCode();
    }
}
