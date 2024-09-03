using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter
{
    [PreferBinarySerialization]
    [CreateAssetMenu(fileName = "PrefabBakedData", menuName = "ECS/Baker/PrefabBakedDataAsset")]
    public sealed class PrefabBakedDataAsset : BakedDataAsset { }
}
