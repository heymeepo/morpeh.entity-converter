#if UNITY_EDITOR
namespace Scellecs.Morpeh.EntityConverter
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
                dependencyTracker.Initialize();
            }
        }
    }
}
#endif
