#if UNITY_EDITOR
namespace Scellecs.Morpeh.EntityConverter
{
    internal struct ImportedAuthoringData
    {
        public string GUID;
        public string path;
        public AuthoringType type;
        public UnityEngine.Object asset;
    }
}
#endif
