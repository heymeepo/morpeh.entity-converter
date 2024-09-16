#if UNITY_EDITOR
namespace Scellecs.Morpeh.EntityConverter
{
    public abstract class SimpleAuthoring<T> : EcsAuthoring where T : struct, IComponent
    {
        public T component;

        public sealed override void OnBake(BakingContext bakingContext, UserContext userContext)
        {
            bakingContext.SetComponent(component);
        }
    }

    public abstract class SimpleAuthoring<T0, T1> : EcsAuthoring
        where T0 : struct, IComponent
        where T1 : struct, IComponent
    {
        public T0 component0;
        public T1 component1;

        public sealed override void OnBake(BakingContext bakingContext, UserContext userContext)
        {
            bakingContext.SetComponent(component0);
            bakingContext.SetComponent(component1);
        }
    }

    public abstract class SimpleAuthoring<T0, T1, T2> : EcsAuthoring
        where T0 : struct, IComponent
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        public T0 component0;
        public T1 component1;
        public T2 component2;

        public sealed override void OnBake(BakingContext bakingContext, UserContext userContext)
        {
            bakingContext.SetComponent(component0);
            bakingContext.SetComponent(component1);
            bakingContext.SetComponent(component2);
        }
    }
}
#endif