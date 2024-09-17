using UnityEditor.Build;

namespace Scellecs.Morpeh.EntityConverter.Editor
{
    internal sealed class EntityConverterBuildPreprocessor : BuildPlayerProcessor
    {
        //private readonly IReadOnlyEntityConverterRepository repository;
        //public override int callbackOrder => 1;

        //public EntityConverterBuildPreprocessor(IReadOnlyEntityConverterRepository repository)
        //{
        //    this.repository = repository;
        //}

        public override void PrepareForBuild(BuildPlayerContext buildPlayerContext)
        {
            //var converter = EntityConverter.Instance;

            //if (converter != null && (converter.bakingFlags & EntityConverterBakingFlags.BakeOnBuild) != 0)
            //{
            //    converter.ForceFullRebake();
            //}
        }
    }
}
