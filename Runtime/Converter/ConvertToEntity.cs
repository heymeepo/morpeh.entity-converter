#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using Scellecs.Morpeh.EntityConverter.Utilities;

namespace Scellecs.Morpeh.EntityConverter
{
    [Icon("Packages/com.scellecs.morpeh.entity-converter/Editor/DefaultResources/Icons/d_Linker@64.png")]
    public sealed class ConvertToEntity : MonoBehaviour
    {
        [SerializeField]
        internal EntityBakedDataAsset bakedDataAsset;

        [SerializeField]
        internal EntityBakedDataAsset bakedDataSceneAsset;

        [SerializeField]
        internal bool excludeFromScene;

        internal void Bake()
        {
            if(PrefabUtils.IsSceneObject(gameObject) && bakedDataSceneAsset != null) 
            {
                Bake(bakedDataSceneAsset);
            }
            else if(bakedDataAsset != null)
            {
                Bake(bakedDataAsset);
            }
        }

        private void Bake(EntityBakedDataAsset bakedDataAsset)
        {
            var bakedRoots = new List<EntityBakedData>();
            GameObjectConversionUtility.ExportBakedData(gameObject, bakedRoots);
            var serialized = Serialization.SerializationUtility.SerializeBakedData(bakedRoots);
            bakedDataAsset.SetSerializedData(serialized);
        }
    }
}
#endif
