#if UNITY_EDITOR
using Scellecs.Morpeh.EntityConverter.Utilities;
using Scellecs.Morpeh.Workaround.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class EntityConverter
    {
        private EntityConverterAssetPostprocessor assetPostprocessor;
        private EntityConverterBuildPreprocessor buildPreprocessor;
        private EntityConverterServiceProvider serviceProvider;
        private EntityConverterRepository repository;
        private AuthoringBakingService bakingService;
        private BakingProcessor bakingProcessor;

        public void Initialize()
        {
            repository = new EntityConverterRepository();
            buildPreprocessor = new EntityConverterBuildPreprocessor(repository);
            bakingProcessor = new BakingProcessor();
            bakingService = new AuthoringBakingService(repository, bakingProcessor);
            serviceProvider = EntityConverterServiceProvider.CreateInstance(repository, bakingService);

            var postprocessors = new List<IAssetPostprocessSystem>();

            postprocessors.Add(new ValidateRepositoryPostprocesor(repository));
            postprocessors.Add(new AutoRebakingPostprocessor(bakingService));
            postprocessors.AddRange(CreateUserDefinedPostprocessors());
            postprocessors.Add(new RestoreActiveSelectionPostprocessor());

            assetPostprocessor = EntityConverterAssetPostprocessor.CreateInstance(postprocessors);
        }

        private IEnumerable<IAssetPostprocessSystem> CreateUserDefinedPostprocessors()
        {
            var types = ReflectionHelpers.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(AssetPostprocessSystem).IsAssignableFrom(type) && type.IsAbstract == false);

            var instances = new List<IAssetPostprocessSystem>();

            foreach (var type in types)
            {
                try
                {
                    var postprocessor = Activator.CreateInstance(type) as AssetPostprocessSystem;
                    postprocessor.Repository = repository;
                    postprocessor.BakingService = bakingService;
                    instances.Add(postprocessor);
                }
                catch (MissingMethodException exc)
                {
                    UnityEngine.Debug.LogWarning($"{exc.Message}. Using parameterized constructors for types derived from AssetPostprocessorSystem is not allowed");                    
                }
            }

            return instances;
        }
    }
}
#endif
