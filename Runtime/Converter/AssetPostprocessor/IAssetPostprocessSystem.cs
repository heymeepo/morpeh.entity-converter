#if UNITY_EDITOR
namespace Scellecs.Morpeh.EntityConverter
{
    internal interface IAssetPostprocessSystem
    {
        public void Execute(OnAssetPostprocessContext context);
    }
}
#endif
