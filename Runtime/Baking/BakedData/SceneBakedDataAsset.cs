using Scellecs.Morpeh.EntityConverter.Utilities;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter
{
    [PreferBinarySerialization]
    public sealed class SceneBakedDataAsset : BakedDataAsset, ISceneAsset
    {
        [field: SerializeField]
        public string SceneGuid { get; set; }
    }
}
