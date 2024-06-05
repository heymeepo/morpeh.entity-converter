namespace Scellecs.Morpeh.EntityConverter
{
    public sealed class WarmupComponentsTypesInitializer : IInitializer
    {
        public World World { get; set; }

        public void OnAwake() => ComponentsTypeCache.WarmupComponentsTypes(World);

        public void Dispose() { }
    }
}
