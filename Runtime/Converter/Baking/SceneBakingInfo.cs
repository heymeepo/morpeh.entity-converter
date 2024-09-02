#if UNITY_EDITOR
using UnityEngine.SceneManagement;

namespace Scellecs.Morpeh.EntityConverter
{
    internal struct SceneBakingInfo
    {
        public SceneBakedDataAsset sceneBakedData;
        public Scene scene;
    }
}
#endif
