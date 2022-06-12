using UnityEngine;

namespace PixelSim.DataContainers.AssetReferencers
{
    [CreateAssetMenu(
        fileName = nameof(TextureReferencer), 
        menuName = "AssetReferencers/" + nameof(TextureReferencer), order = 0)]
    public sealed class TextureReferencer : ScriptableObject
    {
        [field: SerializeField] public Texture2D Dirt_Course { get; private set; }
    }
}