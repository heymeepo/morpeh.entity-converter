#if UNITY_EDITOR
namespace Scellecs.Morpeh.EntityConverter
{
    internal abstract class AssetPostprocessSystem
    { 
        public abstract void Execute(OnAssetPostprocessContext context);
    }
}
#endif
