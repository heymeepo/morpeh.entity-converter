using System;

namespace Scellecs.Morpeh.EntityConverter.Editor
{
    [Serializable]
    internal struct AssetGUIDInfo
    {
        public AuthoringType type;
        public string registrationGUID;
        public string assetGUID;
    }
}
