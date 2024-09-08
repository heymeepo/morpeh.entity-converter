#if UNITY_EDITOR
namespace Scellecs.Morpeh.EntityConverter
{
    public interface IAssetPostprocessSystem
    {
        public void Execute(OnAssetPostprocessContext context);
    }
}
#endif
