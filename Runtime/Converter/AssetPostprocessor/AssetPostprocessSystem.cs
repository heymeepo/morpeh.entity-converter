#if UNITY_EDITOR
namespace Scellecs.Morpeh.EntityConverter
{
    public abstract class AssetPostprocessSystem : IAssetPostprocessSystem
    { 
        public IReadOnlyEntityConverterRepository Repository { get; internal set; }
        public IAuthoringBakingService BakingService { get; internal set; }

        public abstract void Execute(OnAssetPostprocessContext context);
    }
}
#endif
