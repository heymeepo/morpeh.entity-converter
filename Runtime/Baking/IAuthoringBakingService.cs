#if UNITY_EDITOR
namespace Scellecs.Morpeh.EntityConverter
{
    public interface IAuthoringBakingService
    {
        public void ForceGlobalBake();
        public void BakePrefab(string prefabGUID);
        public void BakeScene(string sceneGUID);
    }
}
#endif
