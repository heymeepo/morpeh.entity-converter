#if UNITY_EDITOR
using System;

namespace Scellecs.Morpeh.EntityConverter
{
    [Serializable]
    internal struct AssetGUIDInfo
    {
        public AuthoringType type;
        public string registrationGUID;
        public string assetGUID;
    }
}
#endif
