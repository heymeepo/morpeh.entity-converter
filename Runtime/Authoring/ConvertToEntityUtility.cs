#if UNITY_EDITOR
namespace Scellecs.Morpeh.EntityConverter
{
    public static class ConvertToEntityUtility
    {
        public static bool ValidateEntityLinkCompability(this ConvertToEntity source, ConvertToEntity link)
        {
            return true;
        }
    }
}
#endif
