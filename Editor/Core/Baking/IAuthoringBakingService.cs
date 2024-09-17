namespace Scellecs.Morpeh.EntityConverter.Editor.Baking
{
    public interface IAuthoringBakingService
    {
        public void ForceGlobalBake();
        public void BakePrefab(string prefabGUID);
        public void BakeScene(string sceneGUID);
    }
}
