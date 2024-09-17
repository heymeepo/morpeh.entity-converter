using System;

namespace Scellecs.Morpeh.EntityConverter.Editor
{
    internal interface IEntityConverterDataNotifier
    {
        public event Action DataChanged;

        public bool IsValid();
    }
}
