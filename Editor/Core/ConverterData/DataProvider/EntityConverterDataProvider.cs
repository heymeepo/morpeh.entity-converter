using System;
using UnityEditor;

namespace Scellecs.Morpeh.EntityConverter.Editor
{
    internal sealed class EntityConverterDataProvider : IEntityConverterDataProvider, IEntityConverterDataNotifier
    {
        private EntityConverterDataAsset data;
        private bool isDirty;

        public event Action DataChanged;

        public void Reload() => data = AssetDatabase.LoadAssetAtPath<EntityConverterDataAsset>(EntityConverterUtility.DATA_ASSET_PATH);

        public bool IsValid() => data != null;

        public void SetDirty() => isDirty = IsValid();

        public bool TryGetData(out EntityConverterDataAsset data, bool setDirty = false)
        {
            if (setDirty)
            {
                SetDirty();
            }            

            data = this.data;
            return IsValid();
        }

        public void SaveAndNotifyChangedIfDirty()
        {
            if (isDirty)
            {
                EditorUtility.SetDirty(data);
                DataChanged?.Invoke();
                isDirty = false;
            }
        }
    }
}
