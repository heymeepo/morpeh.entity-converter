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

    public abstract class SimpleAuthoring<T1, T2> : EcsAuthoring 
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        public T1 component1;
        public T2 component2;

        public sealed override void OnBake(BakingContext bakingContext, UserContext userContext)
        {
            bakingContext.SetComponent(component1);
            bakingContext.SetComponent(component2);
        }
    }

    public abstract class SimpleAuthoring<T1, T2, T3> : EcsAuthoring 
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
    {
        public T1 component1;
        public T2 component2;
        public T3 component3;

        public sealed override void OnBake(BakingContext bakingContext, UserContext userContext)
        {
            bakingContext.SetComponent(component1);
            bakingContext.SetComponent(component2);
            bakingContext.SetComponent(component3);
        }
    }
}
#endif
