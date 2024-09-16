namespace Scellecs.Morpeh.EntityConverter
{
    internal interface IEntityConverterDataProvider
    {
        public bool TryGetData(out EntityConverterDataAsset data);

        public bool IsValid();
    }
}