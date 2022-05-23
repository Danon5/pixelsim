using UnityEngine;

namespace PixelSim.ScriptableObjects.AssetReferencers
{
    [CreateAssetMenu(fileName = "TextureReferencer", menuName = "AssetReferencers/TextureReferencer", order = 0)]
    public sealed class TextureReferencer : ScriptableObject
    {
        [field: SerializeField] public Texture2D Dirt_Course { get; private set; }
    }
}