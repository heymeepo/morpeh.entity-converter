#if UNITY_EDITOR
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter
{
    [DisallowMultipleComponent]
    [Icon("Packages/com.scellecs.morpeh.entity-converter/Editor/DefaultResources/Icons/d_Linker@64.png")]
    public sealed class ConvertToEntity : MonoBehaviour
    {
        [SerializeField]
        internal PrefabBakedDataAsset bakedDataAsset;

        [SerializeField]
        internal bool excludeFromScene;

        public EntityLink GetEntityLink() => new EntityLink() { convertToEntity = this };

        internal EcsAuthoring[] GetAuthorings() => GetComponents<EcsAuthoring>();
    }
}
#endif
