using PixelSim.Shared.DataContainers.AssetReferencers;
using UnityEngine;

namespace PixelSim.Shared.Backend
{
    public sealed class AssetReferencer : MonoBehaviour
    {
        [SerializeField] private TextureReferencer _textureReferencer;
        [SerializeField] private PrefabReferencer _prefabReferencer;

        public static TextureReferencer Textures => _singleton._textureReferencer;
        public static PrefabReferencer Prefabs => _singleton._prefabReferencer;

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