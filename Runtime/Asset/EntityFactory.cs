using Scellecs.Morpeh.Transforms;
using System;

namespace Scellecs.Morpeh.EntityConverter
{
    public struct EntityFactory : IDisposable
    {
        public readonly bool IsValid => world.IsNullOrDisposed() == false && asset != null && asset.RootsCount != 0;

        private World world;
        private EntityBakedDataAsset asset;

        public EntityFactory(EntityBakedDataAsset asset, World world)
        {
            this.asset = asset;
            this.world = world;
        }

        public void Dispose() { }

        /// <summary>
        /// Creates a hierarchy of entities based on the asset's baked roots.
        /// </summary>
        /// <returns>
        /// A span of entities representing the hierarchy roots, but not the full hierarchy. If no roots are present, an empty span is returned.
        /// </returns>
        public unsafe Span<Entity> CreateHierarchy()
        {
            if (IsValid)
            {
                int count = asset.RootsCount;
                Entity* roots = stackalloc Entity[count];

                for (int i = 0; i < count; i++)
                {
                    var ent = world.CreateEntity();
                    CreateHierarchy(asset.GetRoot(i), ent);
                    roots[i] = ent;
                }

                return new Span<Entity>(roots, count);
            }

            return Span<Entity>.Empty;
        }

        private void CreateHierarchy(EntityBakedData bakedData, Entity parent)
        {
            for (int i = 0; i < bakedData.ChildsCount; i++)
            {
                var ent = world.CreateEntity();
                ent.SetParent(parent);
                CreateHierarchy(bakedData.GetChild(i), ent);
            }

            bakedData.SetToEntity(parent, world);
        }
    }
}
