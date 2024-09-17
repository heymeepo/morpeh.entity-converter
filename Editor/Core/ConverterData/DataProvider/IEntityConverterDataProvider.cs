namespace Scellecs.Morpeh.EntityConverter.Editor
{
    internal interface IEntityConverterDataProvider
    {
        public bool TryGetData(out EntityConverterDataAsset data, bool setDirty = false);

        public bool IsValid();

        public void SetDirty();
    }
}