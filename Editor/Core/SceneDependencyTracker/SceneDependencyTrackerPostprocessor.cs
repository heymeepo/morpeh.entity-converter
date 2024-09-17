namespace Scellecs.Morpeh.EntityConverter.Editor
{
    internal sealed class SceneDependencyTrackerPostprocessor : IAssetPostprocessSystem
    {
        private readonly SceneDependencyTracker dependencyTracker;

        public SceneDependencyTrackerPostprocessor(SceneDependencyTracker dependencyTracker)
        {
            this.dependencyTracker = dependencyTracker;
        }

        public void Execute(OnAssetPostprocessContext context)
        {
            if (context.DidDomainReload)
            {
                dependencyTracker.Reload();
            }
        }
    }
}
