namespace Scellecs.Morpeh.EntityConverter.Editor
{
    internal interface IEntityConverterDataProvider
    {
        public bool TryGetData(out EntityConverterDataAsset data);

        public bool IsValid();
    }
}