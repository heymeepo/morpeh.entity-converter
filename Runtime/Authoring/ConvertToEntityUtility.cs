#if UNITY_EDITOR
using UnityEditor;

namespace Scellecs.Morpeh.EntityConverter
{
    public static class ConvertToEntityUtility
    {
        public static bool ValidateEntityLinkCompability(this ConvertToEntity source, ConvertToEntity link)
        {
            if (source != null && link != null)
            {
                var sourceSceneName = source.gameObject.scene.name;
                var linkSceneName = link.gameObject.scene.name;

                if (string.IsNullOrEmpty(sourceSceneName) == false && string.IsNullOrEmpty(linkSceneName) == false)
                {
                    if (sourceSceneName == linkSceneName)
                    {
                        return true;
                    }
                }

                var sourcePath = AssetDatabase.GetAssetPath(source);
                var linkPath = AssetDatabase.GetAssetPath(link);

                if (string.IsNullOrEmpty(sourcePath) == false && string.IsNullOrEmpty(linkPath) == false)
                {
                    if (sourcePath == linkPath)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
#endif
