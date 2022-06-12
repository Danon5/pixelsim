using UnityEngine;

namespace PixelSim.DataContainers.AssetReferencers
{
    [CreateAssetMenu(
        fileName = nameof(PrefabReferencer), 
        menuName = "AssetReferencers/" + nameof(PrefabReferencer), order = 0)]
    public sealed class PrefabReferencer : ScriptableObject
    {
        [field: SerializeField] public GameObject ChunkRendererPrefab { get; private set; }
    }
}