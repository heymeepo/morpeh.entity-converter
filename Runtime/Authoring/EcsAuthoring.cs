#if UNITY_EDITOR
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter
{
    public abstract class EcsAuthoring : MonoBehaviour
    {
        public EntityLink GetEntityLink() => GetComponent<ConvertToEntity>().GetEntityLink();

        public virtual void OnBeforeBake(UserContext userContext) { }

        public abstract void OnBake(BakingContext bakingContext, UserContext userContext);
    }
}
#endif