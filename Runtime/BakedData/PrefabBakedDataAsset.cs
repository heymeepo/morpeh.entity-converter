using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter
{
    [PreferBinarySerialization]
    [CreateAssetMenu(fileName = "PrefabBakedData", menuName = "ECS/Baker/EntityBakedDataAsset")]
    public sealed class PrefabBakedDataAsset : BakedDataAsset { }
}
