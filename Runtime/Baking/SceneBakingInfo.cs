#if UNITY_EDITOR
using UnityEngine.SceneManagement;

namespace Scellecs.Morpeh.EntityConverter
{
    internal struct SceneBakingInfo
    {
        public BakedDataAsset bakedData;
        public Scene scene;
    }
}
#endif
