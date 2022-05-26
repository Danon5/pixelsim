using PixelSim.DataContainers.AssetReferencers;
using UnityEngine;

namespace PixelSim.Backend
{
    public sealed class AssetReferencer : MonoBehaviour
    {
        [SerializeField] private TextureReferencer _textureReferencer;

        public static TextureReferencer Textures => _singleton._textureReferencer;

        private static AssetReferencer _singleton;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            _singleton = null;
        }

        private void Awake()
        {
            _singleton = this;
        }
    }
}