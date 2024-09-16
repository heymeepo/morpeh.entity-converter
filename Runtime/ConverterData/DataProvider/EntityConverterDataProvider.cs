#if UNITY_EDITOR
using UnityEditor;

namespace Scellecs.Morpeh.EntityConverter
{
    internal sealed class EntityConverterDataProvider : IEntityConverterDataProvider
    {
        private EntityConverterDataAsset data;

        public void Reload() => data = AssetDatabase.LoadAssetAtPath<EntityConverterDataAsset>(EntityConverterUtility.DATA_ASSET_PATH);

        public bool IsValid() => data != null;

        public bool TryGetData(out EntityConverterDataAsset data)
        {
            data = this.data;
            return data != null;
        }
    }
}
#endif
