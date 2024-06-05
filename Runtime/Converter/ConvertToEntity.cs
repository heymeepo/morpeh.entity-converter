#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using Scellecs.Morpeh.EntityConverter.Serialization;

namespace Scellecs.Morpeh.EntityConverter
{
    public sealed class ConvertToEntity : MonoBehaviour
    {
        [SerializeField]
        private EntityBakedDataAsset bakedAsset;

        [ContextMenu("Bake")]
        public void Bake()
        {
            if (bakedAsset != null)
            {
                var bakedRoots = new List<EntityBakedData>();
                GameObjectConversionUtility.ExportBakedData(gameObject, bakedRoots);
                var serialized = SerializationUtility.SerializeBakedData(bakedRoots);
                bakedAsset.SetSerializedData(serialized);
                gameObject.tag = "EditorOnly";
                UnityEditor.EditorUtility.SetDirty(bakedAsset);
                UnityEditor.EditorUtility.SetDirty(gameObject);
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
            }
        }
    }
}
#endif
