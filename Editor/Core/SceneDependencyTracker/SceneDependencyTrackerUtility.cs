namespace Scellecs.Morpeh.EntityConverter.Editor
{
    internal static class SceneDependencyTrackerUtility
    {
        public static string ExtractGuidFromMissingPrefabName(string name)
        {
            const string guidPrefix = "Missing Prefab with guid: ";
            int startIndex = name.LastIndexOf(guidPrefix);

            if (startIndex != -1)
            {
                startIndex += guidPrefix.Length;
                int endIndex = name.IndexOf(')', startIndex);

                if (endIndex == -1)
                {
                    endIndex = name.Length;
                }

                return name.Substring(startIndex, endIndex - startIndex);
            }

            return null;
        }
    }
}
